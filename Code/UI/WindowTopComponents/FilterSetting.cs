using System;
using UnityEngine;

namespace GodTools.UI.WindowTopComponents;

public class FilterSetting
{
    public readonly Func<Actor, bool> FilterFunc;
    public readonly string ID;
    public readonly Sprite Icon;
    public Sprite InnerIcon;
    public Color IconColor = Color.white;
    public Color InnerColor = Color.white;
    public FilterType Type;
    public bool ToBeRemoved { get; internal set; }

    public FilterSetting()
    {
        
    }
    public FilterSetting(string id, Sprite icon, Func<Actor, bool> filter_func)
    {
        ID = id;
        Icon = icon;
        FilterFunc = filter_func;
    }
    public override bool Equals(object obj)
    {
        return obj is FilterSetting setting && ID == setting.ID;
    }
    
    public override int GetHashCode()
    {
        return ID.GetHashCode();
    }
}