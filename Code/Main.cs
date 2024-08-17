using System.IO;
using GodTools.Effect;
using GodTools.HarmonySpace;
using GodTools.UI;
using NCMS;
using NeoModLoader.api;
using NeoModLoader.api.attributes;
using NeoModLoader.services;
using UnityEngine;

namespace GodTools;

[ModEntry]
internal class Main : MonoBehaviour, IMod, ILocalizable, IReloadable
{
    public static Main instance;
    public static Transform prefab_library;
    public static Transform game_ui_object_temp_library;

    public PosShowEffect actor_select_effect;

    private bool initialized;
    private PosShowEffect last_pos_show;

    public SimpleEffectController<PosShowEffect> pos_show_effect_controller =
        new(Resources.Load<GameObject>("effects/PrefabUnitSelectionEffect"));

    public static ModDeclare Mod { get; private set; }

    private void Update()
    {
        pos_show_effect_controller.update(Time.fixedDeltaTime * C.pos_show_effect_time_scale);
    }

    public string GetLocaleFilesDirectory(ModDeclare pModDeclare)
    {
        return Path.Combine(pModDeclare.FolderPath, "Locales");
    }

    public ModDeclare GetDeclaration()
    {
        return Mod;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public string GetUrl()
    {
        return Mod.RepoUrl;
    }

    public void OnLoad(ModDeclare pModDecl, GameObject pGameObject)
    {
        Mod = pModDecl;
        instance = this;
        prefab_library = new GameObject("Prefabs").transform;
        prefab_library.SetParent(transform);
        prefab_library.localPosition = new Vector3(9999999, 9999999, 0);
        game_ui_object_temp_library = new GameObject("Game UI Objects").transform;
        game_ui_object_temp_library.SetParent(transform);
        game_ui_object_temp_library.localPosition = new Vector3(9999999, 9999999, 0);

        Manager.init();
        MyPowers.init();
        MyStatusEffects.init();

        WindowModInfo.CreateWindow(C.mod_prefix + nameof(WindowModInfo), nameof(WindowModInfo));
        WindowCreatureSpriteEditor.CreateWindow(nameof(WindowCreatureSpriteEditor), nameof(WindowCreatureSpriteEditor));
        WindowCreatureDataEditor.CreateWindow(nameof(WindowCreatureDataEditor), nameof(WindowCreatureDataEditor));

        MyTab.create_tab();
    }

    public void Reload()
    {
    }

    public static void LogWarning(string str)
    {
        LogService.LogWarning($"[GodTools]:{str}");
    }

    [Hotfixable]
    public static void LogInfo(string str)
    {
        LogService.LogInfo($"[GodTools]:{str}");
    }
}