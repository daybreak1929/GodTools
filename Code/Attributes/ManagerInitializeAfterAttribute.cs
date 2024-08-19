using System;
using System.Collections.Generic;
using GodTools.Abstract;

namespace GodTools.Attributes;

internal class ManagerInitializeAfterAttribute : Attribute
{
    public readonly HashSet<Type> after_types = new();

    public ManagerInitializeAfterAttribute(params Type[] types)
    {
        foreach (Type type in types)
        {
            if (!typeof(IManager).IsAssignableFrom(type))
            {
                Main.LogInfo($"Type {type} is not a manager type.");
                continue;
            }

            if (type.IsInterface)
            {
                Main.LogInfo($"Type {type} is an interface type.");
                continue;
            }

            after_types.Add(type);
        }
    }
}