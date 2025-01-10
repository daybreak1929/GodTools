using GodTools.UI.Prefabs;
using NeoModLoader.api.attributes;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UPersian.Utils;

namespace GodTools.UI.CreatureDataEditorGrids;

public class FamilyNameEditor : CreatureDataEditorGrid
{
    public override void Setup()
    {
        _text_input = TextInput.Instantiate(transform, pName: "TextInput");
        _text_input.Setup("", UpdateFamilyName);
        _text_input.SetSize(new(200, 20));
        _text_input.GetComponent<RectTransform>().pivot = new(0.5f, 1);
        TitleKey = "inmny.godtools.ui.data_editor.chinese_name.family_name";
    }
    [Hotfixable]
    private void UpdateFamilyName(string value)
    {
        _actor.data.get("chinese_family_name", out string old_value);
        _actor.data.set("chinese_family_name", value);
        _actor.data.setName(_actor.data.name.ReplaceFirst(old_value, value));
    }
    private TextInput _text_input;
    private Actor _actor;
    [Hotfixable]
    public override void EnabledWith(Actor actor)
    {
        _actor = actor;
        actor.getName();
        actor.data.get("chinese_family_name", out string family_name);
        _text_input.Setup(family_name, UpdateFamilyName);
    }
}