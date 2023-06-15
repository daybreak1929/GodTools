using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodTools.Code
{
    internal class MyPowers
    {
        public static Dictionary<string, PowerButtonClickAction> post_actions = new Dictionary<string, PowerButtonClickAction>();
        public static void init()
        {
            PowerLibrary powers = AssetManager.powers;

            // 强制攻击
            GodPower power = new GodPower();
            power.id = C.GT_force_attack;
            power.name = power.id;
            power.path_icon = C.icon_GP_force_attack;
            power.select_button_action = (PowerButtonClickAction)Delegate.Combine(power.select_button_action, new PowerButtonClickAction(MyPowerActionLibrary.init_click_force_attack));
            power.click_special_action = (PowerActionWithID)Delegate.Combine(power.click_special_action, new PowerActionWithID(MyPowerActionLibrary.click_force_attack));
            powers.add(power);
        }
    }
}
