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

    public static Dictionary<string, SavedActorPacket> SavedActors;
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
        SavedActors[ParseIDforActorData(actor.data)] = new SavedActorPacket()
        {
            ActorData = actor.data.Copy(),
            ClanData = actor.clan?.data.Copy(),
            CultureData = actor.culture?.data.Copy(),
            LanguageData = actor.language?.data.Copy(),
            SubspeciesData = actor.subspecies?.data.Copy()
        };

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
            SavedActors = JsonConvert.DeserializeObject<Dictionary<string, SavedActorPacket>>(File.ReadAllText(_save_path));
        }
        catch (Exception)
        {
            SavedActors = new Dictionary<string, SavedActorPacket>();
        }
        SavedActors ??= new Dictionary<string, SavedActorPacket>();
    }

    private static string _selected_id;

    public class SavedActorPacket
    {
        public ActorData ActorData;
        public ClanData ClanData;
        public CultureData CultureData;
        public LanguageData LanguageData;
        public SubspeciesData SubspeciesData;
    }

    public static void SelectActorToPlace(ActorData data)
    {
        _selected_id = ParseIDforActorData(data);
    }
    public static bool SpawnSelectedSavedActor(WorldTile tile, string power_id)
    {
        var power_data = SavedActors[_selected_id];
        var subspecies_data = power_data.SubspeciesData.Copy();
        var language_data = power_data.LanguageData.Copy();
        var culture_data = power_data.CultureData.Copy();
        var clan_data = power_data.ClanData.Copy();
        var data = power_data.ActorData.Copy();

        if (subspecies_data != null)
        {
            subspecies_data.id = World.world.map_stats.getNextId("subspecies");
            subspecies_data.created_time = World.world.map_stats.world_time;
            data.subspecies = subspecies_data.id;
            World.world.subspecies.loadObject(subspecies_data);
        }

        if (language_data != null)
        {
            language_data.id = World.world.map_stats.getNextId("language");
            language_data.created_time = World.world.map_stats.world_time;
            data.language = language_data.id;
            World.world.languages.loadObject(language_data);
        }

        if (culture_data != null)
        {
            culture_data.id = World.world.map_stats.getNextId("culture");
            culture_data.created_time = World.world.map_stats.world_time;
            data.culture = culture_data.id;
            World.world.cultures.loadObject(culture_data);
        }

        if (clan_data != null)
        {
            clan_data.id = World.world.map_stats.getNextId("clan");
            clan_data.created_time = World.world.map_stats.world_time;
            data.clan = clan_data.id;
            World.world.clans.loadObject(clan_data);
        }

        data.id = World.world.map_stats.getNextId("unit");


        if (World.world.cities.get(data.cityID) == null) data.cityID = -1;

        data.homeBuildingID = -1;
        data.transportID = -1;
        data.x = tile.pos.x;
        data.y = tile.pos.y;
        World.world.units.loadObject(data);
        return true;
    }
}