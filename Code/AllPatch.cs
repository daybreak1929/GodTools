using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using GodTools.Extension;
namespace GodTools.Code
{
    internal static class AllPatch
    {
        public static void patch_all(string id)
        {
            Harmony.CreateAndPatchAll(typeof(AllPatch), id);
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PowerButton), nameof(PowerButton.unselectActivePower))]
        public static void post_poweraction(PowerButton __instance)
        {
            PowerButtonClickAction action;
            if(MyPowers.post_actions.TryGetValue(__instance.godPower.id, out action))
            {
                action(__instance.godPower.id);
            }
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MapBox),nameof(MapBox.canInspectWithRightClick))]
		public static bool is_action_happening_prefix(MapBox __instance, ref bool __result)
        {
			__result = Input.mousePresent && __instance.isAnyPowerSelected() && __instance.selectedButtons.selectedButton.godPower.allow_unit_selection && Input.GetMouseButtonUp(1);
			return false;
		}
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MoveCamera), nameof(MoveCamera.updateMouseCameraDrag))]
        public static bool fix_mouse_drag(MoveCamera __instance)
        {
            __fix_mouse_drag(__instance);
            return false;
        }

        private static void __fix_mouse_drag(MoveCamera instance)
        {
			MoveCamera.cameraDragRun = false;
			bool flag = false;
			bool flag2 = false;
			if (Input.mousePresent)
			{
				if (World.world.isAnyPowerSelected())
				{
					flag = Input.GetMouseButtonDown(2);
					flag2 = Input.GetMouseButton(2);
				}
				else
				{
					flag = Input.GetMouseButtonDown(2);
					flag2 = Input.GetMouseButton(2);
				}
			}
			if (!flag2)
			{
				instance.clearTouches();
				return;
			}
			if (flag && World.world.isOverUI(true))
			{
				instance.clearTouches();
				return;
			}
			if (flag && instance.Origin.x == -1f && instance.Origin.z == -1f)
			{
				instance.Origin = instance.MousePos();
			}
			if (instance.Origin.x == -1f && instance.Origin.y == -1f && instance.Origin.z == -1f)
			{
				return;
			}
			if (flag2)
			{
				MoveCamera.cameraDragRun = true;
				instance.Difference = instance.MousePos() - instance.transform.position;
				if (Toolbox.DistVec3(instance.Origin, instance.MousePos()) > 0.1f)
				{
					MoveCamera.cameraDragActivated = true;
				}
				Vector3 vector = instance.Origin - instance.Difference;
				if (Input.touchSupported)
				{
					MoveCamera.distDebug = Toolbox.DistVec3(instance.firstTouch, instance.TouchPos(true));
					if (World.world.touchTimer > 0.06f)
					{
						if (MoveCamera.distDebug >= 20f || World.world.touchTimer > 0.3f)
						{
							World.world.alreadyUsedZoom = true;
							World.world.alreadyUsedPower = false;
						}
					}
					else if (Input.touchCount == 1)
					{
						return;
					}
				}
				MoveCamera.debugDragDiff = Toolbox.DistVec3(instance.transform.position, vector) / instance.mainCamera.orthographicSize;
				if (Toolbox.DistVec3(instance.transform.position, vector) > 3f)
				{
					instance.setWhooshState(WhooshState.NeedWhoosh);
				}
				if (!Input.mousePresent)
				{
					return;
				}
				instance.transform.position = vector;
				MoveCamera.cameraDragNew = vector.ToString();
				instance.cameraToBounds();
			}
		}
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MapIconLibrary), nameof(MapIconLibrary.drawCursorSprite))]
		public static bool draw_more_icons_under_cursor(MapIconAsset pAsset)
		{
			if (!Input.mousePresent || Input.GetMouseButton(0) || Input.GetMouseButton(1))
			{
				return false;
			}
			if (World.world.isBusyWithUI()|| MoveCamera.inSpectatorMode()||Config.controllingUnit)
			{
				return false;
			}
            if (Main.game.input_controller.sprite_under_cursor == null)
            {
				return true;
            }
			Vector2 mousePos = World.world.getMousePos();
			mousePos.x += -0.3f * MapIconLibrary.getCameraScaleZoom(pAsset);
			mousePos.y += -0.3f * MapIconLibrary.getCameraScaleZoom(pAsset);
			Input.GetMouseButton(0);
			MapMark mapMark = MapIconLibrary.drawMark(pAsset, mousePos, null, null, null, null, 1f, false);
			mapMark.spriteRenderer.sprite = Main.game.input_controller.sprite_under_cursor;
			mousePos.x += 0.3f * MapIconLibrary.getCameraScaleZoom(pAsset);
			mousePos.y += 0.3f * MapIconLibrary.getCameraScaleZoom(pAsset);
			Color color_black = Toolbox.color_black;
			mapMark.spriteRenderer.sprite = Main.game.input_controller.sprite_under_cursor;
			color_black.a = 0.3f;
			mapMark.spriteRenderer.color = color_black;
			mapMark.spriteRenderer.sortingOrder = 9;
			MapMark mapMark2 = MapIconLibrary.drawMark(pAsset, mousePos, null, null, null, null, 1f, false);
			mapMark2.spriteRenderer.sprite = Main.game.input_controller.sprite_under_cursor;
			mapMark2.spriteRenderer.sortingOrder = 10;
			return false;
		}
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BatchBuildings),nameof(BatchBuildings.updateStatusEffects))]
		public static void update_progress(BatchBuildings __instance)
        {
			if (__instance._list == null) return;
			for(int i = 0; i < __instance._list.Count; i++)
            {
				Building building = __instance._list[i];
				if (!building.data.hasFlag(C.flag_in_progress)) continue;
				building.update_progress();
            }
        }
    }
}
