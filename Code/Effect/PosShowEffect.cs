using UnityEngine;
using Object = UnityEngine.Object;

namespace GodTools.Effect;

internal class PosShowEffect : SimpleEffect
{
    public UnitSelectionEffect effect;
    public float left_time;
    public BaseSimObject selected_object;

    public void update(float pElapsed)
    {
        effect.updateSpriteAnimation(pElapsed);
        if (left_time <= 0f && left_time > -1)
        {
            effect.gameObject.SetActive(false);
            selected_object = null;
        }
        else
        {
            left_time -= pElapsed;
            if (selected_object != null)
            {
                if (!selected_object.isAlive())
                {
                    effect.gameObject.SetActive(false);
                    selected_object = null;
                    left_time = 0f;
                    return;
                }

                effect.transform.position = selected_object.currentPosition;
                effect.transform.localScale = selected_object.currentScale;
            }
        }
    }

    public void create(GameObject prefab)
    {
        effect = Object.Instantiate(prefab, Main.instance.gameObject.transform).GetComponent<UnitSelectionEffect>();
        effect.create();
    }

    public bool finished()
    {
        return left_time <= 0 && left_time > -1;
    }

    public void set_active(bool status = true)
    {
        effect.gameObject.SetActive(status);
    }

    public void show(Vector2 position, float scale = 0.25f, float time = 1f)
    {
        show(position, new Vector3(scale, scale), time);
    }

    public void show(Vector2 position, Vector3 scale, float time = 1f)
    {
        effect.gameObject.SetActive(true);
        left_time = time * C.pos_show_effect_time_scale;
        effect.transform.position = position;
        effect.transform.localScale = scale;
        selected_object = null;
    }

    public void show(BaseSimObject actor, Vector3 scale, float time = 1f)
    {
        effect.gameObject.SetActive(true);
        left_time = time * C.pos_show_effect_time_scale;
        effect.transform.position = actor.currentPosition;
        effect.transform.localScale = scale;
        selected_object = actor;
    }

    public void stop()
    {
        left_time = 0;
    }
}