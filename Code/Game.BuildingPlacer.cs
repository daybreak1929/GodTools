using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodTools.Game
{
    internal class BuildingPlacer
    {
        public static BuildingAsset building_to_place;
        public static bool try_to_place_building(WorldTile center_tile, bool finished)
        {
            if (center_tile == null || building_to_place==null) return false;
            if ((Game.instance.player==null && building_to_place.type!=SB.type_hall)||(Game.instance.player!=null && !Game.instance.player.isAlive())) return false;

            Building building = World.world.buildings.addBuilding(building_to_place.id, center_tile, true, false, BuildPlacingType.New);
            if (building == null) return false;
            if (Game.instance.player == null)
            {
                City captital = World.world.cities.buildNewCity(building.currentTile.zone, AssetManager.raceLibrary.get(string.IsNullOrEmpty(building_to_place.race)?SK.human:building_to_place.race), null);
                Game.instance.player = captital.kingdom;
                Game.instance.player.capital = captital;
                Game.instance.init_curr_kingdom();
            }
            Game.instance.player.capital.addBuilding(building);
            if (!finished)
            {
                building.setUnderConstruction();
                Code.Main.log("A building under construction");
                if (Game.instance.input_controller.selector.type_to_select == MapObjectType.Actor)
                {
                    foreach (Actor actor in Game.instance.input_controller.selector.active_objects)
                    {
                        if (actor.isAlive()) { actor.ai.setTask(C.task_construct_new_building); actor.beh_building_target = building; }
                    }
                }
            }   
            return true;
        }
        //static List<WorldTile> highlight_temp_tiles = new List<WorldTile>();
        static ColorArray red_array = new ColorArray(1f, 0, 0, 1, 30, 1);
        internal static void highlight_place_tiles(WorldTile center_tile)
        {
            int x_start = center_tile.x - building_to_place.fundament.left;
            int y_start = center_tile.y - building_to_place.fundament.bottom;
            int x_len = building_to_place.fundament.right + building_to_place.fundament.left + 1;
            int y_len = building_to_place.fundament.bottom + building_to_place.fundament.top + 1;
            ColorType color = World.world.buildings.canBuildFrom(center_tile, building_to_place, null, BuildPlacingType.New) ? ColorType.White : ColorType.Blue;
            //if (color == ColorType.Purple) Code.Main.log("PURPLE");
            //highlight_temp_tiles.Clear();
            for (int x = 0; x < x_len; x++)
            {
                for(int y = 0; y < y_len; y++)
                {
                    WorldTile tile = World.world.GetTile(x+x_start, y+y_start);
                    if (tile != null)
                    {
                        World.world.flashEffects.flashPixel(tile, 20, color);
                        if (color == ColorType.Blue) tile.colorArray = red_array;
                    }
                }
            }
        }
    }
}
