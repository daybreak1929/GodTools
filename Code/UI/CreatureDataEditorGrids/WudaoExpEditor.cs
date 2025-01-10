
#if INMNY_CUSTOMMODT001
using CustomModT001;
using GodTools.UI.Prefabs;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UPersian.Utils;

namespace GodTools.UI.CreatureDataEditorGrids;

public class WudaoExpEditor : CreatureDataEditorGrid
{
    public override void Setup()
    {
        _text_input = TextInput.Instantiate(transform, pName: "TextInput");
        _text_input.Setup("", UpdateValue);
        _text_input.SetSize(new(200, 20));
        _text_input.GetComponent<RectTransform>().pivot = new(0.5f, 1);
        _text_input.input.characterValidation = InputField.CharacterValidation.Decimal;
        TitleKey = "inmny.godtools.ui.data_editor.inmny_custommodt001.wudao_exp";
    }
    private void UpdateValue(string value)
    {
        if (float.TryParse(value, out float parsed_value))
        {
            _actor.data.set("inmny.custommodt001.wudao_exp", parsed_value);
            _actor.setStatsDirty();
        }
    }
    private TextInput _text_input;
    private Actor _actor;
    public override void EnabledWith(Actor actor)
    {
        _actor = actor;
        actor.data.get("inmny.custommodt001.wudao_exp", out int talent);
        _text_input.Setup(talent.ToString(), UpdateValue);
    }
}
#endif