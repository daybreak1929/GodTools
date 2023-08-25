using System;
using System.Reflection;
using HarmonyLib;

namespace GodTools.Code.HarmonySpace;

internal static class Manager
{
    public static void init()
    {
        Type[] all_types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (Type type in all_types)
        {
            if (type.Namespace != $"{nameof(GodTools)}.{nameof(Code)}.{nameof(HarmonySpace)}") continue;

            if (type.Name.StartsWith("H_"))
            {
                Harmony.CreateAndPatchAll(
                    type, C.PATCH_ID
                );
            }
        }
    }
}