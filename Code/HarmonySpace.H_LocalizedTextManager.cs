using HarmonyLib;

namespace GodTools.Code.HarmonySpace;

internal static class H_LocalizedTextManager
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(LocalizedTextManager), nameof(LocalizedTextManager.setLanguage))]
    public static void setLanguage_postfix(string pLanguage)
    {
        MyLocalizedTextManager.apply_localization(LocalizedTextManager.instance.localizedText, pLanguage);
    }
}