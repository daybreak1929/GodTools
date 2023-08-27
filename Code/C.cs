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
        public const string CW_MOD_NAME = "启源核心";
        public const string PATCH_ID = "inmny.GodTools";
        public const string _default = "gt_default";
        public const string mod_prefix = "gt_";
        public const string build_prefix = "build_";
        public const string spawn_prefix = "spawn_";
        public const string place_prefix = "place_";
        public const string title_postfix = "_title";
        public const string desc_postfix = "_description";
        public const string localied_postfix = "_localizedkey";
        public const string first_in = "first_in";
        public const string force_attack = "gt_force_attack";
        public const string about_this = "gt_about_this";
        public const string world_law = "gt_world_law";
        public const string new_game = "gt_new_game";
        public const string resource_cost = "gt_resource_cost";
        public const string icon_GP_force_attack = "iconDamage";
        public const float pos_show_effect_time_scale = 5f;
        public const string status_effect_editor_id = "gt_creature_status_effect_editor";
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
        public const string exit = "gt_exit";
        public const int find_near_unit_size = 2;
        public const int find_near_building_size = 2;
        public const string flag_in_progress = "gt_building_in_progress";
        public const string status_in_progress = "gt_building_progress_val";
        public const string total_progress_val = "gt_building_progress_total";
        public const string progress_id = "gt_building_progress_id";
        public const string progress_free_str = "gt_building_progress_free_str";
        public const string progress_free_val = "gt_building_progress_free_val";
        public const string progress_count = "gt_building_progress_count";

        public const string move_to_point = "gt_move_to_point";
        public const string wait = "gt_wait_for_cmd";
        public const string explore = "gt_explore";
        public const string defense = "gt_stay_for_defense";
        public const string attack_unit = "gt_attack_unit";
        public const string task_always_wait = "gt_always_wait";
        public const string task_gt_fight = "gt_fight";
        public const string task_always_move = "gt_always_random_move";
        public const string task_find_tile = "gt_find_target_tile";
        public const string task_construct_new_building = "gt_construct_building";
        public const string job_player_kingdom = "simple_player_kingdom";
        public const string job_player_city = "simple_player_city";
        
        public const string progress_spawn = "gt_spawn_unit";
        public static readonly string progress_spawn_localized = progress_spawn + localied_postfix;
        
        public static readonly string action_with_cost_tooltip = mod_prefix + "action_with_cost";

        public const string localized_part_actor_asset = "$actor$";
    }
}
