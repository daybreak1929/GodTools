using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GodTools.Abstract;
using HarmonyLib;
using Newtonsoft.Json;
using strings;
using UnityEngine;

namespace GodTools.Features;

public class ConsistentCreaturePowerTop : IManager
{
    public class CreaturePowerData
    {
        public long ID;
        public string Name;
        public double Power;
        public Sprite Sprite;
    }

    public const int MaxCount = 100;
    private static List<long> _sortedCreaturesInTop = new();
    private static Dictionary<long, CreaturePowerData> _creaturePowerData = new();
    
    public static List<CreaturePowerData> GetSortedCreaturePowerData()
    {
        return _sortedCreaturesInTop.Select(id => _creaturePowerData[id]).ToList();
    }

    public static void Check(Actor actor)
    {
        var power = CalcPower(actor);
        try
        {
            actor.getName();
        }
        catch
        {
            Main.LogInfo($"Failed to get name for {actor.data.id}");
        }
        if (_creaturePowerData.TryGetValue(actor.data.id, out var data))
        {
            try
            {
                data.Name = actor.getName();
            }
            catch
            {
                data.Name = "无法获取";
            }
            if (power > data.Power)
            {
                data.Power = power;
                Sort();
            }

            return;
        }

        var lowest_power = _sortedCreaturesInTop.Count > 0 ? _creaturePowerData[_sortedCreaturesInTop[0]].Power : -1;
        if (power < lowest_power && _sortedCreaturesInTop.Count >= MaxCount)
        {
            return;
        }

        var actor_name = "无法获取";
        try
        {
            actor_name = actor.getName();
        }
        catch
        {
            // ignore
        }
        var new_data = new CreaturePowerData()
        {
            ID = actor.data.id,
            Name = actor_name,
            Power = power,
            Sprite = actor._last_colored_sprite ?? actor.asset.getSpriteIcon()
        };
        if (_sortedCreaturesInTop.Count > MaxCount)
        {
            _creaturePowerData.Remove(_sortedCreaturesInTop[0]);
            _sortedCreaturesInTop[0] = actor.data.id;
        }
        else
        {
            _sortedCreaturesInTop.Add(actor.data.id);
        }
        _creaturePowerData.Add(actor.data.id, new_data);
        Sort();
    }

    private static void Sort()
    {
        _sortedCreaturesInTop.Sort((a,b)=> _creaturePowerData[a].Power.CompareTo(_creaturePowerData[b].Power));
    }

    private static double CalcPower(Actor actor)
    {
        var live_time = actor.stats[S.health] / Mathf.Max(1f, 100 - actor.stats[S.armor]);
        var attack_speed = actor.asset.skip_fight_logic ? 1f : (1f / Mathf.Clamp(actor.getAttackCooldown(), 0.016f, 100));
        var attack_fluctuation = (actor.stats[S.damage_range] + 1) * 0.5f;
        var critical_bonus = Mathf.Max(0, actor.stats[S.critical_chance]) *
                             Mathf.Max(0, actor.stats[S.critical_damage_multiplier] - 1);
        var dps = attack_speed * Mathf.Max(1f, actor.stats[S.damage]) *
                  attack_fluctuation * (1 + critical_bonus);

        return ((double)live_time) * ((double)dps) * 0.182699;
    }
    public void Initialize()
    {
        Harmony.CreateAndPatchAll(typeof(ConsistentCreaturePowerTop));
    }
    [HarmonyPostfix,HarmonyPatch(typeof(Actor), nameof(Actor.updateStats))]
    private static void OnStatsUpdate(Actor __instance)
    {
        Check(__instance);
    }
    [HarmonyPostfix,HarmonyPatch(typeof(MapBox), nameof(MapBox.clearWorld))]
    private static void OnCleanUp()
    {
        _creaturePowerData.Clear();
        _sortedCreaturesInTop.Clear();
    }

    private struct RawRectInt(int posX, int posY, int fitWidth, int fitHeight)
    {
        public int x;
        public int y;
        public int width;
        public int height;
    }
    private struct CreaturePowerDataForSave
    {
        public long ID;
        public string Name;
        public double Power;
        public RawRectInt SpriteRect;
    }
    [HarmonyPostfix,HarmonyPatch(typeof(SaveManager), nameof(SaveManager.loadWorld), [typeof(string), typeof(bool)])]
    private static void LoadSave(string pPath)
    {
        var folder = SaveManager.folderPath(pPath);
        var list_save_path = Path.Combine(folder, "consistent_creature_power_top.json");
        var texture_save_path = Path.Combine(folder, "consistent_creature_power_top.png");

        if (!File.Exists(list_save_path) || !File.Exists(texture_save_path))
        {
            return;
        }
        var save_data = JsonConvert.DeserializeObject<List<CreaturePowerDataForSave>>(File.ReadAllText(list_save_path));
        var texture = new Texture2D(0,0);
        texture.LoadImage(File.ReadAllBytes(texture_save_path));

        foreach (var data in save_data)
        {
            _creaturePowerData.Add(data.ID, new CreaturePowerData()
            {
                ID = data.ID, Name = data.Name,Power = data.Power, Sprite = Sprite.Create(texture, new Rect(data.SpriteRect.x, data.SpriteRect.y, data.SpriteRect.width, data.SpriteRect.height), new Vector2(0.5f, 0), 1)
            });
        }
        _sortedCreaturesInTop = save_data.Select(x => x.ID).ToList();
        Sort();
    }
    [HarmonyPostfix,HarmonyPatch(typeof(SaveManager), nameof(SaveManager.saveMapData))]
    private static void Save(string pFolder)
    {
        pFolder = SaveManager.folderPath(pFolder);
        var list_save_path = Path.Combine(pFolder, "consistent_creature_power_top.json");
        var texture_save_path = Path.Combine(pFolder, "consistent_creature_power_top.png");

        var data = GetSortedCreaturePowerData();
        var save_data = new List<CreaturePowerDataForSave>();
        
        var (texture, rects) = CombineSprites(data.Select(x => x.Sprite).ToList());
        for (int i = 0; i < data.Count; i++)
        {
            save_data.Add(new CreaturePowerDataForSave()
            {
                ID = data[i].ID,
                Name = data[i].Name,
                Power = data[i].Power,
                SpriteRect = rects[i]
            });
        }
        File.WriteAllText(list_save_path, JsonConvert.SerializeObject(save_data));
        File.WriteAllBytes(texture_save_path, texture.EncodeToPNG());
    }

