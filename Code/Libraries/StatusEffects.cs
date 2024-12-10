using GodTools.Abstract;
using UnityEngine;

namespace GodTools.Libraries;

public class StatusEffects : ExtendLibrary<StatusEffect, StatusEffects>
{
    public static StatusEffect attack_unit { get; private set; }

    protected override void OnInit()
    {
        RegisterAssets();
        attack_unit.animated = true;
        attack_unit.texture = "aaa";
        attack_unit.sprite_list = new[] { Resources.Load<Sprite>("gt_windows/attack_unit") };
        attack_unit.tier = StatusTier.None;
        attack_unit.duration = 1f;
    }
}