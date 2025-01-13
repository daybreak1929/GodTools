
#if INMNY_CUSTOMMODT001
using CustomModT001;
using GodTools.UI.Prefabs;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UPersian.Utils;

namespace GodTools.UI.CreatureDataEditorGrids;

public class WudaoLevelEditor : SimpleInputEditor
{
    protected override string GetTitleKey()
    {
        return "inmny.godtools.ui.data_editor.inmny_custommodt001.wudao_level";
    }

    protected override void UpdateValue(string value)
    {
        if (int.TryParse(value, out int int_value))
        {
            Actor.data.set("inmny.custommodt001.wudao_level", int_value);
            Actor.setStatsDirty();
        }
    }

    protected override void OnSetup()
    {
        TextInput.input.onValidateInput += (text, charIndex, addedChar) =>
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
    }

    protected override string GetInitValue()
    {
        Actor.data.get("inmny.custommodt001.wudao_level", out int value);
        return value.ToString();
    }
}
#endif