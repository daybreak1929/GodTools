using System;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.UI.Prefabs;

public class InputBar : APrefab<InputBar>
{
    public InputField InputField { get; private set; }

    protected override void Init()
    {
        if (Initialized) return;
        base.Init();

        InputField = transform.Find("Input").GetComponent<InputField>();
    }

    public void Setup(string         text_on_empty      = "", Action<string> on_value_changed = null,
                      Action<string> on_value_submitted = null)
    {
        Init();
        if (on_value_changed != null)
            InputField.onValueChanged.AddListener(on_value_changed.Invoke);
        if (on_value_submitted != null)
            InputField.onEndEdit.AddListener(on_value_submitted.Invoke);
    }

    private static void _init()
    {
        var obj = new GameObject(nameof(InputBar), typeof(Image), typeof(HorizontalLayoutGroup));
        obj.transform.SetParent(Main.prefab_library);

        var bg = obj.GetComponent<Image>();
        bg.sprite = SpriteTextureLoader.getSprite("ui/special/windowNameEdit");
        bg.type = Image.Type.Sliced;


        var input_obj = new GameObject("Input", typeof(InputField), typeof(Text));
        input_obj.transform.SetParent(obj.transform);

        var input = input_obj.GetComponent<InputField>();
        var text = input_obj.GetComponent<Text>();

        input.textComponent = text;
        text.font = LocalizedTextManager.currentFont;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 1;
        text.resizeTextMaxSize = 10;
        text.alignment = TextAnchor.MiddleCenter;


        Prefab = obj.AddComponent<InputBar>();
    }
}