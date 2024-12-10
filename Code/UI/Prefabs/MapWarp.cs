using System.Collections.Generic;
using NeoModLoader.General.UI.Prefabs;
using TMPro;
using UnityEngine;

namespace GodTools.UI.Prefabs;

public class MapWarp : APrefab<MapWarp>
{
    public TMP_Text text;

    protected override void Init()
    {
        if (Initialized) return;
        base.Init();
        text = GetComponent<TMP_Text>();
    }

    public void Setup(string text, List<TileZone> zones)
    {
        Init();
        this.text.text = text;
        // Find center position
        Vector3 center = Vector3.zero;
        foreach (TileZone zone in zones) center += zone.centerTile.posV;
    }

    private static void _init()
    {
        GameObject obj = new(nameof(MapWarp), typeof(TextMeshProUGUI));
        obj.transform.SetParent(Main.prefabs);
        obj.GetComponent<TextMeshProUGUI>().font = Main.default_font;

        Prefab = obj.AddComponent<MapWarp>();
    }
}