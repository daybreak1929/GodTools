using System;
using GodTools.Features;
using GodTools.UI.Prefabs;
using NeoModLoader.General;
using NeoModLoader.General.UI.Prefabs;
using NeoModLoader.General.UI.Window;
using NeoModLoader.General.UI.Window.Layout;
using NeoModLoader.General.UI.Window.Utils.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.UI;

public class WindowSubWorldCreator : AutoLayoutWindow<WindowSubWorldCreator>
{
    private readonly int    _size = 1;
    public static    string WINDOW_ID { get; private set; }

    protected override void Init()
    {
        WINDOW_ID = WindowID;

        AutoHoriLayoutGroup world_size_group = this.BeginHoriGroup(pAlignment: TextAnchor.MiddleCenter, pSpacing: 80);
        {
            RawLocalizedText comment_label = Instantiate(RawLocalizedText.Prefab, world_size_group.transform);
            comment_label.Setup($"{C.mod_prefix}.ui.sub_world_size", alignment: TextAnchor.MiddleLeft);
            comment_label.SetSize(new Vector2(50, 20));
            RawText world_size_label = Instantiate(RawText.Prefab, world_size_group.transform);
            world_size_label.Setup("1x1", alignment: TextAnchor.MiddleRight);
            world_size_label.SetSize(new Vector2(50, 20));
            world_size_label.gameObject.AddComponent<Button>();
            world_size_label.gameObject.AddComponent<TipButton>().textOnClick = $"{C.mod_prefix}.ui.sub_world_size_tip";
        }
        SimpleButton create_button = Instantiate(SimpleButton.Prefab, ContentTransform);
        create_button.Setup(() =>
        {
            ScrollWindowComponent.clickHide();
            var guid = Guid.NewGuid().ToString();
            Worlds.SubWorlds.Add(guid, SubWorld.Create(guid, _size));
        }, null, LM.Get($"{C.mod_prefix}.ui.create_sub_world"), new Vector2(100, 20));
    }
}