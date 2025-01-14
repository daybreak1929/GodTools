using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using DG.Tweening;
using GodTools.Abstract;
using GodTools.Features;
using GodTools.UI.Prefabs;
using HarmonyLib;
using NeoModLoader.api;
using NeoModLoader.api.attributes;
using NeoModLoader.General;
using NeoModLoader.General.UI.Prefabs;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.UI;

public class WindowDetailedHistory : AbstractWideWindow<WindowDetailedHistory>
{
    private RectTransform             content_rect;
    private RectTransform scroll_view_rect;
    private RectTransform filter_content_rect;
    private SingleRowGrid selected_filters;
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

        filter_content_rect = filter_scroll_view.Find("Viewport/Content").GetComponent<RectTransform>();

        filter_content_rect.gameObject.AddComponent<ContentSizeFitter>().verticalFit =
            ContentSizeFitter.FitMode.MinSize;
        set_simple_vert_layout(filter_content_rect.gameObject.AddComponent<VerticalLayoutGroup>());
        
        selected_filters = Instantiate(SingleRowGrid.Prefab,  filter_content_rect);
        selected_filters.Setup($"{C.mod_prefix}.ui.filter", new Vector2(180,   35), new Vector2(28, 28));
        
        void set_simple_vert_layout(VerticalLayoutGroup layout_group)
        {
            layout_group.spacing = 4;
            layout_group.childControlHeight = false;
            layout_group.childControlWidth = false;
            layout_group.childAlignment = TextAnchor.UpperCenter;
            layout_group.childForceExpandHeight = false;
            layout_group.childForceExpandWidth = false;
        }

