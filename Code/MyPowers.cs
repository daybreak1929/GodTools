using System;
using System.Collections.Generic;

namespace GodTools;

internal static class MyPowers
{
    public static Dictionary<string, PowerButtonClickAction> post_actions = new();

    public static void init()
    {
        var powers = AssetManager.powers;

        // 强制攻击
        var power = new GodPower();
        power.id = C.force_attack;
        power.name = power.id;
        power.path_icon = C.icon_GP_force_attack;
        power.select_button_action = (PowerButtonClickAction)Delegate.Combine(power.select_button_action,
            new PowerButtonClickAction(MyPowerActionLibrary.init_click_force_attack));
        power.click_special_action = (PowerActionWithID)Delegate.Combine(power.click_special_action,
            new PowerActionWithID(MyPowerActionLibrary.click_force_attack));
        powers.add(power);
    }
}