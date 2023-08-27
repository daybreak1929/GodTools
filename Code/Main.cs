using System;
using NCMS;
using UnityEngine;
using ReflectionUtility;
using HarmonyLib;
using UnityEngine.UI;
using UnityEngine.Events;
using GodTools.Game;

namespace GodTools.Code
{
    public class Mod
    {
        public static ModDeclaration.Info Info;
        public static GameObject GameObject;
    }
    [ModEntry]
    class Main : MonoBehaviour{
        private bool initialized = false;
        private PosShowEffect last_pos_show;

        public PosShowEffect actor_select_effect;
        public static bool CW_loaded = false;
        public static BuildingProgressLibrary building_progresses;
        public static Main instance;
        public static Transform prefab_library;
        public static Transform game_ui_object_temp_library;
        public static Game.Game game = new Game.Game();
        public SimpleEffectController<PosShowEffect> pos_show_effect_controller = new SimpleEffectController<PosShowEffect>(Resources.Load<GameObject>("effects/PrefabUnitSelectionEffect"));
        public static void warn(string str)
        {
            UnityEngine.Debug.LogWarning($"[{Mod.Info.Name}]:{str}");
        }
        public static void log(string str)
        {
            UnityEngine.Debug.Log($"[{Mod.Info.Name}]:{str}");
        }
        void Update()
        {
            if (!initialized)
            {
                if(GameObject.Find("/Canvas Container Main/Canvas - Windows/windows/welcome/Background/BottomBg")==null) return;
                initialized = true;
                instance = this;
                prefab_library = new GameObject("Prefabs").transform;
                prefab_library.SetParent(transform);
                prefab_library.localPosition = new(9999999,9999999,0);
                game_ui_object_temp_library = new GameObject("Game UI Objects").transform;
                game_ui_object_temp_library.SetParent(transform);
                game_ui_object_temp_library.localPosition = new(9999999, 9999999, 0);
                
                foreach(NCMod mod in NCMS.ModLoader.Mods)
                {
                    if(mod.name == C.MOD_NAME)
                    {
                        Mod.Info = (ModDeclaration.Info)AccessTools.Constructor(typeof(ModDeclaration.Info), new Type[] { typeof(NCMod) }, false).Invoke(new object[] { mod });
                        Mod.GameObject = this.gameObject;
                    }
                    else if (mod.name == C.CW_MOD_NAME)
                    {
                        CW_loaded = true;
                    }
                }

                AllPatch.patch_all(C.PATCH_ID);
                HarmonySpace.Manager.init();
                MyLocalizedTextManager.init();
                MyLocalizedTextManager.apply_localization(LocalizedTextManager.instance.localizedText, LocalizedTextManager.instance.language);
                Prefabs.init();
                MyActorJobs.init();
                MyActorTasks.init();
                MyCityJobs.init();
                MyKingdomJobs.init();
                MyPowers.init();
                MyStatusEffects.init();
                MyTooltips.init();
                MyTab.create_tab();
                MyTab.add_buttons();
                MyTab.apply_buttons();
                game.init();
                building_progresses = new BuildingProgressLibrary();
                AssetManager.instance.add(building_progresses, nameof(building_progresses));
            }
            game.update(Time.fixedDeltaTime);
            pos_show_effect_controller.update(Time.fixedDeltaTime * C.pos_show_effect_time_scale);
        }
    }
}