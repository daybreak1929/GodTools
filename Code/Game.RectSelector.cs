using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GodTools.Game
{
    internal class RectSelector : BaseWorldObject
    {
        SpriteRenderer spr;
        Vector2 start_pos;
        Vector2 cur_pos;
        bool initialized = false;
        float mouse_down_timer = 0;
        void Awake()
        {
            if (!initialized)
            {
                initialized = true;
                initialize();
            }
        }

        public virtual void initialize()
        {
            this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("gt_windows/white_block");
            this.GetComponent<SpriteRenderer>().color = new Color(0, 0.5f, 0.5f, 0.5f);
        }

        public virtual void start()
        {
            this.gameObject.SetActive(true);
            spr = GetComponent<SpriteRenderer>();
            spr.sortingLayerName = "EffectsTop";
            spr.drawMode = SpriteDrawMode.Tiled;
            spr.size = new Vector2(0,0);
            this.transform.localScale = new Vector3(1, 1);
        }
        public override void onStart()
        {
            start_pos = Vector2.zero;
            cur_pos = Vector2.zero;
        }

        public void Update()
        {
            mouse_down_timer += Time.deltaTime;
            if (MapBox.instance.isOverUI()) return;
            if (Input.GetMouseButtonDown(0))
            {
                start_pos = World.world.getMousePos();
                mouse_down_timer = 0;
            }
            if (Input.GetMouseButton(0))
            {
                cur_pos = World.world.getMousePos();
                if(mouse_down_timer > 0.15f)CreatRectangle(start_pos, cur_pos);
            }
            else
            {
                this.transform.localPosition = Globals.POINT_IN_VOID;
            }
        }
        public virtual void CreatRectangle(Vector3 start, Vector3 end)
        {
            spr.size = new Vector2(start.x - end.x, end.y - start.y);
            this.transform.localScale = new Vector3(1, 1);
            this.transform.localPosition = new Vector3(start.x-spr.size.x/2f, start.y);
        }

        public virtual void finish()
        {
        }
    }
}
