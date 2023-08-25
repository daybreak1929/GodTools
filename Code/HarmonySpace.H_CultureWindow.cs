using HarmonyLib;

namespace GodTools.Code.HarmonySpace;

internal static class H_CultureWindow
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CultureWindow), "OnEnable")]
    public static void culture_tech_editor_init(CultureWindow __instance)
    {
        WindowCultureTechEditor.init(__instance);
    }
}