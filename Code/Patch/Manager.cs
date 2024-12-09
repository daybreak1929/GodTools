using System.Reflection;
using GodTools.Abstract;
using HarmonyLib;

namespace GodTools.Patch;

internal class Manager : IManager
{
    public void Initialize()
    {
        var all_types = Assembly.GetExecutingAssembly().GetTypes();
        var ns = GetType().Namespace ?? "";
        foreach (var type in all_types)
        {
            if (type.Namespace?.StartsWith(ns) ?? true) continue;

            if (type.Name.StartsWith("Patch"))
                Harmony.CreateAndPatchAll(
                    type, C.mod_prefix
                );
        }
    }
}