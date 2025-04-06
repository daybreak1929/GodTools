using System.Collections.Generic;
using GodTools.Utils;
using NeoModLoader.api.attributes;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.UI.Prefabs;

public class CreatureTitleDisplay : APrefab<CreatureTitleDisplay>
{
    public Text Text => text;
    [SerializeField]
    private Text text;
    [Hotfixable]
    public void Setup(Actor actor, IEnumerable<string> titles, RectTransform canvas_rect)
    {
        text.text = string.Join("\n", titles);

        transform.localPosition = UITools.WorldToScreenPosition(actor.cur_transform_position + new Vector3(0, actor.stats[S.scale] * actor.getSpriteToRender().rect.height), canvas_rect);
        transform.localScale = Vector3.one / World.world.camera.orthographicSize * 8;
    }
    private static void _init()
    {
        var obj = new GameObject(nameof(CreatureTitleDisplay), typeof(Text), typeof(ContentSizeFitter));
        obj.transform.SetParent(Main.prefabs);

        obj.GetComponent<RectTransform>().pivot = new(0.5f, 0);
        
        var text = obj.GetComponent<Text>();
        text.font = LocalizedTextManager.current_font;
        text.color = Color.white;
        text.alignment = TextAnchor.UpperCenter;
        text.fontSize = 8;
        
        var fitter = obj.GetComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        Prefab = obj.AddComponent<CreatureTitleDisplay>();
        Prefab.text = text;
    }
}