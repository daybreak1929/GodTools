using System.Collections.Generic;
using GodTools.Abstract;
using GodTools.UI.CreatureDataEditorGrids;
using GodTools.UI.Prefabs;
using GodTools.Utils;
using NeoModLoader.api;
using NeoModLoader.General.UI.Prefabs;
using NeoModLoader.General.UI.Window;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GodTools.UI;

public partial class WindowCreatureDataEditor : AbstractWideWindow<WindowCreatureDataEditor>
{
    private Actor _selected_actor;
    public RectTransform contentRect;
    public RectTransform contentScrollRect;
    public RectTransform pageListContent;
    protected override void Init()
    {
        contentRect = ContentTransform.GetComponent<RectTransform>();
        contentRect.pivot = new(0.5f, 1);
        contentRect.sizeDelta = new(0, 0);
        contentRect.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.MinSize;
        contentRect.gameObject.AddComponent<VerticalLayoutGroup>().SetSimpleVertLayout();
        contentRect.GetComponent<VerticalLayoutGroup>().childControlWidth = true;
        
        contentScrollRect = ContentTransform.parent.parent.GetComponent<RectTransform>();
        contentScrollRect.localPosition = new(38.8f, 0);
        contentScrollRect.sizeDelta = new(200, 215);
        
        
        SimpleLine line = Instantiate(SimpleLine.Prefab, BackgroundTransform);
        line.transform.localPosition = new Vector3(-64, 0);
        line.SetSize(new Vector2(2, 215));
        
        
        RectTransform page_list_scroll_view = Instantiate(contentScrollRect, BackgroundTransform);
        page_list_scroll_view.localPosition = new Vector3(-110, 0);
        page_list_scroll_view.sizeDelta = new(70, 215);
        page_list_scroll_view.name = "Page List Scroll View";

        pageListContent = page_list_scroll_view.Find("Viewport/Content").GetComponent<RectTransform>();
#if 一米_中文名
        RegisterGrid<FamilyNameEditor>("chinese_name");
#endif
        #if INMNY_CUSTOMMODT001
        RegisterGrid<WudaoTalentEditor>("inmny_custommodt001");
        RegisterGrid<WudaoLevelEditor>("inmny_custommodt001");
        RegisterGrid<WudaoExpEditor>("inmny_custommodt001");
        #endif
        #if CULTIWAY
        RegisterGrid<PowerLevelEditor>("cultiway");
        #endif
        #if ZHETIAN
        RegisterGrid<ZheTianTalentEditor>("zhetian");
        RegisterGrid<ZheTianLevelEditor>("zhetian");
        #endif
    }
    private void RegisterGrid<T>(string page_name) where T : CreatureDataEditorGrid
    {
        AddPage(page_name);
        var grid = Instantiate(CreatureDataEditorGrid.GetPrefab<T>(), contentRect);
        grid.gameObject.SetActive(false);
        grid.Setup();
        _pages[page_name].Add(grid);
    }
    private void AddPage(string page_name)
    {
        if (_pages.ContainsKey(page_name)) return;
        _pages.Add(page_name, new List<CreatureDataEditorGrid>());
        var page_entry = Instantiate(TextButton.Prefab, pageListContent);
        page_entry.name = page_name;
        page_entry.Setup($"{C.mod_prefix}.ui.data_editor.{page_name}", () =>
        {
            _current_page = page_name;
            UpdatePage();
        });
    }

    private void UpdatePage()
    {
        foreach (var list in _pages.Values)
        {
            foreach (var grid in list)
            {
                grid.gameObject.SetActive(false);
            }
        }

        foreach (TextButton entry in pageListContent.GetComponentsInChildren<TextButton>())
        {
            entry.Button.interactable = entry.name != _current_page;
        }

        if (string.IsNullOrEmpty(_current_page) ||
            !_pages.TryGetValue(_current_page, out var curr_list)) return;
        foreach (var grid in curr_list)
        {
            grid.gameObject.SetActive(true);
            grid.EnabledWith(_selected_actor);
        }
    }

    private Dictionary<string, List<CreatureDataEditorGrid>> _pages = new();
    private string _current_page;
    public override void OnNormalEnable()
    {
        _selected_actor = Config.selectedUnit;
        UpdatePage();
    }
}