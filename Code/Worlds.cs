using System.Collections.Generic;
using GodTools.Features;

namespace GodTools;

public static class Worlds
{
    public static MapBox MajorWorld => World.world;

    public static Dictionary<string, SubWorld> SubWorlds { get; } = new();
}