using GodTools.Abstract;
using GodTools.Features;
using GodTools.UI;

namespace GodTools.Libraries;

public class GodPowers : ExtendLibrary<GodPower, GodPowers>
{
    public static GodPower place_saved_actor { get; private set; }
    public static GodPower split_city { get; private set; }

    protected override void OnInit()
    {
        RegisterAssets();
        /*
        var powers = AssetManager.powers;

        // 强制攻击
        var power = new GodPower();
        power.id = C.force_attack;
        power.name = power.id;
        power.path_icon = C.icon_GP_force_attack;
        power.select_button_action = (PowerButtonClickAction)Delegate.Combine(power.select_button_action,
            new PowerButtonClickAction(
                MyPowerActionLibrary
                    .init_click_force_attack));
        power.click_special_action = (PowerActionWithID)Delegate.Combine(power.click_special_action,
            new PowerActionWithID(
                MyPowerActionLibrary.click_force_attack));
        powers.add(power);
        */
        place_saved_actor.name = place_saved_actor.id;
        place_saved_actor.path_icon = "gt_windows/save_actor_list";
        place_saved_actor.click_action = WindowCreatureSavedList.SpawnSelectedSavedActor;

        split_city.name = split_city.id;
        split_city.path_icon = "ui/icons/iconFinger";
        split_city.select_button_action = CitySplit.ResetCitySplitState;
        split_city.click_special_action = null;
    }
}