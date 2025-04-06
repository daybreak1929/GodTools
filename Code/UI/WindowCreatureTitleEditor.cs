using System;
using GodTools.Abstract;
using GodTools.UI.Prefabs;
using GodTools.Utils;
using NeoModLoader.api;
using NeoModLoader.api.attributes;
using NeoModLoader.General.UI.Prefabs;
using NeoModLoader.General.UI.Window;
using NeoModLoader.General.UI.Window.Utils.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.UI;

public class WindowCreatureTitleEditor : SingleAutoLayoutWindow<WindowCreatureTitleEditor>
{
    protected override void Init()
    {
        var titles_group = this.BeginVertGroup();
        titles_group.GetComponent<RectTransform>().pivot = new(0.5f, 1);
        _pool = new MonoObjPool<CreatureTitleItem>(CreatureTitleItem.Prefab, titles_group.transform,
            active_action: [Hotfixable](x) =>
            {
                x.SetSize(new(190, 48));
            });
        var new_button = SimpleButton.Instantiate(ContentTransform, pName: "NewTitle");
        new_button.Setup([Hotfixable]() =>
        {
            var item = _pool.GetNext();
            var title_uid = $"GodTools.Title.{Guid.NewGuid().ToString()}";
            SelectedUnit.unit.AddTitleGuid(title_uid);
            item.Setup(SelectedUnit.unit, title_uid, obj =>
            {
                _pool.Return(obj.GetComponent<CreatureTitleItem>());
                UpdateLayout();
            });
            UpdateLayout();
        }, SpriteTextureLoader.getSprite("gt_windows/plus"), pSize: new(190, 24));
        
        _title_group = titles_group.GetComponent<RectTransform>();
        _new_button = new_button.GetComponent<RectTransform>();
    }

    private RectTransform _title_group;
    private RectTransform _new_button;
    [Hotfixable]
    private void UpdateLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(ContentTransform.GetComponent<RectTransform>());
        //_title_group.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
        //ContentTransform.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
        //_new_button.localPosition = new(0, -_title_group.sizeDelta.y-_new_button.sizeDelta.y-5);
    }
    private MonoObjPool<CreatureTitleItem> _pool;
    public override void OnNormalEnable()
    {
        var actor = SelectedUnit.unit;
        if (actor == null) return;
        _pool.Clear();
        var title_guids = actor.GetTitleGuids();
        foreach (var title_guid in title_guids)
        {
            var item = _pool.GetNext();
            item.Setup(actor, title_guid, obj =>
            {
                _pool.Return(obj.GetComponent<CreatureTitleItem>());
            });
        }
    }
}