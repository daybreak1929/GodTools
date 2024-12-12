using System.Collections.Generic;
using UnityEngine;

namespace GodTools.Features;

public class SubWorld : MonoBehaviour
{
    private WorldTile[] _tile_list;

    private WorldTile[,] _tile_map;
    public  string       Guid { get; private set; }
    public  int          Size { get; private set; }

    public static SubWorld Create(string guid, int size)
    {
        var obj = new GameObject($"[SubWorld] {guid}", typeof(SubWorld));

        var subworld = obj.GetComponent<SubWorld>();
        subworld.Guid = guid;
        subworld.Size = size;

        subworld.generate_tiles();
        subworld.create_tile_layers();

        return subworld;
    }

    private void create_tile_layers()
    {
    }

    public WorldTile GetTile(int x, int y)
    {
        if (x < 0 || x >= Size || y < 0 || y >= Size) return null;

        return _tile_map[x, y];
    }

    private void generate_tiles()
    {
        _tile_map = new WorldTile[Size, Size];
        _tile_list = new WorldTile[Size * Size];
        var tile_id = 0;
        for (var y = 0; y < Size; y++)
        for (var x = 0; x < Size; x++)
        {
            var tile = new WorldTile(x, y, tile_id);
            _tile_map[x, y] = tile;
            _tile_list[tile_id] = tile;
            tile.resetNeighbourLists();
            tile_id++;
        }

        var neighbours = new List<WorldTile>();
        var neighbours_all = new List<WorldTile>();
        for (var i = 0; i < Size * Size; i++)
        {
            WorldTile tile = _tile_list[i];

            WorldTile neighbour;
            neighbour = GetTile(tile.x - 1, tile.y);
            tile.AddNeighbour(neighbour, TileDirection.Left, GeneratorTool._neighbours, GeneratorTool._neighboursAll);
            neighbour = GetTile(tile.x + 1, tile.y);
            tile.AddNeighbour(neighbour, TileDirection.Right, GeneratorTool._neighbours, GeneratorTool._neighboursAll);
            neighbour = GetTile(tile.x, tile.y - 1);
            tile.AddNeighbour(neighbour, TileDirection.Down, GeneratorTool._neighbours, GeneratorTool._neighboursAll);
            neighbour = GetTile(tile.x, tile.y + 1);
            tile.AddNeighbour(neighbour, TileDirection.Up, GeneratorTool._neighbours, GeneratorTool._neighboursAll);
            neighbour = GetTile(tile.x - 1, tile.y - 1);
            tile.AddNeighbour(neighbour, TileDirection.Null, GeneratorTool._neighbours, GeneratorTool._neighboursAll,
                true);
            neighbour = GetTile(tile.x - 1, tile.y + 1);
            tile.AddNeighbour(neighbour, TileDirection.Null, GeneratorTool._neighbours, GeneratorTool._neighboursAll,
                true);
            neighbour = GetTile(tile.x + 1, tile.y - 1);
            tile.AddNeighbour(neighbour, TileDirection.Null, GeneratorTool._neighbours, GeneratorTool._neighboursAll,
                true);
            neighbour = GetTile(tile.x + 1, tile.y + 1);
            tile.AddNeighbour(neighbour, TileDirection.Null, GeneratorTool._neighbours, GeneratorTool._neighboursAll,
                true);
            tile.neighbours = neighbours.ToArray();
            tile.neighboursAll = neighbours_all.ToArray();
            neighbours.Clear();
            neighbours_all.Clear();
        }
    }
}