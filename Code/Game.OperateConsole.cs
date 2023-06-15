using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using GodTools.Extension;
namespace GodTools.Game
{
    internal class OperateConsole : MonoBehaviour
    {
        private GameObject info_displayer;
        private GameObject special_container;
        private GameObject common_container;
        private GameObject map_container;
        public void init()
        {
            this.transform.localPosition = new Vector3(0, 0);
            GameObject exit_button = Helper.create_button_with_bg("Exit", "gt_windows/exit", Code.Main.game.end, C.exit);
            exit_button.transform.SetParent(this.transform);
            exit_button.transform.localPosition = new Vector3(860, 480);
            exit_button.transform.localScale = new Vector3(0.8f, 0.8f);

            GameObject top = new GameObject("Top", typeof(Image));
            top.transform.SetParent(this.transform);
            top.GetComponent<Image>().sprite = Helper.get_button();
            top.GetComponent<Image>().type = Image.Type.Sliced;
            top.GetComponent<RectTransform>().sizeDelta = new Vector2(1700, 60);
            top.transform.localPosition = new Vector3(-52.2f, 480);
            top.transform.localScale = new Vector3(1, 1);

            GameObject modes = new GameObject("Modes", typeof(Image));
            modes.transform.SetParent(this.transform);
            modes.GetComponent<Image>().sprite = Helper.get_button();
            modes.GetComponent<Image>().type = Image.Type.Sliced;
            modes.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 200);
            modes.transform.localPosition = new Vector3(860, 50);
            modes.transform.localScale = new Vector3(1, 1);

