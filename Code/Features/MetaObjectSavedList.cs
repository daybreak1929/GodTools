using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GodTools.Abstract;
using GodTools.Utils;
using NeoModLoader.General;
using Newtonsoft.Json;
using strings;
using UnityEngine;

namespace GodTools.Features;

public abstract class MetaObjectSavedList<TList, TObject, TData> : IManager where TData : MetaObjectData, new() where TList : MetaObjectSavedList<TList, TObject, TData> where TObject : MetaObject<TData>, new()
{
    protected static string SavePath =
        Path.Combine(Application.persistentDataPath, $"saved_{typeof(TObject).Name.Underscore()}s.json");

    public static TList Instance { get; private set; }
    protected virtual string GetTypeID()
    {
        return typeof(TObject).Name.Underscore();
    }

    protected abstract DropsAction GetDropAction();
    protected abstract MetaSystemManager<TObject, TData> GetSystemManager();
    protected Dictionary<string, TData> SavedDatas;
    protected PowerButton HiddenSpawnObjectButton;

    public List<TData> GetAll()
    {
        return SavedDatas.Values.ToList();
    }
    protected string ParseIDforData(TData data)
    {
        return JsonConvert.SerializeObject(data).GetHashCode().ToString();
    }

    public bool DataSaved(TData data)
    {
        data.save();
        return SavedDatas.ContainsKey(ParseIDforData(data));
    }

    public void SaveData(TData data)
    {
        data.save();
        SavedDatas[ParseIDforData(data)] = data.CopyGeneral();
        File.WriteAllText(SavePath, JsonConvert.SerializeObject(SavedDatas));
    }

    public void UnsaveData(TData data)
    {
        data.save();
        SavedDatas.Remove(ParseIDforData(data));
        File.WriteAllText(SavePath, JsonConvert.SerializeObject(SavedDatas));
    }

    public void SelectToPlace(TData data)
    {
        SelectedData = data;
        HiddenSpawnObjectButton.clickActivePower();
    }

    protected TData SelectedData { get; private set; }

    private Tuple<TData, TObject> _last_data = null;
    public TObject GetObject(TData data)
    {
        if (_last_data?.Item1 == data)
        {
            return _last_data?.Item2;
        }
        var new_data = data.CopyGeneral();
        new_data.id = World.world.map_stats.getNextId(GetTypeID());
        new_data.created_time = World.world.map_stats.world_time;
        var obj = GetSystemManager().loadObject(new_data);
        
        _last_data = new Tuple<TData, TObject>(data, obj);
        return obj;
    }
    
    
    public void Initialize()
    {
        try
        {
            SavedDatas = JsonConvert.DeserializeObject<Dictionary<string, TData>>(File.ReadAllText(SavePath));
        }
        catch (Exception)
        {
            SavedDatas = new Dictionary<string, TData>();
        }
        SavedDatas ??= new Dictionary<string, TData>();
        Instance = (TList)this;
        
        var power_id = $"gt_{GetTypeID()}_spawn";
        var power = AssetManager.powers.clone(power_id, PowerLibrary.TEMPLATE_DROPS);
        power.drop_id = power_id;
        var drop = AssetManager.drops.clone(power_id, S_Drop.friendship);
        drop.action_landed = GetDropAction();
        HiddenSpawnObjectButton = PowerButtonCreator.CreateGodPowerButton(power_id, SpriteTextureLoader.getSprite("ui/icons/iconAbout"), Main.prefabs);
    }
}