using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodTools.Code
{
    internal static class MyLocalizedTextManager
    {
        public static void init()
        {
            set(EquipmentType.Weapon.ToString(), "武器");
            set(EquipmentType.Boots.ToString(), "靴子");
            set(EquipmentType.Helmet.ToString(), "头盔");
            set(EquipmentType.Ring.ToString(), "戒指");
            set(EquipmentType.Amulet.ToString(), "项链");
            set(EquipmentType.Armor.ToString(), "胸甲");
            set(C.item_editor_description, "上方选择装备，下方编辑");
            set(C.item_template, "样式");
            set(C.item_by, "锻造者");
            set(C.item_from, "锻造地");
            set(C.item_year, "锻造年份");
            set(C.item_material, "材质");
            set(C.item_modifier, "词缀");
            set(C.success, "成功");
            set(C.fail, "失败");
            set(C.aboutthis_title, "此模组");
            set(C.worldlaw_title, "世界法则");
            set(C.force_attack_title, "强制攻击");
            set(C.select_units_title, "选择单位");
            set(C.equipments, "装备");
            set(C.action_controller_title, "行动");
            set(C.attack_controller_title, "攻击");
            set(C.job_controller_title, "职业");
            set(C.action_controller_desc, "描述");
            set(C.attack_controller_desc, "描述");
            set(C.job_controller_desc, "描述");
            set(C.move_to_point + C.title_postfix, "移动至");
            set(C.wait + C.title_postfix, "待命");
            set(C.attack_unit + C.title_postfix, "攻击");
            set(C.defense + C.title_postfix, "驻守");
            set(C.exit + C.title_postfix, "退出");
            set(C.explore + C.title_postfix, "探索");
            set(C.progress_spawn_localized, "正在生产:$actor$");
            set(C.mod_prefix+"tent", "帐篷");
            set(C.mod_prefix + "build_tent", "建造帐篷");
            set(C.mod_prefix + "resource_cost", "资源消耗");
        }
        private static void set(string key, string value)
        {
            NCMS.Utils.Localization.Set(key, value);
        }
    }
}