            GameObject vanllia = new GameObject("Vanllia", typeof(Image));
            vanllia.GetComponent<Image>().sprite = Helper.get_button();
            vanllia.GetComponent<Image>().type = Image.Type.Sliced;
            vanllia.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
            GameObject __vanllia_mode_button = new GameObject("Button", typeof(Image), typeof(Button));
            __vanllia_mode_button.GetComponent<Image>().sprite = Resources.Load<Sprite>("gt_windows/worldbox");
            __vanllia_mode_button.transform.SetParent(vanllia.transform);
            __vanllia_mode_button.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);
            __vanllia_mode_button.transform.localPosition = new Vector3(0, 0);
            __vanllia_mode_button.transform.localScale = new Vector3(1, 1);
            __vanllia_mode_button.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
            {
                info_displayer.GetComponent<InfoDisplayer>().hide();
                info_displayer.SetActive(false);
                special_container.SetActive(false);
                common_container.SetActive(false);
                map_container.SetActive(false);
                Code.Main.game.input_controller.disable();
                CanvasMain.instance.canvas_ui.transform.Find("CanvasBottom").gameObject.SetActive(true);
            }));
            vanllia.transform.SetParent(modes.transform);
            vanllia.transform.localPosition = new Vector3(0, 75);
            vanllia.transform.localScale = new Vector3(1, 1);
            

            GameObject building = GameObject.Instantiate(vanllia, modes.transform);
            building.name = "Buildings";
            building.transform.Find("Button").GetComponent<Image>().sprite = Resources.Load<Sprite>("ui/icons/items/icon_hammer_wood");
            building.transform.localPosition = new Vector3(0, 25);
            building.transform.localScale = new Vector3(1, 1);
            building.transform.Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
            building.transform.Find("Button").GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
            {
                CanvasMain.instance.canvas_ui.transform.Find("CanvasBottom").gameObject.SetActive(false);
                PowerButtonSelector.instance.unselectAll();
                info_displayer.SetActive(true);
                special_container.SetActive(true);
                common_container.SetActive(true);
                map_container.SetActive(true);
                BuildingPlacer.building_to_place = null;
                Code.Main.game.input_controller.enable();
                Code.Main.game.input_controller.set_type(MapObjectType.Building);
                info_displayer.GetComponent<InfoDisplayer>().hide();
                special_container.GetComponent<ButtonContainerController>().switch_to_container(building.name);
                common_container.GetComponent<ButtonContainerController>().switch_to_container(building.name);
                if(Game.instance.player==null || !Game.instance.player.isAlive())
                {
                    special_container.GetComponent<ButtonContainerController>().switch_to_page(C.first_in);
                    common_container.GetComponent<ButtonContainerController>().switch_to_page(C.first_in);
                }
            }));

            GameObject units = GameObject.Instantiate(vanllia, modes.transform);
            units.name = "Units";
            units.transform.Find("Button").GetComponent<Image>().sprite = Resources.Load<Sprite>("ui/icons/iconPopulation");
            units.transform.localPosition = new Vector3(0, -25);
            units.transform.localScale = new Vector3(1, 1);
            units.transform.Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
            units.transform.Find("Button").GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
            {
                CanvasMain.instance.canvas_ui.transform.Find("CanvasBottom").gameObject.SetActive(false);
                PowerButtonSelector.instance.unselectAll();
                info_displayer.SetActive(true);
                special_container.SetActive(true);
                common_container.SetActive(true);
                map_container.SetActive(true);
                BuildingPlacer.building_to_place = null;
                Code.Main.game.input_controller.enable();
                Code.Main.game.input_controller.set_type(MapObjectType.Actor);
                info_displayer.GetComponent<InfoDisplayer>().hide();
                special_container.GetComponent<ButtonContainerController>().switch_to_container(units.name);
                common_container.GetComponent<ButtonContainerController>().switch_to_container(units.name);
            }));

            GameObject diplomacy = GameObject.Instantiate(vanllia, modes.transform);
            diplomacy.name = "Diplomacy";
            diplomacy.transform.Find("Button").GetComponent<Image>().sprite = Resources.Load<Sprite>("ui/icons/iconKingdomList");
            diplomacy.transform.localPosition = new Vector3(0, -75);
            diplomacy.transform.localScale = new Vector3(1, 1);
            diplomacy.transform.Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
            diplomacy.transform.Find("Button").GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
            {
                CanvasMain.instance.canvas_ui.transform.Find("CanvasBottom").gameObject.SetActive(false);
                PowerButtonSelector.instance.unselectAll();
                info_displayer.SetActive(true);
                special_container.SetActive(true);
                common_container.SetActive(true);
                map_container.SetActive(true);
                BuildingPlacer.building_to_place = null;
                Code.Main.game.input_controller.disable();
                info_displayer.GetComponent<InfoDisplayer>().hide();
                special_container.GetComponent<ButtonContainerController>().switch_to_container(diplomacy.name);
                common_container.GetComponent<ButtonContainerController>().switch_to_container(diplomacy.name);
            }));


            List<string> container_ids = new List<string>() { building.name, units.name, diplomacy.name };
            special_container = new GameObject("Special Buttons Containers", typeof(ButtonContainerController));
            special_container.transform.SetParent(this.transform);
            special_container.GetComponent<ButtonContainerController>().init(container_ids);

            common_container = new GameObject("Common Buttons Containers", typeof(Image), typeof(ButtonContainerController));
            common_container.GetComponent<Image>().sprite = Resources.Load<Sprite>("gt_windows/bottom_bar");    
            common_container.transform.SetParent(this.transform);
            common_container.transform.localPosition = new Vector3(0, -430);
            common_container.GetComponent<RectTransform>().sizeDelta = new Vector2(2000, 200);
            common_container.GetComponent<ButtonContainerController>().init(container_ids);

            info_displayer = new GameObject("Info Displayer", typeof(Image), typeof(InfoDisplayer));
            info_displayer.transform.SetParent(this.transform);
            info_displayer.GetComponent<Image>().sprite = Helper.get_window_bg();
            info_displayer.transform.localPosition = new Vector3(-781f, -400);
            info_displayer.transform.localScale = Vector3.one;
            info_displayer.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 300);
            info_displayer.GetComponent<InfoDisplayer>().init();
            add_units_buttons();
            add_buildings_buttons();
            add_diplomacy_buttons();

            map_container = new GameObject("Map Container");
            map_container.transform.SetParent(this.transform);
        }
        public void update(float elapsed)
        {
            if (Game.instance.input_controller.curr_main_object == null || !Game.instance.input_controller.curr_main_object.isAlive()) return;
            update_common_container(Game.instance.input_controller.curr_main_object);
            update_special_container(Game.instance.input_controller.curr_main_object);
        }

        private void add_diplomacy_buttons()
        {
            //throw new NotImplementedException();
        }

        private void add_buildings_buttons()
        {
            #region 初次启动时，建筑页表(C.first_in)，各个种族的hall_1
            foreach (BuildingAsset building in AssetManager.buildings.list)
            {
                if (building.type != SB.type_hall) continue;
                if (building.upgradeLevel != 1) continue;
                GameObject main_base = Helper.create_button_with_bg("Main Base for "+building.race, building.sprites.animationData[0].list_main[0],
                    new UnityEngine.Events.UnityAction(() =>
                    {
                        if (Game.instance.player != null && Game.instance.player.isAlive()) { BuildingPlacer.building_to_place = null; return; }

                        BuildingAsset building_asset = building;
                        BuildingPlacer.building_to_place = BuildingPlacer.building_to_place == building_asset ? null : building_asset;
                    }), C.wait);
                add_building_button(C.first_in, main_base);
            }
            #endregion
            #region 选中Hall时各个种族的市民生成，建筑页表(Hall_{race_name})
            foreach (ActorAsset actor_asset in AssetManager.actor_library.list)
            {
                if (!actor_asset.unit || actor_asset.baby) continue;
                Race race = AssetManager.raceLibrary.get(actor_asset.race);
                if (race == null || !race.civilization) continue;

                GameObject citizen_produce = Helper.create_button_with_bg("Citizen Produce for " + actor_asset.id, SpriteTextureLoader.getSprite("ui/icons/"+actor_asset.icon),
                    new UnityEngine.Events.UnityAction(() =>
                    {
                        if (Game.instance.input_controller.curr_main_object == null || Game.instance.input_controller.curr_main_object.objectType != MapObjectType.Building) return;
                        foreach(BaseSimObject obj in Game.instance.input_controller.selector.active_objects)
                        {
                            if (obj.objectType != MapObjectType.Building || obj.b.asset != Game.instance.input_controller.curr_main_object.b.asset) continue;

                            obj.b.addProgress(C.progress_spawn, actor_asset.id);
                        }
                    }), C.wait);
                add_building_button("Hall_"+actor_asset.race, citizen_produce);
            }
            #endregion
        }

        private void add_units_buttons()
        {
            GameObject wait_for_cmd = Helper.create_button_with_bg("Wait For CMD", "gt_windows/wait_for_cmd", new UnityEngine.Events.UnityAction(() => 
            {
                if (Game.instance.input_controller.selector.type_to_select == MapObjectType.Building) return;
                foreach(BaseSimObject obj in Game.instance.input_controller.selector.active_objects)
                {
                    obj.a.cancelAllBeh(null);
                    obj.a.ai.setJob(C.wait);
                }
            }), C.wait);
            add_unit_button(C._default, wait_for_cmd);
            GameObject explore = Helper.create_button_with_bg("Explore", "gt_windows/move_to_point", new UnityEngine.Events.UnityAction(() =>
            {
                if (Game.instance.input_controller.selector.type_to_select == MapObjectType.Building) return;
                foreach (BaseSimObject obj in Game.instance.input_controller.selector.active_objects)
                {
                    obj.a.cancelAllBeh(null);
                    obj.a.ai.setJob(C.explore);
                }
            }), C.explore);
            add_unit_button(C._default, explore);

            GameObject house_0 = Helper.create_button_with_bg("Tent0", AssetManager.buildings.get(SB.tent_human).sprites.animationData[0].list_main[0],
                new UnityEngine.Events.UnityAction(() =>
                {
                    if (Game.instance.player == null || !Game.instance.player.isAlive()) return;
                    if (Game.instance.input_controller.selector.type_to_select == MapObjectType.Building) return;

                    BuildingAsset building_asset = AssetManager.buildings.get(SB.tent_human);
                    if (BuildingPlacer.building_to_place == building_asset)
                    {
                        BuildingPlacer.building_to_place = null;
                    }
                    else
                    {
                        BuildingPlacer.building_to_place = building_asset;
                    }
                }), C.wait);
            add_unit_button(C._default, house_0);
        }

        public void active()
        {
            info_displayer.SetActive(false);
            special_container.SetActive(false);
            common_container.SetActive(false);
            map_container.SetActive(false);
        }
        private void add_unit_button(string page, GameObject button_obj) { common_container.GetComponent<ButtonContainerController>().add_button( "Units", page, button_obj); }
        private void add_building_button(string page, GameObject button_obj) { common_container.GetComponent<ButtonContainerController>().add_button( "Buildings", page, button_obj); }
        private void add_diplomacy_button(string page, GameObject button_obj) { common_container.GetComponent<ButtonContainerController>().add_button( "Diplomacy", page, button_obj); }

        private void update_special_container(BaseSimObject curr_main_object)
        {
            //throw new NotImplementedException();
        }

        private void update_common_container(BaseSimObject curr_main_object)
        {
            if(curr_main_object.objectType == MapObjectType.Building)
            {
                switch (curr_main_object.b.asset.type)
                {
                    case nameof(SB.type_hall):
                        common_container.GetComponent<ButtonContainerController>().switch_to_page("Hall_" + curr_main_object.b.asset.race);
                        break;
                }
            }
            else
            {

            }
        }
    }
}
