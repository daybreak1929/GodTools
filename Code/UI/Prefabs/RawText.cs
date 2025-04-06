using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GodTools.UI.Prefabs;

public class RawText : APrefab<RawText>
{
    [SerializeField]
    private Text text;

    public Text Text => text;
    public RectTransform RectTransform => text.rectTransform;

    public string Value
    {
        get => text.text;
        set => text.text = value;
    }
    public Color Color
    {
        get => text.color;
        set => text.color = value;
    }
    public TextAnchor Alignment
    {
        get => text.alignment;
        set => text.alignment = value;
    }
    

    public void Setup(string text, Color color = default, TextAnchor alignment = TextAnchor.UpperLeft)
    {
        Text.text = text;
        if (color != default) Text.color = color;

        Text.alignment = alignment;
    }

    private static void _init()
    {
        var obj = new GameObject(nameof(RawText), typeof(Text));
        obj.transform.SetParent(Main.prefabs);
        var text = obj.GetComponent<Text>();
        text.color = Color.white;
        text.font = LocalizedTextManager.current_font;
        text.resizeTextForBestFit = true;
        text.resizeTextMaxSize = 12;
        text.resizeTextMinSize = 1;
        text.alignment = TextAnchor.UpperLeft;
        text.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 240);


        Prefab = obj.AddComponent<RawText>();
        Prefab.text = text;
    }
}