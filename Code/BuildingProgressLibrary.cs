using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodTools.Code
{
    internal class BuildingProgressLibrary : AssetLibrary<BuildingProgressAsset>
    {
        public override void init()
        {
            base.init();
            BuildingProgressAsset asset;
            asset = new BuildingProgressAsset();
            asset.id = C.progress_spawn;
            asset.begin_action = null;
            asset.end_action = spawn_unit;
            asset.prio = 0;
            asset.progress = 10f;
            asset.translate_key = C.progress_spawn_localized;
            asset.translate_action = _replace_actor;
            this.add(asset);
        }
        private string _replace_actor(string input_str, string custom_str, float custom_val)
        {
            return input_str.Replace(C.localized_part_actor_asset, LocalizedTextManager.getText(AssetManager.actor_library.get(custom_str).nameLocale));
        }
        private bool spawn_unit(Building building, string custom_str, float custom_val)
        {
            //Main.log("Spawn Unit:" + custom_str);
            if (building.kingdom.capital.getPopulationTotal() >= building.kingdom.capital.getPopulationMaximum()) return false;

            WorldTile spawn_tile = building.currentTile;
            Actor spawned_unit = World.world.units.spawnNewUnit(custom_str, spawn_tile, true, 0);

            if (spawned_unit == null) return false;

            spawned_unit.ai.nextJobDelegate = Delegates.wait_for_cmd_at_next_job;
            building.city.addNewUnit(spawned_unit, true);
            return true;
        }
    }
}
