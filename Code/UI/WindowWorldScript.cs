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
    }
}