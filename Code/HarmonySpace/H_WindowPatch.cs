using GodTools.UI;
using HarmonyLib;
using NeoModLoader.General;
using UnityEngine;

namespace GodTools.HarmonySpace;

internal static class H_WindowPatch
{
    private static bool initialized;

    private static float _last_y = 84;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(WindowCreatureInfo), "OnEnable")]
    public static void item_editor_init(WindowCreatureInfo __instance)
    {
        if (!initialized)
        {
            initialized = true;
            AddEntryButtonForWindow(__instance, nameof(WindowCreatureSpriteEditor));
        }

        WindowStatusEffectEditor.init(__instance);
        if (Config.selectedUnit.asset.use_items)
        {
            WindowItemEditor.init(__instance);
            WindowItemEditor.entry_button.SetActive(true);
        }
        else if (WindowItemEditor.entry_button != null)
        {
            WindowItemEditor.entry_button.SetActive(false);
        }
    }

    private static void AddEntryButtonForWindow(WindowCreatureInfo target_window, string entry_window_id)
    {
        PowerButtonCreator.CreateWindowButton(entry_window_id, entry_window_id,
                                              SpriteTextureLoader.getSprite("ui/Icons/iconAttractive"),
                                              target_window.transform.Find("Background"), new Vector2(156, _last_y));
        _last_y -= 40;
    }
}