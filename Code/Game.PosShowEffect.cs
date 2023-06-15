using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GodTools.Game
{
    internal class PosShowEffect : SimpleEffect
    {
        public float left_time = 0f;
        public BaseSimObject selected_object;
        public UnitSelectionEffect effect;
        public void update(float pElapsed)
        {
            effect.updateSpriteAnimation(pElapsed, false);
            if (this.left_time <= 0f && this.left_time > -1)
            {
                this.effect.gameObject.SetActive(false);
                this.selected_object = null;
            }
            else
            {
                this.left_time -= pElapsed;
                if (this.selected_object != null)
                {
                    if (!this.selected_object.isAlive())
                    {
                        this.effect.gameObject.SetActive(false);
                        this.selected_object = null;
                        this.left_time = 0f;
                        return;
                    }
                    this.effect.transform.position = this.selected_object.currentPosition;
                    this.effect.transform.localScale = this.selected_object.currentScale;
                }
            }
        }
        public void show(Vector2 position, float scale = 0.25f, float time = 1f)
        {
            show(position, new Vector3(scale, scale), time);
        }
        public void show(Vector2 position, Vector3 scale, float time = 1f)
        {
            this.effect.gameObject.SetActive(true);
            this.left_time = time * C.pos_show_effect_time_scale;
            this.effect.transform.position = position;
            this.effect.transform.localScale = scale;
            this.selected_object = null;
        }
        public void show(BaseSimObject actor, Vector3 scale, float time = 1f)
        {
            this.effect.gameObject.SetActive(true);
            this.left_time = time * C.pos_show_effect_time_scale;
            this.effect.transform.position = actor.currentPosition;
            this.effect.transform.localScale = scale;
            this.selected_object = actor;
        }
        public void stop()
        {
            left_time = 0;
        }

        public void create(GameObject prefab)
        {
            this.effect = GameObject.Instantiate<GameObject>(prefab, Code.Main.instance.gameObject.transform).GetComponent<UnitSelectionEffect>();
            this.effect.create();
        }

        public bool finished()
        {
            return this.left_time <= 0 && this.left_time > -1;
        }

        public void set_active(bool status = true)
        {
            this.effect.gameObject.SetActive(status);
        }
    }
}
