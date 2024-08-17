using System.Reflection;
using HarmonyLib;

namespace GodTools.HarmonySpace;

internal static class Manager
{
    public static void init()
    {
        var all_types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (var type in all_types)
        {
            if (type.Namespace != $"{nameof(GodTools)}.{nameof(HarmonySpace)}") continue;

            if (type.Name.StartsWith("H_"))
                Harmony.CreateAndPatchAll(
                    type, C.PATCH_ID
                );
        }
    }
}