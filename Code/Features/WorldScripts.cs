using System;
using System.Collections.Generic;
using System.IO;
using GodTools.Abstract;
using GodTools.Features.WorldScript;
using Newtonsoft.Json;
using UnityEngine;

namespace GodTools.Features;

public class WorldScripts : IManager
{
    private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "saved_scripts.json");
    public static List<ScriptInstance> Scripts;
    public void Initialize()
    {
        try
        {
            Scripts = JsonConvert.DeserializeObject<List<ScriptInstance>>(File.ReadAllText(SavePath));
        }
        catch (Exception)
        {
            Scripts = new ();
        }
        Scripts ??= new ();
    }
}