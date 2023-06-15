using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GodTools.Code;
namespace GodTools.Extension
{
    internal static class BuildingExtension
    {
        public static void addProgress(this Building building, string progress_id, string custom_str="", float custom_val=0)
        {
            BuildingProgressAsset progress_asset = Main.building_progresses.get(progress_id);
            if (!building.data.hasFlag(C.flag_in_progress)) { building.data.addFlag(C.flag_in_progress); building.data.set(C.status_in_progress, progress_asset.progress); }
            building.data.get(C.progress_id, out string cur_progress_id, null);
            if(cur_progress_id!=null && cur_progress_id != progress_id)
            {
                BuildingProgressAsset cur_progress_asset = Main.building_progresses.get(cur_progress_id);
                if (cur_progress_asset.prio >= progress_asset.prio) return;
                building.data.set(C.status_in_progress, progress_asset.progress);
            }
            building.data.set(C.progress_id, progress_id);
            building.data.change(C.progress_count, 1);
            building.data.set(C.progress_free_str, custom_str);
            building.data.set(C.progress_free_val, custom_val);
           
        }
        public static void updateProgress(this Building building)
        {
            building.data.get(C.status_in_progress, out float cur_progress, 999);
            cur_progress -= World.world.elapsed;
            if(cur_progress <= 0)
            {
                building.data.get(C.progress_id, out string progress_id, null);
                BuildingProgressAsset cur_progress_asset = Main.building_progresses.get(progress_id);
                building.data.get(C.progress_free_str, out string free_str, null);
                building.data.get(C.progress_free_val, out float free_val, 0);

                if (cur_progress_asset.end_action != null)
                {
                    if (!cur_progress_asset.end_action(building, free_str, free_val)) return; 
                }

                building.data.get(C.progress_count, out int count, 0);
                if(count > 1)
                {
                    building.data.set(C.progress_count, count - 1);
                    cur_progress = cur_progress_asset.progress;
                    if(cur_progress_asset.begin_action!=null) cur_progress_asset.begin_action(building, free_str, free_val);
                }
                else
                {
                    building.data.removeFlag(C.flag_in_progress);
                }
            }
            building.data.set(C.status_in_progress, cur_progress);
        }
    }
}
