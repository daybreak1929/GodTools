using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GodTools.Abstract;
using GodTools.UI.Prefabs;
using GodTools.UI.Prefabs.TopElementPrefabs;
using GodTools.UI.WindowTopComponents;
using GodTools.Utils;
using NeoModLoader.api;
using NeoModLoader.api.attributes;
using NeoModLoader.General;
using NeoModLoader.General.Game.extensions;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.UI;

public partial class WindowTops : AbstractWideWindow<WindowTops>
{
    private const float single_element_height = 40;
    private const float start_y               = -single_element_height / 2;

    private readonly List<SortKey>                     sort_keys = new();
    
    private readonly HashSet<FilterSetting> last_and_filter_settings = new();
    private readonly HashSet<FilterSetting> last_or_filter_settings = new();
    private readonly HashSet<FilterSetting> last_not_filter_settings = new();
    private readonly List<FilterSetting> all_filter_settings = new();
    
    private          List<Actor>                       _list;
    private          ObjectPoolGenericMono<TopElementActor> _pool;
    private          RectTransform                     content_rect;
    private          MonoObjPool<FilterButton>         filter_button_pool;

    private RectTransform filter_content_rect;
    private RectTransform keyword_content_rect;
    private int           last_view_end = -1;

    private int           last_view_start = 9999;
    private RectTransform scroll_view_rect;

    private SingleRowGrid selected_filters;
    private SingleRowGrid selected_keywords;

    private MonoObjPool<SortKeyButton> sort_button_pool;
    private bool                       ui_filters_dirty = true;

    private bool ui_sort_keys_dirty = true;

    private void Update()
    {
        if (Initialized) OnUpdate();
    }

    protected override void Init()
    {
        content_rect = ContentTransform.GetComponent<RectTransform>();
        scroll_view_rect = content_rect.parent.parent.GetComponent<RectTransform>();
        scroll_view_rect.localPosition = new Vector3(0, 10);
        scroll_view_rect.sizeDelta = new Vector2(200, 230);
        content_rect.pivot = new Vector2(0.5f,        1);
        BackgroundTransform.Find("Scrollgradient").localPosition = new(97.8f, -106);

        SimpleLine line1 = Instantiate(SimpleLine.Prefab, BackgroundTransform);
        line1.transform.localPosition = new Vector3(-100, 0);
        line1.SetSize(new Vector2(2, 250));
        SimpleLine line2 = Instantiate(SimpleLine.Prefab, BackgroundTransform);
        line2.transform.localPosition = new Vector3(100, 0);
        line2.SetSize(new Vector2(2, 250));

        RectTransform filter_scroll_view = Instantiate(scroll_view_rect, BackgroundTransform);
        filter_scroll_view.localPosition = new Vector3(-200, 0);
        filter_scroll_view.name = "Filter Scroll View";
        RectTransform keyword_scroll_view = Instantiate(scroll_view_rect, BackgroundTransform);
        keyword_scroll_view.localPosition = new Vector3(200, 0);
        keyword_scroll_view.name = "Keyword Scroll View";

        filter_content_rect = filter_scroll_view.Find("Viewport/Content").GetComponent<RectTransform>();
        keyword_content_rect = keyword_scroll_view.Find("Viewport/Content").GetComponent<RectTransform>();

        filter_content_rect.gameObject.AddComponent<ContentSizeFitter>().verticalFit =
            ContentSizeFitter.FitMode.MinSize;
        keyword_content_rect.gameObject.AddComponent<ContentSizeFitter>().verticalFit =
            ContentSizeFitter.FitMode.MinSize;
        filter_content_rect.gameObject.AddComponent<VerticalLayoutGroup>().SetSimpleVertLayout();
        keyword_content_rect.gameObject.AddComponent<VerticalLayoutGroup>().SetSimpleVertLayout();


        selected_filters = Instantiate(SingleRowGrid.Prefab,  filter_content_rect);
        selected_keywords = Instantiate(SingleRowGrid.Prefab, keyword_content_rect);

        selected_filters.Setup($"{C.mod_prefix}.ui.filter", new Vector2(180,   35), new Vector2(28, 28));
        selected_keywords.Setup($"{C.mod_prefix}.ui.keyword", new Vector2(180, 35), new Vector2(28, 28));
        
        
        CreateGrid_VANILLA();
#if INMNY_CUSTOMMODT001
        CreateGrid_INMNY_CUSTOMMODT001();
#endif
#if INMNY_CUSTOMMODT002
        CreateGrid_INMNY_CUSTOMMODT002();
#endif
#if CULTIWAY
        CreateGrid_CULTIWAY();
#endif
#if WITCHCRAFT_WORLDBOX
        CreateGrid_WITCHCRAFT_WORLDBOX();
#endif
#if ZHETIAN
        CreateGrid_ZHETIAN();
#endif

        var to_top_button = RawLocalizedText.Instantiate(BackgroundTransform, pName: "ToTop");
        to_top_button.SetSize(new(30,10));
        to_top_button.Setup($"{C.mod_prefix}.ui.to_top", alignment: TextAnchor.MiddleLeft);
        to_top_button.transform.localPosition = new(-70, -120);
        to_top_button.gameObject.AddComponent<Button>().onClick.AddListener(ToTop);
        var to_bottom_button = RawLocalizedText.Instantiate(BackgroundTransform, pName: "ToBottom");
        to_bottom_button.SetSize(new(30,10));
        to_bottom_button.Setup($"{C.mod_prefix}.ui.to_bottom", alignment: TextAnchor.MiddleRight);
        to_bottom_button.transform.localPosition = new(70, -120);
        to_bottom_button.gameObject.AddComponent<Button>().onClick.AddListener(ToBottom);

        var jump_input = TextInput.Instantiate(BackgroundTransform, pName: "Jump");
        jump_input.Setup("", CheckJump);
        jump_input.SetSize(new(80, 20));
        jump_input.text.alignment = TextAnchor.MiddleCenter;
        jump_input.input.characterValidation = InputField.CharacterValidation.Integer;
        jump_input.transform.localPosition = new(0, -118);
        var placeholder_text = RawLocalizedText.Instantiate(jump_input.input.transform, pName: "Placeholder");
        placeholder_text.Setup($"{C.mod_prefix}.ui.jump_to", new Color(1,1,1,0.3f),alignment: TextAnchor.MiddleCenter);
        placeholder_text.SetSize(new(60, 20));
        placeholder_text.transform.localPosition = new(33, 0);
        jump_input.input.placeholder = placeholder_text.Text;

        _pool = new ObjectPoolGenericMono<TopElementActor>(TopElementActor.Prefab, ContentTransform);
        sort_button_pool = new MonoObjPool<SortKeyButton>(SortKeyButton.Prefab, selected_keywords.Grid.transform);
        filter_button_pool = new MonoObjPool<FilterButton>(FilterButton.Prefab, selected_filters.Grid.transform);
    }

