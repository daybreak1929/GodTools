using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GodTools.Abstract;
using HarmonyLib;
using NeoModLoader.api.attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace GodTools.Features;

public class DetailedHistory : IManager
{
    public static List<WorldLogMessage> Messages = new();
    
    [HarmonyPostfix, HarmonyPatch(typeof(WorldLogMessageExtensions), nameof(WorldLogMessageExtensions.add))]
    private static void WorldLogMessageExtensions_add_postfix(WorldLogMessage pMessage)
    {
        Messages.Add(pMessage);
    }
    struct WorldLogMessageData
    {
        public string text;
        public string special1;
        public string special2;
        public string special3;
        public Color color_special1;
        public Color color_special2;
        public Color color_special3;
        public string date;
        public string icon;
        public string city;
        public string kingdom;
        public string alliance;
        public string unit;
        public Vector3 location;
    }
    public class FieldsOnlyContractResolver : DefaultContractResolver
    {
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            // 仅包含字段
            return objectType.GetFields(BindingFlags.Public | BindingFlags.Instance).Cast<MemberInfo>().ToList();
        }
    }
    [Hotfixable]
    [HarmonyPostfix, HarmonyPatch(typeof(SaveManager), nameof(SaveManager.saveMapData))]
    private static void SaveManager_saveMapData_postfix(string pFolder)
    {
        var data = new List<WorldLogMessageData>(Messages.Count);
        foreach (var msg in Messages)
        {
            data.Add(new WorldLogMessageData
            {
                text = msg.text,
                special1 = msg.special1,
                special2 = msg.special2,
                special3 = msg.special3,
                color_special1 = msg.color_special1,
                color_special2 = msg.color_special2,
                color_special3 = msg.color_special3,
                date = msg.date,
                icon = msg.icon,
                city = msg.city?.data?.id,
                kingdom = msg.kingdom?.data?.id,
                alliance = msg.alliance?.data?.id,
                unit = msg.unit?.data?.id,
                location = msg.location
            });
        }
        var folder = SaveManager.folderPath(pFolder);
        var file = folder + "/worldlog.json";
        
        Main.LogInfo($"Saving world log to {file}");
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ContractResolver = new FieldsOnlyContractResolver(),
            Formatting = Formatting.None
        };
        File.WriteAllText(file, JsonConvert.SerializeObject(data, settings));
    }

    public void Initialize()
    {
        Harmony.CreateAndPatchAll(typeof(DetailedHistory));
    }
}