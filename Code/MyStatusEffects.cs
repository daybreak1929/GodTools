using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GodTools.Code
{
    internal class MyStatusEffects
    {
        public static void init()
        {
            StatusEffect status;
            status = new StatusEffect();
            status.id = C.attack_unit;
            status.animated = true;
            status.texture = "aaa";
            status.sprite_list = new UnityEngine.Sprite[] { Resources.Load<Sprite>("gt_windows/attack_unit") };
            status.tier = StatusTier.None;
            status.duration = 1f;
            add(status);
            
        }
        private static void add(StatusEffect status) { AssetManager.status.add(status); }
    }
}
