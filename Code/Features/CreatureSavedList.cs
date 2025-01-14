using System;
using System.Collections.Generic;
using System.IO;
using GodTools.Abstract;
using GodTools.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace GodTools.Features;

public class CreatureSavedList : IManager
{
    private static readonly string _save_path = Path.Combine(Application.persistentDataPath, "saved_actors.json");

    public static Dictionary<string, ActorData> SavedActors;
    private static string ParseIDforActorData(ActorData data)
    {
        return JsonConvert.SerializeObject(data).GetHashCode().ToString();
    }

    public static bool ActorSaved(Actor actor)
    {
        actor.prepareForSave();
        return SavedActors.ContainsKey(ParseIDforActorData(actor.data));
    }

    public static void SaveActor(Actor actor)
    {
        actor.prepareForSave();
        SavedActors[ParseIDforActorData(actor.data)] = ActorTools.Copy(actor.data);

        File.WriteAllText(_save_path, JsonConvert.SerializeObject(SavedActors));
    }

    public static void UnsaveActor(Actor actor)
    {
        actor.prepareForSave();
        SavedActors.Remove(ParseIDforActorData(actor.data));
        File.WriteAllText(_save_path, JsonConvert.SerializeObject(SavedActors));
    }

    public static void UnsaveActorData(ActorData data)
    {
        SavedActors.Remove(ParseIDforActorData(data));
        File.WriteAllText(_save_path, JsonConvert.SerializeObject(SavedActors));
    }

    public void Initialize()
    {
        try
        {
            SavedActors = JsonConvert.DeserializeObject<Dictionary<string, ActorData>>(File.ReadAllText(_save_path));
        }
        catch (Exception)
        {
            SavedActors = new Dictionary<string, ActorData>();
        }
        SavedActors ??= new Dictionary<string, ActorData>();
    }
}