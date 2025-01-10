
#if INMNY_CUSTOMMODT001
using CustomModT001;
using GodTools.UI.Prefabs;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UPersian.Utils;

namespace GodTools.UI.CreatureDataEditorGrids;

public class WudaoLevelEditor : CreatureDataEditorGrid
{
    public override void Setup()
    {
        _text_input = TextInput.Instantiate(transform, pName: "TextInput");
        _text_input.Setup("", UpdateValue);
        _text_input.SetSize(new(200, 20));
        _text_input.GetComponent<RectTransform>().pivot = new(0.5f, 1);
        _text_input.input.onValidateInput += (text, charIndex, addedChar) =>
        {
            if (addedChar < '0' || addedChar > '9')
            {
                return '\0';
            }
            if (int.TryParse(text + addedChar, out int value))
            {
                if (value > Cultisys.MaxLevel)
                {
                    return '\0';
                }
            }
            return addedChar;
        };
        TitleKey = "inmny.godtools.ui.data_editor.inmny_custommodt001.wudao_level";
    }
    private void UpdateValue(string value)
    {
        if (int.TryParse(value, out int int_value))
        {
            _actor.data.set("inmny.custommodt001.wudao_level", int_value);
            _actor.setStatsDirty();
        }
    }
    private TextInput _text_input;
    private Actor _actor;
    public override void EnabledWith(Actor actor)
    {
        _actor = actor;
        actor.data.get("inmny.custommodt001.wudao_level", out int value);
        _text_input.Setup(value.ToString(), UpdateValue);
    }
}
#endif