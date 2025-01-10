
#if CULTIWAY
using Cultiway.Core.Components;
using Cultiway.Utils.Extension;
using GodTools.UI.Prefabs;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.UI.CreatureDataEditorGrids;

public class PowerLevelEditor : CreatureDataEditorGrid
{
    public override void Setup()
    {
        _text_input = TextInput.Instantiate(transform, pName: "TextInput");
        _text_input.Setup("", UpdateValue);
        _text_input.SetSize(new(200, 20));
        _text_input.GetComponent<RectTransform>().pivot = new(0.5f, 1);
        _text_input.input.characterValidation = InputField.CharacterValidation.Integer;
        TitleKey = "inmny.godtools.ui.data_editor.cultiway.power_level";
    }
    private void UpdateValue(string value)
    {
        if (int.TryParse(value, out int parsed_value))
        {
            _actor.GetExtend().AddComponent(new PowerLevel()
            {
                value = Mathf.Clamp(parsed_value, 0, 20)
            });
        }
    }
    private TextInput _text_input;
    private Actor _actor;
    public override void EnabledWith(Actor actor)
    {
        _actor = actor;
        _text_input.Setup(((int)actor.GetExtend().GetPowerLevel()).ToString(), UpdateValue);
    }
}
#endif