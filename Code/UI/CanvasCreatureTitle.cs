using System;
using System.Linq;
using GodTools.Abstract;
using GodTools.UI.Prefabs;
using GodTools.Utils;
using NeoModLoader.api.attributes;
using UnityEngine;

namespace GodTools.UI;

public class CanvasCreatureTitle : MonoBehaviour
{
    internal static void Init()
    {
        var canvas = Instantiate(CanvasMain.instance.canvas_tooltip, CanvasMain.instance.transform);
        canvas.name = "CanvasCreatureTitle";
        canvas.gameObject.AddComponent<CanvasCreatureTitle>();
    }

    private void Start()
    {
        _pool = new MonoObjPool<CreatureTitleDisplay>(CreatureTitleDisplay.Prefab, transform);
        CanvasRect = GetComponent<RectTransform>();
    }

    private MonoObjPool<CreatureTitleDisplay> _pool;
    public RectTransform CanvasRect { get; private set; }
    private void Update()
    {
        OnUpdate();
    }
    [Hotfixable]
    private void OnUpdate()
    {
        _pool.ResetToStart();
        if (ScrollWindow.currentWindows.Count == 0)
            foreach (var unit in World.world.units.getSimpleList())
            {
                if (unit == null) continue;
                if (!unit.isAlive()) continue;
                if (!unit.is_visible) continue;
                
                var title_guids = unit.GetTitleGuids();
                if (title_guids.Count == 0) continue;
                _pool.GetNext().Setup(unit, title_guids.Select(x=>unit.GetTitle(x)), CanvasRect);
            }
        _pool.ClearUnsed();
    }
}