using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace GodTools.Utils;

public static class DataCopyTools
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static TData CopyGeneral<TData>(this TData data) where TData : BaseSystemData
    {
        if (data is ActorData actor_data)
            return actor_data.Copy() as TData;
        if (data is ItemData item_data)
            return item_data.Copy() as TData;
        if (data is CultureData culture_data)
            return culture_data.Copy() as TData;
        if (data is LanguageData language_data)
            return language_data.Copy() as TData;
        if (data is ClanData clan_data)
            return clan_data.Copy() as TData;
        if (data is SubspeciesData subspecies_data)
            return subspecies_data.Copy() as TData;
        
        var copied_data = JsonConvert.DeserializeObject<TData>(JsonConvert.SerializeObject(data, Formatting.None, GeneralTools.private_members_visit_settings), GeneralTools.private_members_visit_settings);
        if (copied_data.custom_data_bool?.dict == null) copied_data.custom_data_bool = null;

        if (copied_data.custom_data_float?.dict == null) copied_data.custom_data_float = null;

        if (copied_data.custom_data_int?.dict == null) copied_data.custom_data_int = null;

        if (copied_data.custom_data_string?.dict == null) copied_data.custom_data_string = null;
        
        if (copied_data.custom_data_long?.dict == null) copied_data.custom_data_long = null;

        return copied_data;
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static ActorData Copy(this ActorData data)
    {
        if (data == null) return null;
        var copied_data = JsonConvert.DeserializeObject<ActorData>(JsonConvert.SerializeObject(data, Formatting.None, GeneralTools.private_members_visit_settings), GeneralTools.private_members_visit_settings);
        if (copied_data.custom_data_bool?.dict == null) copied_data.custom_data_bool = null;

        if (copied_data.custom_data_float?.dict == null) copied_data.custom_data_float = null;

        if (copied_data.custom_data_int?.dict == null) copied_data.custom_data_int = null;

        if (copied_data.custom_data_string?.dict == null) copied_data.custom_data_string = null;
        
        if (copied_data.custom_data_long?.dict == null) copied_data.custom_data_long = null;

        return copied_data;
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static ItemData Copy(this ItemData data)
    {
        if (data == null) return null;
        var copied_data = JsonConvert.DeserializeObject<ItemData>(JsonConvert.SerializeObject(data, Formatting.None, GeneralTools.private_members_visit_settings), GeneralTools.private_members_visit_settings);
        if (copied_data.custom_data_bool?.dict == null) copied_data.custom_data_bool = null;

        if (copied_data.custom_data_float?.dict == null) copied_data.custom_data_float = null;

        if (copied_data.custom_data_int?.dict == null) copied_data.custom_data_int = null;

        if (copied_data.custom_data_string?.dict == null) copied_data.custom_data_string = null;
        
        if (copied_data.custom_data_long?.dict == null) copied_data.custom_data_long = null;

        return copied_data;
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static CultureData Copy(this CultureData data)
    {
        if (data == null) return null;
        var copied_data = JsonConvert.DeserializeObject<CultureData>(JsonConvert.SerializeObject(data, Formatting.None, GeneralTools.private_members_visit_settings), GeneralTools.private_members_visit_settings);
        if (copied_data.custom_data_bool?.dict == null) copied_data.custom_data_bool = null;

        if (copied_data.custom_data_float?.dict == null) copied_data.custom_data_float = null;

        if (copied_data.custom_data_int?.dict == null) copied_data.custom_data_int = null;

        if (copied_data.custom_data_string?.dict == null) copied_data.custom_data_string = null;
        
        if (copied_data.custom_data_long?.dict == null) copied_data.custom_data_long = null;

        return copied_data;
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static ClanData Copy(this ClanData data)
    {
        if (data == null) return null;
        var copied_data = JsonConvert.DeserializeObject<ClanData>(JsonConvert.SerializeObject(data, Formatting.None, GeneralTools.private_members_visit_settings), GeneralTools.private_members_visit_settings);
        if (copied_data.custom_data_bool?.dict == null) copied_data.custom_data_bool = null;

        if (copied_data.custom_data_float?.dict == null) copied_data.custom_data_float = null;

        if (copied_data.custom_data_int?.dict == null) copied_data.custom_data_int = null;

        if (copied_data.custom_data_string?.dict == null) copied_data.custom_data_string = null;
        
        if (copied_data.custom_data_long?.dict == null) copied_data.custom_data_long = null;

        return copied_data;
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static LanguageData Copy(this LanguageData data)
    {
        if (data == null) return null;
        var copied_data = JsonConvert.DeserializeObject<LanguageData>(JsonConvert.SerializeObject(data, Formatting.None, GeneralTools.private_members_visit_settings), GeneralTools.private_members_visit_settings);
        if (copied_data.custom_data_bool?.dict == null) copied_data.custom_data_bool = null;

        if (copied_data.custom_data_float?.dict == null) copied_data.custom_data_float = null;

        if (copied_data.custom_data_int?.dict == null) copied_data.custom_data_int = null;

        if (copied_data.custom_data_string?.dict == null) copied_data.custom_data_string = null;
        
        if (copied_data.custom_data_long?.dict == null) copied_data.custom_data_long = null;

        return copied_data;
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static SubspeciesData Copy(this SubspeciesData data)
    {
        if (data == null) return null;
        var copied_data = JsonConvert.DeserializeObject<SubspeciesData>(JsonConvert.SerializeObject(data, Formatting.None, GeneralTools.private_members_visit_settings), GeneralTools.private_members_visit_settings);
        if (copied_data.custom_data_bool?.dict == null) copied_data.custom_data_bool = null;

        if (copied_data.custom_data_float?.dict == null) copied_data.custom_data_float = null;

        if (copied_data.custom_data_int?.dict == null) copied_data.custom_data_int = null;

        if (copied_data.custom_data_string?.dict == null) copied_data.custom_data_string = null;
        
        if (copied_data.custom_data_long?.dict == null) copied_data.custom_data_long = null;

        return copied_data;
    }
    
}