using System;
using System.Collections.Generic;
using System.Linq;
using GodTools.Code;
using GodTools.Extension;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace GodTools.Game;

public class NormalData
{
    public List<string> allowed_races = new();
    public Dictionary<string, List<BuildingAsset>> race_buildings = new();
    public Dictionary<string, List<ActorAsset>> race_actors = new();

    public List<string> first_in_allowed_actions = new();
    public Dictionary<string, List<string>> units_all_allowed_actions = new();
    public Dictionary<string, List<string>> buildings_all_allowed_actions = new();
    
    public Dictionary<string, GameObject> buildings_buttons = new();
    public Dictionary<string, GameObject> units_buttons = new();
    public Dictionary<string, GameObject> first_in_buttons = new();

    public void init()
    {
        check_allowed_races();
        check_units_allowed_actions();
        check_buildings_allowed_actions();
        check_first_in_allowed_actions();
    }

    private void check_first_in_allowed_actions()
    {
        foreach (List<BuildingAsset> _race_buildings in race_buildings.Keys.Select(race_id => race_buildings[race_id]))
        {
            foreach (BuildingAsset building_asset in _race_buildings.Where(building_asset => building_asset.type == SB.type_hall && building_asset.upgradeLevel == 1))
            {
                first_in_allowed_actions.Add(C.place_prefix + building_asset.id);
                GameObject obj = create_place_action_button(building_asset.id);
                if(obj == null) continue;
                first_in_buttons[C.place_prefix + building_asset.id] = obj;
                break;
            }
        }
    }

    private void check_allowed_races()
    {
        foreach (Race race in AssetManager.raceLibrary.list.Where(race => race.civilization))
        {
            #region 检查建筑类型

            foreach (BuildingAsset building_asset in AssetManager.buildings.list.Where(building_asset =>
                         building_asset.race == race.id))
            {
                
            }

            #endregion

            #region 检查生物类型

            foreach (ActorAsset actor_asset in AssetManager.actor_library.list.Where(actor_asset =>
                         actor_asset.race == race.id))
            {
                
            }

            #endregion
            allowed_races.Add(race.id);
            race_buildings[race.id] = new List<BuildingAsset>();
            race_buildings[race.id].AddRange(AssetManager.buildings.list.Where(building_asset =>
                building_asset.race == race.id));

            race_actors[race.id] = new();
            foreach (ActorAsset actor_asset in AssetManager.actor_library.list.Where(actor_asset =>
                         actor_asset.race == race.id))
            {
                if(actor_asset.baby) continue;
                race_actors[race.id].Add(actor_asset);
            }
        }
    }

    private void check_units_allowed_actions()
    {
        foreach (ActorAsset actor_asset in AssetManager.actor_library.list.Where(actor_asset =>
                     !actor_asset.id.StartsWith("_")))
        {
            units_all_allowed_actions[actor_asset.id] = new List<string>();
            if(actor_asset.baby) continue;
            if(!allowed_races.Contains(actor_asset.race)) continue;

            List<BuildingAsset> building_assets = race_buildings[actor_asset.race];
            foreach (BuildingAsset building_asset in building_assets.Where(building_asset => !string.IsNullOrEmpty(building_asset.upgradedFrom)))
            {
                if(units_buttons.ContainsKey(C.build_prefix + building_asset.id)) continue;
                units_all_allowed_actions[actor_asset.id].Add(C.build_prefix + building_asset.id);
                GameObject obj = create_build_action_button(building_asset.id);
                if (obj == null) continue;
                units_buttons[C.build_prefix + building_asset.id] = obj;
            }
        }
    }

    private void check_buildings_allowed_actions()
    {
        foreach (BuildingAsset building_asset in AssetManager.buildings.list.Where(building_asset =>
                     !building_asset.id.StartsWith("!")))
        {
            buildings_all_allowed_actions[building_asset.id] = new List<string>();
            if (building_asset.type == SB.type_hall)
            {
                if (building_asset.upgradeLevel == 0) continue;
                foreach (ActorAsset actor_asset in race_actors[building_asset.race].Where(actor_asset => actor_asset.unit))
                {
                    if (buildings_buttons.ContainsKey(C.spawn_prefix + actor_asset.id)) continue;
                    buildings_all_allowed_actions[building_asset.id].Add(C.spawn_prefix + actor_asset.id);
                    GameObject obj = create_spawn_action_button(actor_asset.id);
                    if (obj == null) continue;
                    buildings_buttons[C.spawn_prefix + actor_asset.id] = obj;
                }
            }
            else if (building_asset.type == SB.type_barracks)
            {
                
            }
        }
    }

