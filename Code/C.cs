using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodTools
{
    internal class C
    {
        public const string MOD_NAME = "GodTools";
        public const string PATCH_ID = "inmny.GodTools";
        public const string _default = "gt_default";
        public const string first_in = "first_in";
        public const string GT_force_attack = "GT_ForceAttack";
        public const string GT_about_this = "GT_AboutThis";
        public const string GT_world_law = "GT_WorldLaw";
        public const string GT_new_game = "GT_SelectUnits";
        public const string job_controller = "Job Controller";
        public const string action_controller = "Action Controller";
        public const string attack_controller = "Attack Controller";
        public const string icon_GP_force_attack = "iconDamage";
        public const float pos_show_effect_time_scale = 5f;
        public const float rect_update_time_scale = 3f;
        public const string status_effect_editor_id = "gt_creature_status_effect_editor";
        public const string equipments = "equipments";
        public const string item_editor_id = "gt_item_editor";
        public const string item_editor_description = "gt_item_editor_desc";
        public const string item_from = "item_from";
        public const string item_year = "item_year";
        public const string item_by = "item_by";
        public const string item_material = "item_material";
        public const string item_template = "item_template";
        public const string item_modifier = "item_modifier";
        public const string success = "gt_success";
        public const string fail = "gt_fail";
        public const string exit = "exit";
        public const string title_postfix = "_title";
        public const string desc_postfix = "_description";
        public const string localied_postfix = "_localizedkey";
        public const int find_near_unit_size = 2;
        public const int find_near_building_size = 2;
        public const string flag_in_progress = "gt_building_in_progress";
        public const string status_in_progress = "gt_building_progress_val";
        public const string total_progress_val = "gt_building_progress_total";
        public const string progress_id = "gt_building_progress_id";
        public const string progress_free_str = "gt_building_progress_free_str";
        public const string progress_free_val = "gt_building_progress_free_val";
        public const string progress_count = "gt_building_progress_count";
        public static readonly string aboutthis_title = GT_about_this + title_postfix;
        public static readonly string worldlaw_title = GT_world_law + title_postfix;
        public static readonly string force_attack_title = GT_force_attack + title_postfix;
        public static readonly string select_units_title = GT_new_game + title_postfix;
        public static readonly string job_controller_title = job_controller + title_postfix;
        public static readonly string action_controller_title = action_controller + title_postfix;
        public static readonly string attack_controller_title = attack_controller + title_postfix;
        public static readonly string job_controller_desc = job_controller + desc_postfix;
        public static readonly string action_controller_desc = action_controller + desc_postfix;
        public static readonly string attack_controller_desc = attack_controller + desc_postfix;

        public const string move_to_point = "move_to_point";
        public const string wait = "wait_for_cmd";
        public const string explore = "explore";
        public const string defense = "stay_for_defense";
        public const string attack_unit = "attack_unit";
        public const string task_always_wait = "always_wait";
        public const string task_gt_fight = "gt_fight";
        public const string task_always_move = "always_random_move";
        public const string task_find_tile = "find_target_tile";
        public const string task_construct_new_building = "gt_construct_building";
        public const string job_player_kingdom = "simple_player_kingdom";
        public const string job_player_city = "simple_player_city";

        public const string progress_spawn = "spawn_unit";
        public static readonly string progress_spawn_localized = progress_spawn + localied_postfix;

        public const string localized_part_actor_asset = "$actor$";
    }
}
