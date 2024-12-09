using System;
using System.Collections.Generic;
using GodTools.UI.Prefabs.TopElementPrefabs;
using NeoModLoader.api;
using NeoModLoader.api.attributes;
using UnityEngine;

namespace GodTools.UI;

public class WindowTops : AbstractWideWindow<WindowTops>
{
    private const float                             single_element_height = 35;
    private const float                             start_y               = -17.5f;
    private       List<Actor>                       _list;
    private       ObjectPoolGenericMono<ActorLevel> _pool;
    private       RectTransform                     content_rect;
    private       RectTransform                     scroll_view_rect;

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
        _pool = new ObjectPoolGenericMono<ActorLevel>(ActorLevel.Prefab, ContentTransform);
    }

    [Hotfixable]
    public override void OnNormalEnable()
    {
        _pool.clear();
        _list = World.world.units.getSimpleList();
        content_rect.sizeDelta = new Vector2(0, _list.Count * 35 + 15);
    }

    [Hotfixable]
    private void OnUpdate()
    {
        Vector3 curr_position = ContentTransform.localPosition;

        var view_y_start = curr_position.y;
        var view_y_end = curr_position.y + scroll_view_rect.sizeDelta.y;

        var view_start_idx = (int)(view_y_start / single_element_height);
        var view_end_idx = Math.Min((int)(view_y_end / single_element_height) + 1, _list.Count - 1);

        _pool.clear();
        for (var i = view_start_idx; i <= view_end_idx; i++)
        {
            Actor actor = _list[i];
            ActorLevel actor_level = _pool.getNext();
            actor_level.Setup(actor, i);
            actor_level.transform.localPosition = new Vector2(0, start_y - i * single_element_height);
        }
    }
}