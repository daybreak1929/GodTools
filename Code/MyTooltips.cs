using System.Collections.Generic;
using NCMS.Utils;
using Newtonsoft.Json;
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
            asset.callback = (tooltip, type, data) =>
            {
                tooltip.name.text = LocalizedTextManager.getText(data.tip_name);
                tooltip.addDescription(LocalizedTextManager.getText(data.tip_description));
                
                Dictionary<string, int> resources_cost = JsonConvert.DeserializeObject<Dictionary<string, int>>(data.tip_description_2);

                if (resources_cost == null)
                {
                    return;
                }
                foreach (string resource_key in resources_cost.Keys)
                {
                    if (resources_cost[resource_key] == 0) continue;
                    ResourceAsset resource_asset = AssetManager.resources.get(resource_key);
                    tooltip.addLineText(LocalizedTextManager.getText(resource_asset.id), resources_cost[resource_key].ToString());
                }
                
            };
            add(asset, Prefabs.action_with_cost_tooltip_prefab);
        }

        private static void add(TooltipAsset asset, Object prefab)
        {
            AssetManager.tooltips.add(asset);
            resources_dict[asset.prefab_id] = prefab;
        }
    }
}