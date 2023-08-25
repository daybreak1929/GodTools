using HarmonyLib;

namespace GodTools.Code.HarmonySpace;

internal static class H_WindowCreatureInfo
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(WindowCreatureInfo), "OnEnable")]
    public static void item_editor_init(WindowCreatureInfo __instance)
    {
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
}