        _type_filter_grid = new_filter_grid("type");
        _type_filter_pool = new MonoObjPool<SimpleButton>(SimpleButton.Prefab, _type_filter_grid.Grid.transform);
        
        
        _pool = new MonoObjPool<DetailedLogItem>(DetailedLogItem.Prefab, content_rect);
        filter_button_pool = new MonoObjPool<FilterButton>(FilterButton.Prefab, selected_filters.Grid.transform);
        

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
        placeholder_text.Setup($"{C.mod_prefix}.ui.jump_to_year", new Color(1,1,1,0.3f),alignment: TextAnchor.MiddleCenter);
        placeholder_text.SetSize(new(60, 20));
        placeholder_text.transform.localPosition = new(33, 0);
        jump_input.input.placeholder = placeholder_text.Text;
    }
    private MonoObjPool<DetailedLogItem> _pool;
    private MonoObjPool<FilterButton> filter_button_pool;
    
    private List<WorldLogMessage> _list = new();

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
    private const string year_pattern = @"y:(\d+)";
    [Hotfixable]
    private void CheckJump(string num)
    {
        if (int.TryParse(num, out var year))
        {
            var idx = 0;
            for (int i = 0; i < _list.Count; i++)
            {
                Match match = Regex.Match(_list[i].date, year_pattern);

                if (match.Success)
                {
                    string msg_year_str = match.Groups[1].Value;
                    if (!int.TryParse(msg_year_str, out var msg_year)) continue;
                    if (msg_year >= year)
                    {
                        idx = i;
                        break;
                    }
                }
                else
                {
                    continue;
                }
            }
            idx = Math.Min(_list.Count - 1, Math.Max(0, idx));
            ContentTransform.DOLocalMoveY(idx * single_element_height, 1);
        }
    }

    private TitledGrid _type_filter_grid;
    private MonoObjPool<SimpleButton> _type_filter_pool;
    private TitledGrid new_filter_grid(string filter_type)
    {
        TitledGrid grid = Instantiate(TitledGrid.Prefab, filter_content_rect);
        var group_id = $"{C.mod_prefix}.ui.filter.worldlog.{filter_type}";
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

            if (filters.Any(x => x.group_id == group_id)) AllUpdate();
        });
        grid.Title.gameObject.AddComponent<TipButton>().textOnClick = $"{C.mod_prefix}.ui.switch_bw_list";
        return grid;
    }

    private class Filter
    {
        private readonly Func<WorldLogMessage, bool> _filter;
        public readonly  string            group_id;

        public readonly Sprite Icon;
        public readonly string ID;

        public Filter(string id, string group_id, Func<WorldLogMessage, bool> filter, Sprite icon)
        {
            ID = id;
            _filter = filter;
            Icon = icon;
            this.group_id = group_id;
        }

        public bool BlackList => Instance.blacklist_filter_groups.Contains(group_id);

        public bool FilterMsg(WorldLogMessage msg)
        {
            return _filter(msg) != BlackList;
        }
    }
    private readonly HashSet<string> blacklist_filter_groups = new();
    private readonly List<Filter>    filters                 = new();
    private void Update()
    {
        if (Initialized) OnUpdate();
    }

    public override void OnNormalEnable()
    {
        RegisterIconFilters();
        
        AllUpdate();
    }
    [Hotfixable]
    public void AllUpdate()
    {
        _list.Clear();
        if (filters.Count == 0)
        {
            _list.AddRange(DetailedHistory.Messages);
        }
        else
            _list.AddRange(DetailedHistory.Messages.Where(msg => filters.All(x => x.FilterMsg(msg))));
        
        content_rect.sizeDelta = new Vector2(0, (1+_list.Count) * single_element_height);
        last_view_start = 9999;
        last_view_end = -1;
        _pool.Clear();
        ui_filters_dirty = true;
    }
    [Hotfixable]
    private void RegisterIconFilters()
    {
        _type_filter_pool.Clear();
        var icon_set = new HashSet<string>();
        foreach (var msg in DetailedHistory.Messages)
        {
            if (!icon_set.Add(msg.icon)) continue;
            var icon = msg.icon;
            var group_id = _type_filter_grid.Title.GetComponent<LocalizedText>().key;
            var icon_filter = new Filter($"{group_id}.{icon}", group_id, msg => msg.icon == icon, SpriteTextureLoader.getSprite($"ui/icons/{icon}"));
            var button = _type_filter_pool.GetNext();
            var tip_name = LM.Has(icon_filter.ID) ? icon_filter.ID : LM.Has(msg.text) ? msg.text : "";
            button.Setup([Hotfixable]() =>
            {
                var filter_idx = filters.FindIndex(x => x.ID == icon_filter.ID);
                if (filter_idx == -1)
                {
                    if (!blacklist_filter_groups.Contains(group_id) && filters.Any(x => x.group_id == group_id))
                    {
                        return;
                    }
                    filters.Add(icon_filter);
                }
                else
                {
                    filters.RemoveAt(filter_idx);
                }
                AllUpdate();
            }, icon_filter.Icon, null, new(28,28), pTipType: "tip", new TooltipData()
            {
                tip_name = tip_name
            });
            button.Background.enabled = false;
            if (string.IsNullOrEmpty(tip_name))
            {
                button.TipButton.enabled = false;
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(filter_content_rect);
    }

    private const float single_element_height = 24;
    private const float start_y               = -single_element_height / 2;
    private int           last_view_end = -1;

    private int           last_view_start = 9999;
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
        
        /*
        _pool.Clear();
        for (int i = view_start_idx; i <= view_end_idx; i++)
        {
            var msg = _messages[i];
            var obj = _pool.GetNext();
            obj.Text.text = msg.getFormatedText(obj.Text, true, true);
            obj.transform.localPosition = new Vector3(0, start_y - i * single_element_height);
        }
*/
        
        _pool.MarkUnused(obj =>
        {
            var y = obj.transform.localPosition.y;
            return y > actual_start_y || y < actual_end_y;
        });

        for (var i = view_start_idx; i <= view_end_idx; i++)
            if ((i < last_view_start || i > last_view_end) && (i>=0 && i<_list.Count))
            {
                // 需要显示
                var msg = _list[i];
                var obj = _pool.GetNext();
                obj.Setup(msg, InspectMsg);
                obj.transform.localPosition = new Vector3(0, start_y - i * single_element_height);
            }

        last_view_start = view_start_idx;
        last_view_end = view_end_idx;
        
        
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

    private bool ui_filters_dirty;

    private void InspectMsg(WorldLogMessage msg)
    {
        Main.LogInfo($"Inspect {msg.date}");
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