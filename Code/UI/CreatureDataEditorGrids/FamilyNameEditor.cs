using GodTools.UI.Prefabs;
using NeoModLoader.api.attributes;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UPersian.Utils;

namespace GodTools.UI.CreatureDataEditorGrids;

public class FamilyNameEditor : SimpleInputEditor
{
    protected override string GetTitleKey()
    {
        return "inmny.godtools.ui.data_editor.chinese_name.family_name";
    }

    protected override void OnSetup()
    {
        TextInput.input.characterValidation = InputField.CharacterValidation.None;
    }

    protected override void UpdateValue(string value)
    {
        Actor.data.get("chinese_family_name", out string old_value);
        Actor.data.set("chinese_family_name", value);
        Actor.data.setName(Actor.data.name.ReplaceFirst(old_value, value));
    }

    protected override string GetInitValue()
    {
        Actor.getName();
        Actor.data.get("chinese_family_name", out string family_name);
        return family_name;
    }
}