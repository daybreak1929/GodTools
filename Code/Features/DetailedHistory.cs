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
        var data = new List<WorldLogMessage>(Messages.Count);
        foreach (var msg in Messages)
        {
            data.Add(msg);
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