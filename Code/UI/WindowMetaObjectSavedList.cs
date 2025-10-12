using System;
using System.Collections.Generic;
using System.Reflection;
using GodTools.Features;
using GodTools.UI.Prefabs;
using NeoModLoader.api;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;

namespace GodTools.UI;

public class WindowMetaObjectSavedList<TWindow, TCard, TList, TObject, TData> : AbstractWindow<TWindow> 
    where TData : MetaObjectData, new()
    where TObject : MetaObject<TData>, new()
    where TList : MetaObjectSavedList<TList, TObject, TData>
    where TCard : SavedMetaObjectCard<TCard, TObject, TData>
    where TWindow : WindowMetaObjectSavedList<TWindow, TCard, TList, TObject, TData>
{
    private const float single_element_height = 56;
    private const float start_y               = -single_element_height / 2;

    private static TData _selected_data;

    private TList _t_list;
    private List<TData>                           _list;
    private ObjectPoolGenericMono<TCard> _pool;
    private PowerButton                               _power_button;

    private RectTransform                 content_rect;
    private RectTransform                 scroll_view_rect;
    protected override void Init()
    {
        content_rect = ContentTransform.GetComponent<RectTransform>();
        scroll_view_rect = content_rect.parent.parent.GetComponent<RectTransform>();
        content_rect.pivot = new Vector2(0.5f, 1);
        _pool = new ObjectPoolGenericMono<TCard>(
            typeof(TCard).GetProperty("Prefab", BindingFlags.Static | BindingFlags.Public)?.GetMethod
                .Invoke(null, []) as TCard, ContentTransform);
        
        _t_list = typeof(TList).GetProperty("Instance", BindingFlags.Static | BindingFlags.Public)?.GetMethod.Invoke(null, []) as TList;
    }

    private void Update()
    {
        OnUpdate();
    }

    private void OnUpdate()
    {
        content_rect.sizeDelta = new Vector2(0, _list.Count * single_element_height);
        Vector3 curr_position = ContentTransform.localPosition;

        var view_y_start = curr_position.y;
        var view_y_end = curr_position.y + scroll_view_rect.sizeDelta.y;

        var view_start_idx = (int)(view_y_start / single_element_height);
        var view_end_idx = Math.Min((int)(view_y_end / single_element_height) + 1, _list.Count - 1);

        _pool.clear();
        for (var i = view_start_idx; i <= view_end_idx; i++)
        {
            var idx = i;
            var data = _list[idx];
            var card = _pool.getNext();
            card.Setup(data, () =>
            {
                ScrollWindow.hideAllEvent();
                
                //CreatureSavedList.SelectActorToPlace(data);
                _power_button.clickActivePower();
            }, () =>
            {
                _t_list.UnsaveData(data);
                _list.RemoveAt(idx);
                Main.LogInfo("delete");
            });
            card.transform.localPosition = new Vector2(0, start_y - i * single_element_height);
        }
    }

    public override void OnNormalEnable()
    {
        _list = _t_list.GetAll();
    }
}