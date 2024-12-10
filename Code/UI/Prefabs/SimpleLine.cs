using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.UI.Prefabs;

public class SimpleLine : APrefab<SimpleLine>
{
    private static void _init()
    {
        GameObject obj = new(nameof(SimpleLine), typeof(Image));
        obj.transform.SetParent(Main.prefabs);

        obj.GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("ui/special/ui_line_strong");
        obj.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 100);

        Prefab = obj.AddComponent<SimpleLine>();
    }
}