    [Hotfixable]
    public void ApplySort()
    {
        _list = World.world.units.getSimpleList().FindAll([Hotfixable](x)=>x!=null && x.data != null&& x.isAlive() && !x.object_destroyed && x.asset.canBeInspected);

        if (last_and_filter_settings.Count > 0)
        {
            foreach (FilterSetting setting in last_and_filter_settings)
            {
                _list = _list.FindAll(setting.FilterFunc.Invoke);
            }
        }
        if (last_or_filter_settings.Count > 0)
        {
            List<Actor> or_list = new();
            foreach (FilterSetting setting in last_or_filter_settings)
            {
                or_list.AddRange(_list.FindAll(setting.FilterFunc.Invoke));
            }
            if (last_and_filter_settings.Count > 0)
                _list = _list.Intersect(or_list).ToList();
            else
                _list = or_list;
        }
        if (last_not_filter_settings.Count > 0)
        {
            foreach (FilterSetting setting in last_not_filter_settings)
            {
                _list = _list.FindAll(x => !setting.FilterFunc.Invoke(x));
            }
        }
        
        content_rect.sizeDelta = new Vector2(0, (_list.Count + 1) * single_element_height);
        _list.Sort((a, b) =>
        {
            foreach (SortKey key in sort_keys)
            {
                var result = key.Compare(a, b);
                if (result != 0)
                    return result;
            }

            return 0;
        });
        last_view_start = 9999;
        last_view_end = -1;
        _pool.clear();
    }

    private TitledGrid new_filter_grid(string filter_type)
    {
        TitledGrid grid = Instantiate(TitledGrid.Prefab, filter_content_rect);
        var group_id = $"{C.mod_prefix}.ui.filter.{filter_type}";
        grid.Setup(group_id, 180, new Vector2(28, 28), new Vector2(4, 4));
        grid.Title.gameObject.AddComponent<Button>().onClick.AddListener([Hotfixable]() =>
        {
            grid.Grid.gameObject.SetActive(!grid.Grid.gameObject.activeSelf);
            LayoutRebuilder.MarkLayoutForRebuild(filter_content_rect);
        });
        grid.Title.gameObject.AddComponent<TipButton>().textOnClick = $"{C.mod_prefix}.ui.filter.expand_or_retract";
        return grid;
    }

