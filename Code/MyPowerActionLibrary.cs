namespace GodTools;

internal static class MyPowerActionLibrary
{
    private static Actor force_attack_attacker;
    private static Actor force_attack_defender;

    internal static bool click_force_attack(WorldTile pTile, string pPowerID)
    {
        if (force_attack_attacker == null || !force_attack_attacker.isAlive())
        {
            force_attack_attacker =
                pTile == null ? World.world.getActorNearCursor() : ActionLibrary.getActorFromTile(pTile);
            if (force_attack_attacker == null)
            {
                Main.LogInfo("Attacker is NULL");
                return false;
            }

            Main.LogInfo($"Attacker is:{force_attack_attacker.data.id}");
            WorldTip.showNow($"选中攻击方:{force_attack_attacker.getName()}", false, "top");
            Main.instance.pos_show_effect_controller.get()
                .show(force_attack_attacker, force_attack_attacker.currentScale, 2f);
            return true;
        }

        if (force_attack_defender == null || !force_attack_defender.isAlive())
        {
            force_attack_defender =
                pTile == null ? World.world.getActorNearCursor() : ActionLibrary.getActorFromTile(pTile);
            if (force_attack_defender == null)
            {
                Main.LogInfo("Defender is NULL");
                return false;
            }

            WorldTip.showNow($"选中受击方:{force_attack_defender.getName()}", false, "top");
            Main.LogInfo($"Defender is:{force_attack_defender.data.id}");
            Main.instance.pos_show_effect_controller.get()
                .show(force_attack_defender, force_attack_defender.currentScale, 0.8f);
        }

        if (force_attack_attacker == force_attack_defender)
        {
            force_attack_attacker = null;
            force_attack_defender = null;
            WorldTip.showNow("取消攻击方选中状态", false, "top");
            return false;
        }

        // 强制攻击数据备份
        var tmp = force_attack_attacker.stats[S.range];
        var angry_civillians = World.world.worldLaws.world_law_angry_civilians.boolVal;
        force_attack_attacker.stats[S.range] = 999;
        World.world.worldLaws.world_law_angry_civilians.boolVal = true;

        force_attack_attacker.attackTimer = 0;
        force_attack_attacker.targetAngle.z = 0;
        force_attack_attacker.updateRotation();

        var result = force_attack_attacker.tryToAttack(force_attack_defender);

        Main.LogInfo($"{force_attack_attacker.data.id} try attack {force_attack_defender.data.id}:{result}");
        WorldTip.showNow(
            $"'{force_attack_attacker.getName()}'强制攻击'{force_attack_defender.getName()}'结果:{LocalizedTextManager.getText(result ? C.success : C.fail)}",
            false, "top");
        //恢复数据
        force_attack_attacker.stats[S.range] = tmp;
        World.world.worldLaws.world_law_angry_civilians.boolVal = angry_civillians;

        Main.instance.pos_show_effect_controller.get()
            .show(force_attack_attacker, force_attack_attacker.currentScale, 2f);

        force_attack_defender = null;
        return true;
    }


    internal static bool init_click_force_attack(string pPowerID)
    {
        force_attack_attacker = null;
        force_attack_defender = null;
        return false;
    }
}