
#if CULTIWAY
using Cultiway.Core.Components;
using Cultiway.Utils.Extension;
using UnityEngine;

namespace GodTools.UI.CreatureDataEditorGrids;

public class PowerLevelEditor : SimpleInputEditor
{
    protected override string GetTitleKey()
    {
        return "inmny.godtools.ui.data_editor.cultiway.power_level";
    }

    protected override void UpdateValue(string value)
    {
        if (int.TryParse(value, out int parsed_value))
        {
            Actor.GetExtend().AddComponent(new PowerLevel()
            {
                value = Mathf.Clamp(parsed_value, 0, 20)
            });
        }
    }
    protected override string GetInitValue()
    {
        return ((int)Actor.GetExtend().GetPowerLevel()).ToString();
    }
}
#endif