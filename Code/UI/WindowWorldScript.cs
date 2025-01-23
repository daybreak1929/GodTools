using GodTools.Utils;
using NeoModLoader.api;
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
        
        
        scriptConfigureContent = script_configure_scroll_view.Find("Viewport/Content").GetComponent<RectTransform>();
        detailConfigureContent = detail_configure_scroll_view.Find("Viewport/Content").GetComponent<RectTransform>();
        
        
    }
}