    private static (Texture2D, List<RawRectInt>) CombineSprites(List<Sprite> sprites)
    {
        var sizes = sprites.Select(s => new Vector2Int((int)s.textureRect.width, (int)s.textureRect.height)).ToList();
        (var positions, var tex_size) = CalcLayout(sprites, sizes);
        Texture2D combined_texture = new Texture2D(
            tex_size.x, 
            tex_size.y, 
            TextureFormat.RGBA32, 
            false);
        
        Color[] clear_pixels = new Color[tex_size.x * tex_size.y];
        for (int i = 0; i < clear_pixels.Length; i++)
        {
            clear_pixels[i] = Color.clear;
        }
        combined_texture.SetPixels(clear_pixels);

        var rects = new List<RawRectInt>();
        for (int i = 0; i < sprites.Count; i++)
        {
            Sprite sprite = sprites[i];
            Vector2Int pos = positions[i];
            Vector2Int size = sizes[i];

            Texture2D source_texture = sprite.texture.getAsReadable();
            Rect sprite_rect = sprite.textureRect;
            
            Color[] pixels = source_texture.GetPixels(
                (int)sprite_rect.x, 
                (int)sprite_rect.y, 
                size.x, 
                size.y);

            var fit_width = size.x;
            var fit_height = size.y;
            if (pos.x < 0 || pos.y < 0 || 
                pos.x + size.x > tex_size.x || 
                pos.y + size.y > tex_size.y)
            {
                fit_width = Mathf.Min(size.x, tex_size.x - pos.x);
                fit_height = Mathf.Min(size.y, tex_size.y - pos.y);
                
            }
            combined_texture.SetPixels(pos.x, pos.y, fit_width, fit_height, pixels);
            rects.Add(new RawRectInt(pos.x, pos.y, fit_width, fit_height));
        }
        
        combined_texture.Apply();
        return (combined_texture, rects);
    }
    private static int CalculateRequiredHeight(List<Vector2Int> sizes, int width)
    {
        int current_x = 0;
        int current_y = 0;
        int row_max_height = 0;

        foreach (var size in sizes)
        {
            if (current_x + size.x > width)
            {
                current_x = 0;
                current_y += row_max_height;
                row_max_height = 0;
            }
            
            current_x += size.x;
            row_max_height = Mathf.Max(row_max_height, size.y);
        }
        current_y += row_max_height;
        
        return current_y;
    }

    private const int SuggestedWidth = 1024;
    private const int SuggestedHeight = 1024;
    private static (List<Vector2Int>, Vector2Int) CalcLayout(List<Sprite> sprites, List<Vector2Int> sizes)
    {
        int total_width = 0;
        int total_height = 0;
        int max_sprite_width = 0;
        int max_sprite_height = 0;
        
        foreach (var size in sizes)
        {
            total_width += size.x;
            total_height += size.y;
            max_sprite_width = Mathf.Max(max_sprite_width, size.x);
            max_sprite_height = Mathf.Max(max_sprite_height, size.y);
        }
        
        int current_width = Mathf.Clamp(max_sprite_width, max_sprite_width, SuggestedWidth);
        if (total_width < SuggestedWidth)
        {
            current_width = total_width;
        }
        
        int current_height = CalculateRequiredHeight(sizes, current_width);
        
        while (current_height > SuggestedHeight && current_width < SuggestedWidth)
        {
            current_width = Mathf.Min((int)(current_width * 1.5f), SuggestedWidth);
            current_height = CalculateRequiredHeight(sizes, current_width);
        }
        
        List<Vector2Int> positions = CalculateSimpleLayout(sprites, sizes, new Vector2Int(current_width, current_height));
        
        return (positions, new Vector2Int(current_width, current_height));
    }
    private static List<Vector2Int> CalculateSimpleLayout(List<Sprite> sprites, List<Vector2Int> sizes, Vector2Int textureSize)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        int current_x = 0;
        int current_y = 0;
        int row_max_height = 0;

        for (int i = 0; i < sprites.Count; i++)
        {
            var size = sizes[i];
            
            if (current_x + size.x > textureSize.x)
            {
                current_x = 0;
                current_y += row_max_height;
                row_max_height = 0;
            }
            
            positions.Add(new Vector2Int(current_x, current_y));
            
            current_x += size.x;
            row_max_height = Mathf.Max(row_max_height, size.y);
        }
        
        return positions;
    }
}