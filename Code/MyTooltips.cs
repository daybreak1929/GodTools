using System.Collections.Generic;
using NCMS.Utils;
using ReflectionUtility;
using UnityEngine;

namespace GodTools.Code
{
    internal static class MyTooltips
    {
        private static Dictionary<string, Object> resources_dict;
        public static void init()
        {
            resources_dict = Reflection.GetField(typeof(ResourcesPatch), null, "modsResources") as Dictionary<string, Object>;

            TooltipAsset asset;
            asset = new();
            asset.id = C.action_with_cost_tooltip;
            asset.prefab_id = $"tooltips/{asset.id}";
            add(asset, Prefabs.action_with_cost_tooltip_prefab);
        }

        private static void add(TooltipAsset asset, Object prefab)
        {
            AssetManager.tooltips.add(asset);
            resources_dict[asset.prefab_id] = prefab;
        }
    }
}