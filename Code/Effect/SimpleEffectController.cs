using System.Collections.Generic;
using UnityEngine;

namespace GodTools.Effect;

internal interface SimpleEffect
{
    void update(float elapsed);
    void create(GameObject prefab);
    bool finished();
    void set_active(bool status = true);
}

internal class SimpleEffectController<T> where T : SimpleEffect, new()
{
    private readonly List<T> __active_effects = new();
    private readonly Stack<T> __inactive_effects = new();
    private readonly GameObject __prefab;

    public SimpleEffectController(GameObject prefab)
    {
        __prefab = prefab;
    }

    public List<T> get_active_effects()
    {
        return __active_effects;
    }

    public T get(bool independent = false)
    {
        T ret;
        if (__inactive_effects.Count > 0)
        {
            ret = __inactive_effects.Pop();
            if (!independent) __active_effects.Add(ret);
            ret.set_active();
            return ret;
        }

        ret = new T();
        ret.create(__prefab);
        ret.set_active();
        if (!independent) __active_effects.Add(ret);
        return ret;
    }

    public void update(float elapsed)
    {
        for (var i = 0; i < __active_effects.Count; i++)
        {
            __active_effects[i].update(elapsed);
            if (__active_effects[i].finished())
            {
                __active_effects[i].set_active(false);
                __inactive_effects.Push(__active_effects[i]);
                __active_effects.RemoveAt(i);
                i--;
            }
        }
    }

    public void inactive_all()
    {
        foreach (var t in __active_effects)
        {
            t.set_active(false);
            __inactive_effects.Push(t);
        }

        __active_effects.Clear();
    }
}