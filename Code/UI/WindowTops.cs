using System;
using System.Collections.Generic;
using System.Linq;
using GodTools.Abstract;
using GodTools.UI.Prefabs;
using GodTools.UI.Prefabs.TopElementPrefabs;
using NeoModLoader.api;
using NeoModLoader.api.attributes;
using NeoModLoader.General;
using NeoModLoader.General.Game.extensions;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.UI;

public class WindowTops : AbstractWideWindow<WindowTops>
{
    private const float single_element_height = 40;
    private const float start_y               = -single_element_height / 2;

    private readonly HashSet<string> blacklist_filter_groups = new();
    private readonly List<Filter>    filters                 = new();

    private readonly List<SortKey>                     sort_keys = new();
    private          List<Actor>                       _list;
    private          ObjectPoolGenericMono<ActorLevel> _pool;
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
    private bool ui_filters_dirty = true;

    private bool ui_sort_keys_dirty = true;

    private void Update()
    {
        if (Initialized) OnUpdate();
    }

    protected override void Init()
    {
        content_rect = ContentTransform.GetComponent<RectTransform>();
        scroll_view_rect = content_rect.parent.parent.GetComponent<RectTransform>();
        scroll_view_rect.localPosition = Vector3.zero;
        scroll_view_rect.sizeDelta = new Vector2(200, 250);
        content_rect.pivot = new Vector2(0.5f, 1);


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
        set_simple_vert_layout(filter_content_rect.gameObject.AddComponent<VerticalLayoutGroup>());
        set_simple_vert_layout(keyword_content_rect.gameObject.AddComponent<VerticalLayoutGroup>());

        void set_simple_vert_layout(VerticalLayoutGroup layout_group)
        {
            layout_group.spacing = 4;
            layout_group.childControlHeight = false;
            layout_group.childControlWidth = false;
            layout_group.childAlignment = TextAnchor.UpperCenter;
            layout_group.childForceExpandHeight = false;
            layout_group.childForceExpandWidth = false;
        }


        selected_filters = Instantiate(SingleRowGrid.Prefab,  filter_content_rect);
        selected_keywords = Instantiate(SingleRowGrid.Prefab, keyword_content_rect);

        selected_filters.Setup($"{C.mod_prefix}.ui.filter", new Vector2(180,   35), new Vector2(28, 28));
        selected_keywords.Setup($"{C.mod_prefix}.ui.keyword", new Vector2(180, 35), new Vector2(28, 28));

        TitledGrid race_filter_grid = new_filter_grid("race");
        AssetManager.raceLibrary.ForEach<Race, RaceLibrary>(race_asset =>
        {
            if (race_asset.nature) return;
            var local_race_id = race_asset.id;
            try
            {
                LM.AddToCurrentLocale($"{C.mod_prefix}.ui.filter.race.{race_asset.nameLocale}",
                    LM.Get(race_asset.nameLocale));
                new_filter(race_filter_grid, race_asset.nameLocale, race_asset.path_icon, a => a.isRace(local_race_id));
            }
            catch (Exception)
            {
                // ignored
            }
        });

        TitledGrid vanilla_keyword_grid = new_keyword_grid("vanilla");
        new_keyword(vanilla_keyword_grid, "level", "ui/icons/iconLevels", (a, b) =>
        {
            var res = a.data.level.CompareTo(b.data.level);
            if (res == 0) res = a.data.experience.CompareTo(b.data.experience);

            return res;
        });
        new_keyword(vanilla_keyword_grid, "kills", "ui/icons/iconSkulls",
            (a, b) => a.data.kills.CompareTo(b.data.kills));
        new_keyword(vanilla_keyword_grid, "birth", "ui/icons/iconAge",
            (a, b) => a.data.getAge().CompareTo(b.data.getAge()));

#if INMNY_CUSTOMMODT001
        TitledGrid inmny_custommodt001_keyword_grid = new_keyword_grid("inmny_custommodt001");
        new_keyword(inmny_custommodt001_keyword_grid, "talent", "inmny/custommodt001/talent", (a, b) =>
        {
            a.data.get("inmny.custommodt001.talent", out float a_talent);
            b.data.get("inmny.custommodt001.talent", out float b_talent);
            return a_talent.CompareTo(b_talent);
        });
        new_keyword(inmny_custommodt001_keyword_grid, "level", "inmny/custommodt001/cultilevel", (a, b) =>
        {
            a.data.get("inmny.custommodt001.wudao_level", out int a_level);
            b.data.get("inmny.custommodt001.wudao_level", out int b_level);
            var res = a_level.CompareTo(b_level);
            if (res == 0)
            {
                a.data.get("inmny.custommodt001.wudao_exp", out int a_exp);
                b.data.get("inmny.custommodt001.wudao_exp", out int b_exp);
                res = a_exp.CompareTo(b_exp);
            }

            return res;
        });
#endif
        TitledGrid new_filter_grid(string filter_type)
        {
            TitledGrid grid = Instantiate(TitledGrid.Prefab, filter_content_rect);
            var group_id = $"{C.mod_prefix}.ui.filter.{filter_type}";
            grid.Setup(group_id, 180, new Vector2(28, 28), new Vector2(4, 4));
            grid.Title.color = Color.white;
            var bw_switch = grid.Title.gameObject.AddComponent<Button>();
            bw_switch.onClick.AddListener(() =>
            {
                if (!blacklist_filter_groups.Add(group_id))
                {
                    blacklist_filter_groups.Remove(group_id);
                    grid.Title.color = Color.white;
                }
                else
                {
                    grid.Title.color = Color.black;
                }

                if (filters.Any(x => x.group_id == group_id)) ApplyFilter();
            });
            grid.Title.gameObject.AddComponent<TipButton>().textOnClick = $"{C.mod_prefix}.ui.switch_bw_list";
            return grid;
        }

        TitledGrid new_keyword_grid(string keyword_type)
        {
            TitledGrid grid = Instantiate(TitledGrid.Prefab, keyword_content_rect);
            grid.Setup($"{C.mod_prefix}.ui.keyword.{keyword_type}", 180, new Vector2(28, 28), new Vector2(4, 4));
            return grid;
        }

        void new_filter(TitledGrid grid, string filter_id, string icon_path, Func<Actor, bool> filter_func)
        {
            var obj = new GameObject(filter_id, typeof(Image), typeof(Button), typeof(TipButton));
            obj.transform.SetParent(grid.Grid.GetComponent<RectTransform>());
            obj.transform.localScale = Vector3.one;
            var icon = obj.GetComponent<Image>();
            icon.sprite = SpriteTextureLoader.getSprite(icon_path);
            var tip_button = obj.GetComponent<TipButton>();
            filter_id = $"{grid.Title.GetComponent<LocalizedText>().key}.{filter_id}";
            tip_button.textOnClick = filter_id;
            var button = obj.GetComponent<Button>();
            var local_filter = filter_func;
            var local_filter_id = filter_id;
            Sprite local_sprite = icon.sprite;
            var local_group_id = grid.Title.GetComponent<LocalizedText>().key;
            button.onClick.AddListener(() =>
            {
                var filter_idx = filters.FindIndex(x => x.ID == local_filter_id);
                if (filter_idx == -1)
                {
                    var filter = new Filter(local_filter_id, local_group_id, local_filter, local_sprite);
                    filters.Add(filter);
                }
                else
                {
                    filters.RemoveAt(filter_idx);
                }

                ui_filters_dirty = true;
                ApplyFilter();
            });
        }

        void new_keyword(TitledGrid grid, string keyword_id, string icon_path, Func<Actor, Actor, int> compare_func)
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
            Sprite local_sprite = icon.sprite;
            button.onClick.AddListener(() =>
            {
                var key_idx = sort_keys.FindIndex(x => x.ID == local_keyword_id);
                if (key_idx == -1)
                {
                    var key = new SortKey(local_keyword_id, local_compare, local_sprite);
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

        _pool = new ObjectPoolGenericMono<ActorLevel>(ActorLevel.Prefab, ContentTransform);
        sort_button_pool = new MonoObjPool<SortKeyButton>(SortKeyButton.Prefab, selected_keywords.Grid.transform);
        filter_button_pool = new MonoObjPool<FilterButton>(FilterButton.Prefab, selected_filters.Grid.transform);
    }

    [Hotfixable]
    public void ApplySort()
    {
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

    public void ApplyFilter()
    {
        _list = World.world.units.getSimpleList();
        foreach (Filter filter in filters) _list = _list.FindAll(filter.FilterActor);

        content_rect.sizeDelta = new Vector2(0, _list.Count * single_element_height + 15);
        ApplySort();
    }

    [Hotfixable]
    public override void OnNormalEnable()
    {
        _list = World.world.units.getSimpleList();
        ApplyFilter();
    }

    [Hotfixable]
    private void OnUpdate()
    {
        Vector3 curr_position = ContentTransform.localPosition;

        var view_y_start = curr_position.y;
        var view_y_end = curr_position.y + scroll_view_rect.sizeDelta.y;

        var view_start_idx = (int)(view_y_start / single_element_height);
        var view_end_idx = Math.Min((int)(view_y_end / single_element_height) + 1, _list.Count - 1);

        var actual_start_y = start_y - view_start_idx * single_element_height;
        var actual_end_y = start_y   - view_end_idx   * single_element_height;
        foreach (ActorLevel elm in _pool._elements_total)
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

        for (var i = view_start_idx; i <= view_end_idx; i++)
            if (i < last_view_start || i > last_view_end)
            {
                // 需要显示
                Actor actor = _list[i];
                ActorLevel comp = _pool.getNext();
                comp.Setup(actor, i);
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
            foreach (Filter filter in filters)
            {
                FilterButton button = filter_button_pool.GetNext();
                button.Setup(filter);
            }

            ui_filters_dirty = false;
        }
    }

    private class SortKey
    {
        private readonly Func<Actor, Actor, int> _compare;
        public readonly  Sprite                  Icon;

        public readonly string ID;

        public SortKey(string id, Func<Actor, Actor, int> compare, Sprite icon)
        {
            ID = id;
            _compare = compare;
            Icon = icon;
        }

        public bool IsAscend { get; private set; } = true;

        public int Compare(Actor a, Actor b)
        {
            return _compare(a, b) * (IsAscend ? 1 : -1);
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

    private class Filter
    {
        private readonly Func<Actor, bool> _filter;
        public readonly  string            group_id;

        public readonly Sprite Icon;
        public readonly string ID;

        public Filter(string id, string group_id, Func<Actor, bool> filter, Sprite icon)
        {
            ID = id;
            _filter = filter;
            Icon = icon;
            this.group_id = group_id;
        }

        public bool BlackList => Instance.blacklist_filter_groups.Contains(group_id);

        public bool FilterActor(Actor actor)
        {
            return _filter(actor) != BlackList;
        }
    }

    private class FilterButton : APrefab<FilterButton>
    {
        public void Setup(Filter filter)
        {
            GetComponent<Image>().sprite = filter.Icon;
            GetComponent<TipButton>().textOnClick = filter.ID;
            GetComponent<TipButton>().textOnClickDescription = filter.BlackList
                ? $"{C.mod_prefix}.ui.filter.blacklist"
                : $"{C.mod_prefix}.ui.filter.whitelist";
        }

        private static void _init()
        {
            var obj = new GameObject(nameof(FilterButton), typeof(Image), typeof(Button), typeof(TipButton));
            obj.transform.SetParent(Main.prefabs);
            Prefab = obj.AddComponent<FilterButton>();
        }
    }
}