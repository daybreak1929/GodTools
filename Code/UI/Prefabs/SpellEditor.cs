#if 启源_启源核心
using System;
using System.Collections.Generic;
using Cultivation_Way.Library;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;

namespace GodTools.UI.Prefabs;

public class SpellEditor : APrefab<SpellEditor>
{
    private readonly List<CW_SpellAsset> _spells = new();

    public List<CW_SpellAsset> GetResult()
    {
        return new List<CW_SpellAsset>(_spells);
    }

    public void Setup(Action<List<CW_SpellAsset>> pActionOnEnd = null)
    {
    }

    private static void _init()
    {
        var obj = new GameObject(nameof(SpellEditor), typeof(RectTransform));

        Prefab = obj.AddComponent<SpellEditor>();
    }
}
#endif