    private TitledGrid new_keyword_grid(string keyword_type)
    {
        TitledGrid grid = Instantiate(TitledGrid.Prefab, keyword_content_rect);
        grid.Setup($"{C.mod_prefix}.ui.keyword.{keyword_type}", 180, new Vector2(28, 28), new Vector2(4, 4));
        grid.Title.gameObject.AddComponent<Button>().onClick.AddListener([Hotfixable]() =>
        {
            grid.Grid.gameObject.SetActive(!grid.Grid.gameObject.activeSelf);
            LayoutRebuilder.MarkLayoutForRebuild(keyword_content_rect);
        });
        grid.Title.gameObject.AddComponent<TipButton>().textOnClick = $"{C.mod_prefix}.ui.filter.expand_or_retract";
        return grid;
    }
    private Dictionary<TitledGrid, MonoObjPool<FilterButtonInGrid>> filter_button_in_grid_pool_dict = new();
    
    private FilterButtonInGrid new_filter(TitledGrid grid, string filter_id, string icon_path, Func<Actor, bool> filter_func)
    {
        if (!filter_button_in_grid_pool_dict.ContainsKey(grid))
        {
            filter_button_in_grid_pool_dict[grid] = new MonoObjPool<FilterButtonInGrid>(FilterButtonInGrid.Prefab, grid.Grid.transform);
        }
        var button = filter_button_in_grid_pool_dict[grid].GetNext();
        button.Setup($"{grid.Title.GetComponent<LocalizedText>().key}.{filter_id}", icon_path, filter_func, all_filter_settings);
        return button;
    }

    private void new_keyword(TitledGrid grid, string keyword_id, string icon_path, Func<Actor, Actor, int> compare_func, Func<Actor,string> major_key_disp = null)
    {
        var obj = new GameObject(keyword_id, typeof(Image), typeof(Button), typeof(TipButton));
        obj.transform.SetParent(grid.Grid.GetComponent<RectTransform>());
        obj.transform.localScale = Vector3.one;
        var icon = obj.GetComponent<Image>();
        icon.sprite = SpriteTextureLoader.getSprite(icon_path);
        var tip_button = obj.GetComponent<TipButton>();
        keyword_id = $"{grid.Title.GetComponent<LocalizedText>().key}.{keyword_id}";
        tip_button.textOnClick = keyword_id;
        var button = obj.GetComponent<Button>();
        var local_keyword_id = keyword_id;
        var local_compare = compare_func;
        var local_disp = major_key_disp;
        Sprite local_sprite = icon.sprite;
        button.onClick.AddListener(() =>
        {
            var key_idx = sort_keys.FindIndex(x => x.ID == local_keyword_id);
            if (key_idx == -1)
            {
                var key = new SortKey(local_keyword_id, local_compare, local_sprite, local_disp);
                sort_keys.Add(key);
            }
            else
            {
                sort_keys.RemoveAt(key_idx);
            }

            ui_sort_keys_dirty = true;
            ApplySort();
        });
    }
    [Hotfixable]
    public override void OnNormalEnable()
    {
        CheckDynamicGrid_VANILLA();
        ApplySort();
    }

