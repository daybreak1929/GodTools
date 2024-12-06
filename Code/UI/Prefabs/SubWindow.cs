using NeoModLoader.General.UI.Prefabs;
using UnityEngine;

namespace GodTools.UI.Prefabs;

public abstract class SubWindow<TSubWindow> : APrefab<TSubWindow> where TSubWindow : SubWindow<TSubWindow>
{
    public static TSubWindow CreateAndInit(string window_id)
    {
        TSubWindow window = Instantiate(Prefab, CanvasMain.instance.transformWindows);
        window.Init();
        return window;
    }

    protected sealed override void Init()
    {
        if (Initialized) return;
        base.Init();
        OnInit();
    }

    protected virtual void OnInit()
    {
        
    }
}