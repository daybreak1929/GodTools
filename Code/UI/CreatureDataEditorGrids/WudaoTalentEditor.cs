
#if INMNY_CUSTOMMODT001
using CustomModT001;
using GodTools.UI.Prefabs;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UPersian.Utils;


namespace GodTools.UI.CreatureDataEditorGrids;

public class WudaoTalentEditor : CreatureDataEditorGrid
{
    public override void Setup()
    {
        _text_input = TextInput.Instantiate(transform, pName: "TextInput");
        _text_input.Setup("", UpdateTalent);
        _text_input.SetSize(new(200, 20));
        _text_input.GetComponent<RectTransform>().pivot = new(0.5f, 1);
        _text_input.input.characterValidation = InputField.CharacterValidation.Integer;
        TitleKey = "inmny.godtools.ui.data_editor.inmny_custommodt001.talent";
    }
    private void UpdateTalent(string value)
    {
        if (int.TryParse(value, out int talent))
            _actor.data.set("inmny.custommodt001.talent", talent);
    }
    private TextInput _text_input;
    private Actor _actor;
    public override void EnabledWith(Actor actor)
    {
        _actor = actor;
        _text_input.Setup(((int)actor.GetTalent()).ToString(), UpdateTalent);
    }
}
#endif