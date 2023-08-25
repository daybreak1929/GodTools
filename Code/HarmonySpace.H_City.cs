using HarmonyLib;

namespace GodTools.Code.HarmonySpace;

internal static class H_City
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(City),nameof(City.getPopulationTotal))]
    public static void add_player_to_pop(City __instance, ref int __result)
    {
        if (__instance.kingdom.cities.Count == 1) __result++;
    }
}