    [Hotfixable]
    public void ToTop()
    {
        ContentTransform.DOLocalMoveY(0, 1);
    }
    [Hotfixable]
    public void ToBottom()
    {
        ContentTransform.DOLocalMoveY(ContentTransform.GetComponent<RectTransform>().sizeDelta.y - scroll_view_rect.sizeDelta.y, 1);
    }
    [Hotfixable]
    private void CheckJump(string num)
    {
        if (int.TryParse(num, out var idx))
        {
            idx = Math.Min(_list.Count - 1, Math.Max(0, idx));
            ContentTransform.DOLocalMoveY(idx * single_element_height, 1);
        }
    }
    [Hotfixable]
    private void OnUpdate()
    {
        CheckFilters();
        Vector3 curr_position = ContentTransform.localPosition;

        var view_y_start = curr_position.y;
        var view_y_end = curr_position.y + scroll_view_rect.sizeDelta.y;

        var view_start_idx = (int)(view_y_start / single_element_height);
        var view_end_idx = Math.Min((int)(view_y_end / single_element_height) + 1, _list.Count - 1);

        var actual_start_y = start_y - view_start_idx * single_element_height;
        var actual_end_y = start_y   - view_end_idx   * single_element_height;
        foreach (TopElementActor elm in _pool._elements_total)
        {
            GameObject game_object = elm.gameObject;
            if (!game_object.activeSelf) continue;

            var y = game_object.transform.localPosition.y;
            if (y > actual_start_y || y < actual_end_y)
            {
                game_object.SetActive(false);
                _pool._elements_inactive.Push(elm);
            }
        }

        SortKey major_sort_key = null;
        if (sort_keys.Count > 0)
        {
            major_sort_key = sort_keys[0];
        }

        for (var i = view_start_idx; i <= view_end_idx; i++)
            if (i < last_view_start || i > last_view_end)
            {
                // 需要显示
                Actor actor = _list[i];
                TopElementActor comp = _pool.getNext();
                comp.Setup(actor, i, major_sort_key?.GetMajorKeyDisplay(actor) ?? "");
                comp.transform.localPosition = new Vector3(0, start_y - i * single_element_height);
            }

        last_view_start = view_start_idx;
        last_view_end = view_end_idx;

        if (ui_sort_keys_dirty)
        {
            sort_button_pool.Clear();
            foreach (SortKey key in sort_keys)
            {
                SortKeyButton button = sort_button_pool.GetNext();
                button.Setup(key);
            }

            ui_sort_keys_dirty = false;
        }

        if (ui_filters_dirty)
        {
            filter_button_pool.Clear();
            foreach (FilterSetting filter in all_filter_settings)
            {
                FilterButton button = filter_button_pool.GetNext();
                button.Setup(filter);
            }

            ui_filters_dirty = false;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(filter_content_rect);
    }
    [Hotfixable]
    private void CheckFilters()
    {
        bool need_refresh = false;
        if (last_not_filter_settings.Count + last_and_filter_settings.Count + last_or_filter_settings.Count !=
            all_filter_settings.Count)
        {
            ui_filters_dirty = true;
            need_refresh = true;
        }
        need_refresh |= last_not_filter_settings.Any(x => x.Type != FilterType.Not);
        need_refresh |= last_and_filter_settings.Any(x => x.Type != FilterType.And);
        need_refresh |= last_or_filter_settings.Any(x => x.Type != FilterType.Or);
        
        last_not_filter_settings.Clear();
        last_and_filter_settings.Clear();
        last_or_filter_settings.Clear();
        foreach (var setting in all_filter_settings)
        {
            switch (setting.Type)
            {
                case FilterType.Not:
                    last_not_filter_settings.Add(setting);
                    break;
                case FilterType.And:
                    last_and_filter_settings.Add(setting);
                    break;
                case FilterType.Or:
                    last_or_filter_settings.Add(setting);
                    break;
            }
        }

        if (need_refresh)
        {
            ApplySort();
        }
    }

    private class SortKey
    {
        private readonly Func<Actor, Actor, int> _compare;
        public readonly  Sprite                  Icon;
        private readonly Func<Actor, string> _major_key_disp;

        public readonly string ID;

        public SortKey(string id, Func<Actor, Actor, int> compare, Sprite icon, Func<Actor, string> major_key_disp)
        {
            ID = id;
            _compare = compare;
            _major_key_disp = major_key_disp;
            Icon = icon;
        }

        public bool IsAscend { get; private set; } = true;

        public int Compare(Actor a, Actor b)
        {
            return _compare(a, b) * (IsAscend ? 1 : -1);
        }
        public string GetMajorKeyDisplay(Actor actor)
        {
            return _major_key_disp?.Invoke(actor) ?? "";
        }

        public void Switch()
        {
            IsAscend = !IsAscend;
        }
    }

    private class SortKeyButton : APrefab<SortKeyButton>
    {
        public void Setup(SortKey key)
        {
            GetComponent<Image>().sprite = key.Icon;
            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(() =>
            {
                key.Switch();
                Instance.ApplySort();
                Instance.ui_sort_keys_dirty = true;
            });
            GetComponent<TipButton>().textOnClick = key.ID;
            GetComponent<TipButton>().textOnClickDescription = key.IsAscend
                ? $"{C.mod_prefix}.ui.sort.ascending"
                : $"{C.mod_prefix}.ui.sort.descending";
            transform.Find("Arrow").GetComponent<Image>().sprite =
                SpriteTextureLoader.getSprite(key.IsAscend ? "ui/icons/iconArrowDOWN" : "ui/icons/iconArrowUP");
        }

        private static void _init()
        {
            var obj = new GameObject(nameof(SortKeyButton), typeof(Image), typeof(Button), typeof(TipButton),
                typeof(HorizontalLayoutGroup));
            obj.transform.SetParent(Main.prefabs);

            var arrow = new GameObject("Arrow", typeof(Image));
            arrow.transform.SetParent(obj.transform);

            Prefab = obj.AddComponent<SortKeyButton>();
        }
    }
}