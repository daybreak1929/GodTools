using System;
using System.Collections.ObjectModel;

namespace GodTools.Abstract;

public class DependencyAttribute : Attribute
{
    public DependencyAttribute(params Type[] types)
    {
        Types = Array.AsReadOnly(types);
    }

    public ReadOnlyCollection<Type> Types { get; }
}