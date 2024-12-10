#if 启源_启源核心
using System;
using System.Collections.Generic;
using Cultivation_Way.Constants;
using Cultivation_Way.Core;
using Cultivation_Way.UI.prefabs;
using NeoModLoader.General;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.UI.Prefabs;

public class ElementEditor : APrefab<ElementEditor>
{
    private const int element_type_nr = Core.element_type_nr;

    private readonly List<Action<CW_Element>> actions_on_end = new();
    private          int[]                    _elements;
    private          SliderBar[]              _sliders;
    private          SimpleText               _title;

    protected override void Init()
    {
        if (Initialized) return;

        _elements = new int[element_type_nr];

        _sliders = GetComponentsInChildren<SliderBar>();
        for (var i = 0; i < _elements.Length; i++)
        {
            _elements[i] = 100;
            var idx = i;
            _sliders[i].Setup(_elements[i], 0, 100, value => { UpdateElementWeight(idx, value); });
        }

        _title = transform.Find("Title").GetComponent<SimpleText>();

        base.Init();
    }

    public void Setup(Action<CW_Element> pActionOnEnd = null)
    {
        Init();

        actions_on_end.Clear();
        AddEndEditAction(pActionOnEnd);
    }

    private void UpdateElementWeight(int idx, float value)
    {
        _elements[idx] = (int)value;
        OnEndEdit();
    }

    private void OnEndEdit()
    {
        _title.text.text = LM.Get(GetResult().GetElementType().id);
        foreach (var action in actions_on_end) action.Invoke(GetResult());
    }

    public void AddEndEditAction(Action<CW_Element> action)
    {
        actions_on_end.Add(action);
    }

    public CW_Element GetResult()
    {
        return new CW_Element(_elements, true);
    }

    private static void _init()
    {
        var obj = new GameObject(nameof(ElementEditor), typeof(RectTransform), typeof(VerticalLayoutGroup),
            typeof(ContentSizeFitter));
        obj.transform.SetParent(Main.prefabs);

        var layout = obj.GetComponent<VerticalLayoutGroup>();
        OT.InitializeNoActionVerticalLayoutGroup(layout);

        var fitter = obj.GetComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        SimpleText title = Instantiate(SimpleText.Prefab, obj.transform);
        title.Setup("Empty", TextAnchor.MiddleCenter, new Vector2(200, 20));
        title.name = "Title";
        for (var i = 0; i < element_type_nr; i++)
        {
            SliderBar slider = Instantiate(SliderBar.Prefab, obj.transform);
            slider.name = Core.element_str[i];
        }

        Prefab = obj.AddComponent<ElementEditor>();
    }
}
#endif