    private GameObject create_spawn_action_button(string actor_asset_id, UnityAction action = null)
    {
        ActionWithCostButton button =
            Object.Instantiate(Prefabs.action_with_cost_button_prefab, Main.game_ui_object_temp_library);
        ActorAsset actor_asset = AssetManager.actor_library.get(actor_asset_id);
        Dictionary<string, int> spawn_actor_resource_cost = new();
        try
        {
            spawn_actor_resource_cost =
                Newtonsoft.Json.JsonConvert.DeserializeObject(actor_asset.showIconInspectWindow_id) as
                    Dictionary<string, int> ?? new();
        }
        catch (Exception)
        {
            return null;
        }
        
        if (spawn_actor_resource_cost.Count == 0)
        {
            if (actor_asset.cost.gold > 0) spawn_actor_resource_cost[SR.gold] = actor_asset.cost.gold;
            if (actor_asset.cost.wood > 0) spawn_actor_resource_cost[SR.wood] = actor_asset.cost.wood;
            if (actor_asset.cost.stone > 0) spawn_actor_resource_cost[SR.stone] = actor_asset.cost.stone;
            if (actor_asset.cost.common_metals > 0) spawn_actor_resource_cost[SR.common_metals] = actor_asset.cost.common_metals;
        }
        button.load(SpriteTextureLoader.getSprite($"ui/icons/{actor_asset.icon}"), new(60,60), new TooltipData()
        {
            tip_name = actor_asset.nameLocale,
            tip_description = C.resource_cost,
            tip_description_2 = JsonConvert.SerializeObject(spawn_actor_resource_cost)
        }, action ?? (() =>
        {
            if (Game.instance.input_controller.curr_main_object == null || Game.instance.input_controller.curr_main_object.objectType != MapObjectType.Building) return;
            foreach(BaseSimObject obj in Game.instance.input_controller.selector.active_objects)
            {
                if (obj.objectType != MapObjectType.Building || obj.b.asset != Game.instance.input_controller.curr_main_object.b.asset) continue;

                obj.b.add_progress(C.progress_spawn, actor_asset.id);
            }
        }));
        button.name = C.spawn_prefix + actor_asset_id;
        return button.gameObject;
    }

    private GameObject create_build_action_button(string building_asset_id, UnityAction action = null)
    {
        ActionWithCostButton button =
            Object.Instantiate(Prefabs.action_with_cost_button_prefab, Main.game_ui_object_temp_library);
        BuildingAsset building_asset = AssetManager.buildings.get(building_asset_id);
        Dictionary<string, int> build_building_resource_cost = new();
        try
        {
            build_building_resource_cost =
                Newtonsoft.Json.JsonConvert.DeserializeObject(building_asset.sprite_path) as
                    Dictionary<string, int> ?? new();
        }
        catch (Exception)
        {
            return null;
        }

        if (build_building_resource_cost.Count == 0)
        {
            if (building_asset.cost.gold > 0) build_building_resource_cost[SR.gold] = building_asset.cost.gold;
            if (building_asset.cost.wood > 0) build_building_resource_cost[SR.wood] = building_asset.cost.wood;
            if (building_asset.cost.stone > 0) build_building_resource_cost[SR.stone] = building_asset.cost.stone;
            if (building_asset.cost.common_metals > 0) build_building_resource_cost[SR.common_metals] = building_asset.cost.common_metals;
        }
        button.load(building_asset.sprites.animationData[0].list_main[0], new(60,60), new TooltipData()
        {
            tip_name = building_asset.type,
            tip_description = C.resource_cost,
            tip_description_2 = JsonConvert.SerializeObject(build_building_resource_cost)
        }, action ?? (() =>
        {
            if (Game.instance.player == null || !Game.instance.player.isAlive() ||
                Game.instance.input_controller.selector.type_to_select == MapObjectType.Building) return;

            BuildingPlacer.building_to_place = BuildingPlacer.building_to_place == building_asset ? null : building_asset;
        }));
        button.name = C.build_prefix + building_asset_id;
        return button.gameObject;
    }

    private GameObject create_place_action_button(string building_asset_id, UnityAction action = null)
    {
        ActionWithCostButton button =
            Object.Instantiate(Prefabs.action_with_cost_button_prefab, Main.game_ui_object_temp_library);
        BuildingAsset building_asset = AssetManager.buildings.get(building_asset_id);
        
        button.load(building_asset.sprites.animationData[0].list_main[0], new(60,60), new TooltipData()
        {
            tip_name = building_asset.type,
            tip_description = C.resource_cost,
            tip_description_2 = JsonConvert.SerializeObject(new())
        }, action ?? (() =>
        {
            if (Game.instance.player != null && Game.instance.player.isAlive()) return;

            BuildingPlacer.building_to_place = BuildingPlacer.building_to_place == building_asset ? null : building_asset;
        }));
        button.name = C.place_prefix + building_asset_id;
        return button.gameObject;
    }
}