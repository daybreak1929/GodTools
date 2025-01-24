using GodTools.Abstract;
using GodTools.Features;
using GodTools.Features.WorldScript;
using GodTools.UI.Prefabs;
using GodTools.Utils;
using NeoModLoader.api;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.UI;

public class WindowWorldScript : AbstractWideWindow<WindowWorldScript>
{
    public RectTransform allScriptContent;
    public RectTransform scriptConfigureContent;
    public RectTransform detailConfigureContent;
    protected override void Init()
    {
        allScriptContent = ContentTransform.GetComponent<RectTransform>();
        allScriptContent.pivot = new(0.5f, 1);
        allScriptContent.sizeDelta = new(0, 0);
        allScriptContent.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.MinSize;
        allScriptContent.gameObject.AddComponent<VerticalLayoutGroup>().SetSimpleVertLayout();

        var all_script_scroll_view = allScriptContent.parent.parent;
        var script_configure_scroll_view = Instantiate(all_script_scroll_view, BackgroundTransform);
        script_configure_scroll_view.name = "ScriptConfigure";
        var detail_configure_scroll_view = Instantiate(all_script_scroll_view, BackgroundTransform);
        detail_configure_scroll_view.name = "DetailConfigure";

        all_script_scroll_view.transform.localPosition = new(-220, 0);
        all_script_scroll_view.GetComponent<RectTransform>().sizeDelta = new(120, 250);

        script_configure_scroll_view.transform.localPosition = new(-70, 0);
        script_configure_scroll_view.GetComponent<RectTransform>().sizeDelta = new(160, 250);

        detail_configure_scroll_view.transform.localPosition = new(150, 0);
        detail_configure_scroll_view.GetComponent<RectTransform>().sizeDelta = new(260, 250);
        
        SimpleLine line1 = Instantiate(SimpleLine.Prefab, BackgroundTransform);
        line1.transform.localPosition = new Vector3(-155, 0);
        line1.SetSize(new Vector2(2, 250));
        SimpleLine line2 = Instantiate(SimpleLine.Prefab, BackgroundTransform);
        line2.transform.localPosition = new Vector3(15, 0);
        line2.SetSize(new Vector2(2, 250));
        
        scriptConfigureContent = script_configure_scroll_view.Find("Viewport/Content").GetComponent<RectTransform>();
        detailConfigureContent = detail_configure_scroll_view.Find("Viewport/Content").GetComponent<RectTransform>();
        
        SetupScriptConfiguration();


        allScriptContent.GetComponent<VerticalLayoutGroup>().childControlWidth = true;
        
        _allScriptPool = new MonoObjPool<WorldScriptItem>(WorldScriptItem.Prefab, allScriptContent);
        var add_scrip_button = TextButton.Instantiate(allScriptContent, pName: "AddScript");
        add_scrip_button.Setup($"{C.mod_prefix}.ui.world_script.add_script", () =>
        {
            var script = WorldScripts.NewScript();
            var item = _allScriptPool.GetNext(-2);
            item.Setup(script);
        });
        
        scriptConfigureContent.gameObject.SetActive(false);
        detailConfigureContent.gameObject.SetActive(false);
    }
    private MonoObjPool<WorldScriptItem> _allScriptPool;
    public override void OnFirstEnable()
    {
        base.OnFirstEnable();
        foreach (var script in WorldScripts.Scripts)
        {
            var item = _allScriptPool.GetNext(-2);
            item.Setup(script);
        }
    }
    private WorldScriptItem _selectedScript;
    public void Select(WorldScriptItem script)
    {
        _selectedScript = script;
        foreach (var item in _allScriptPool.ActiveObjs)
        {
            item.name.GetComponent<Button>().interactable = true;
        }
        script.name.GetComponent<Button>().interactable = false;
        scriptConfigureContent.gameObject.SetActive(true);
        UpdateScriptConfiguration();
    }

    public void UpdateDetailPage(string page)
    {
        if (string.IsNullOrEmpty(page))
        {
            detailConfigureContent.gameObject.SetActive(false);
        }
    }

    public void UnSelect(WorldScriptItem script_to_confirm = null)
    {
        if (script_to_confirm != null && _selectedScript != script_to_confirm)
        {
            return;
        }
        _selectedScript = null;
        foreach (var item in _allScriptPool.ActiveObjs)
        {
            item.name.GetComponent<Button>().interactable = true;
        }
        scriptConfigureContent.gameObject.SetActive(false);
    }
    public void Delete(WorldScriptItem script)
    {
        if (_selectedScript == script)
        {
            UnSelect(script);
        }
        WorldScripts.Scripts.Remove(script.script);
        _allScriptPool.Return(script);
    }
    private TextInput _nameEditor;
    private void SetupScriptConfiguration()
    {
        _nameEditor = TextInput.Instantiate(scriptConfigureContent, pName: "NameEditor");
        _nameEditor.Setup("", value => _selectedScript.Rename(value));
        _nameEditor.SetSize(new(150, 20));
        _nameEditor.GetComponent<RectTransform>().pivot = new(0.5f, 1);
        _nameEditor.text.alignment = TextAnchor.MiddleCenter;
    }
    private void UpdateScriptConfiguration()
    {
        _nameEditor.input.SetTextWithoutNotify(_selectedScript.name.Value);
    }
}