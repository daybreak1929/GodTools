using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GodTools.Code;
namespace GodTools.Game
{
    internal class InputController
    {
        public bool enabled { get; private set; }
        private MapObjectType type_to_select;
        public ObjectRectSelector selector { get; private set; }
        private PosShowEffect click_pos_show;
        public Sprite sprite_under_cursor { get; private set; }
        public BaseSimObject curr_main_object { get; private set; }
        public void init()
        {
            selector = new GameObject("Rect Selector", typeof(SpriteRenderer), typeof(ObjectRectSelector)).GetComponent<ObjectRectSelector>();
            selector.gameObject.SetActive(false);
            click_pos_show = Main.instance.pos_show_effect_controller.get(true);
        }
        public void enable()
        {
            enabled = true;
            selector.start();
            curr_main_object = null;
        }
        public void disable()
        {
            enabled = false;
            selector.finish();
            selector.clear_active();
            curr_main_object = null;
        }
        public void set_type(MapObjectType type) 
        {
            if (selector.type_to_select != type)
            {
                selector.clear_active();
                selector.type_to_select = type;
                curr_main_object = null;
            }
        }
        public void update(float elapsed)
        {
            if (!enabled) return;
            sprite_under_cursor = null;
            click_pos_show.update(elapsed);
            if (MapBox.instance.isOverUI()) return;
            if (selector.type_to_select == MapObjectType.Actor)
            {
                update_control_for_actors();
                update_buildings_place(false);
            }
            else
            {
                update_control_for_buildings();
                update_buildings_place(true);
            }
            if (selector.active_objects.Count > 0) curr_main_object = selector.active_objects.First();
        }

        private void update_buildings_place(bool finished)
        {
            if (BuildingPlacer.building_to_place != null)
            {
                sprite_under_cursor = BuildingPlacer.building_to_place.sprites.animationData[0].list_main[0];

                WorldTile center_tile = World.world.getMouseTilePos();
                if (center_tile == null) return;

                BuildingPlacer.highlight_place_tiles(center_tile);

                if (Input.GetMouseButtonUp(0))
                {
                    BuildingPlacer.try_to_place_building(World.world.getMouseTilePos(), finished);
                }
            }
        }

        private void update_control_for_buildings()
        {
            Building selected_building = Helper.get_building_near_mouse();
            if (selected_building == null) { click_pos_show.set_active(false); return; }
            if(click_pos_show.finished() || click_pos_show.selected_object!=selected_building) click_pos_show.show(selected_building, selected_building.currentScale, -1);

            // 单选建筑
            if (((Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.LeftControl)) && (Input.GetMouseButtonUp(1))|| Input.GetMouseButtonUp(0)))
            {
                if (selector.active_objects.Contains(selected_building))
                {
                    selector.active_objects.Remove(selected_building);
                    List<PosShowEffect> effects = Main.instance.pos_show_effect_controller.get_active_effects();
                    foreach (PosShowEffect effect in effects)
                    {
                        if (effect.selected_object == selected_building && effect.left_time < -1)
                        {
                            effect.stop();
                            break;
                        }
                    }
                }
                else if (Game.instance.player == null || !Game.instance.player.isAlive() || selected_building.kingdom == Game.instance.player)
                {
                    selector.active_objects.Add(selected_building);
                    Main.instance.pos_show_effect_controller.get().show(selected_building, selected_building.currentScale, -1);
                }
                return;
            }

        }

        private void update_control_for_actors()
        {
            
            if(Input.GetKey(KeyCode.LeftShift))
            {// 攻击单位
                Actor selected_actor = Helper.get_actor_near_mouse();
                if (selected_actor != null)
                {
                    sprite_under_cursor = SpriteTextureLoader.getSprite("ui/icons/iconDamage");
                    if (Input.GetMouseButtonUp(1))
                    {
                        MyActorTasks.BehCheckGTFight.target = selected_actor;
                        foreach (BaseSimObject obj in selector.active_objects)
                        {
                            obj.a.cancelAllBeh(null);
                            obj.a.ai.setJob(C.attack_unit);
                        }
                        selected_actor.addStatusEffect(C.attack_unit);

                        if (selector.active_objects.Count > 0) click_pos_show.show(selected_actor, selected_actor.currentScale);
                        return;
                    }
                }
                // 修理/建造建筑
                Building building_to_construct = Helper.get_building_near_mouse(Game.instance.player);
                if(building_to_construct !=null && (building_to_construct.isUnderConstruction() || building_to_construct.data.health < building_to_construct.getMaxHealth()))
                {
                    sprite_under_cursor = SpriteTextureLoader.getSprite("ui/icons/items/icon_hammer_wood");
                    if (Input.GetMouseButtonUp(1))
                    {
                        foreach (BaseSimObject obj in selector.active_objects)
                        {
                            if (obj.a.isAlive() && obj.kingdom == Game.instance.player) { obj.a.cancelAllBeh(null); obj.a.ai.setTask(C.task_construct_new_building); obj.a.beh_building_target = building_to_construct; }
                        }
                    }
                    if (selector.active_objects.Count > 0) click_pos_show.show(building_to_construct, building_to_construct.currentScale);
                    return;
                }
            }
            
            // 单选单位
            if((Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.LeftControl)) && Input.GetMouseButtonUp(1))
            {
                Actor selected_actor = Helper.get_actor_near_mouse();
                if (selected_actor != null)
                {
                    if (selector.active_objects.Contains(selected_actor))
                    {
                        selector.active_objects.Remove(selected_actor);
                        List<PosShowEffect> effects = Main.instance.pos_show_effect_controller.get_active_effects();
                        foreach(PosShowEffect effect in effects)
                        {
                            if(effect.selected_object == selected_actor && effect.left_time < -1)
                            {
                                effect.stop();
                                break;
                            }
                        }
                    }
                    else if(Game.instance.player==null || !Game.instance.player.isAlive()||selected_actor.kingdom == Game.instance.player)
                    {
                        selector.active_objects.Add(selected_actor);
                        Main.instance.pos_show_effect_controller.get().show(selected_actor, selected_actor.currentScale, -1);
                    }
                }
                return;
            }
            // 移动
            if (Input.GetMouseButtonUp(1))
            {
                WorldTile tile = World.world.getMouseTilePos();
                if (tile != null)
                {
                    foreach (BaseSimObject obj in selector.active_objects)
                    {
                        obj.a.cancelAllBeh(null);
                        obj.a.attackTarget = null;
                        obj.a.goTo(tile, true, false);
                    }
                    if (selector.active_objects.Count>0) click_pos_show.show(World.world.getMousePos());
                    return;
                }
            }
        }
    }
}
