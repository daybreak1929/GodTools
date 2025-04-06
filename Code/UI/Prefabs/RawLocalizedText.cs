using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.UI.Prefabs;

public class RawLocalizedText : APrefab<RawLocalizedText>
{
    private LocalizedText mLocalizedText;
    private Text          mText;

    public Text Text
    {
        get
        {
            Init();
            return mText;
        }
    }

    public LocalizedText LocalizedText
    {
        get
        {
            Init();
            return mLocalizedText;
        }
    }

    protected override void Init()
    {
        if (Initialized) return;
        base.Init();

        mText = GetComponent<Text>();
        mLocalizedText = GetComponent<LocalizedText>();
    }

    public void Setup(string text_key, Color color = default, TextAnchor alignment = TextAnchor.UpperLeft)
    {
        LocalizedText.setKeyAndUpdate(text_key);
        if (color != default) Text.color = color;

        Text.alignment = alignment;
    }

    private static void _init()
    {
        var obj = new GameObject(nameof(RawText), typeof(Text), typeof(LocalizedText));
        obj.transform.SetParent(Main.prefabs);
        var text = obj.GetComponent<Text>();
        text.color = Color.white;
        text.font = LocalizedTextManager.current_font;
        text.resizeTextForBestFit = true;
        text.resizeTextMaxSize = 12;
        text.resizeTextMinSize = 1;
        text.alignment = TextAnchor.UpperLeft;
        text.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 240);


        Prefab = obj.AddComponent<RawLocalizedText>();
    }
}