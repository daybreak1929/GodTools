using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GodTools.Code;
using UnityEngine;

namespace GodTools.Game
{
    internal class ObjectRectSelector : RectSelector
    {
        public MapObjectType type_to_select;
        private HashSetBaseSimObject objects_a;
        private HashSetBaseSimObject objects_b;
        public HashSetBaseSimObject active_objects;
        private HashSetBaseSimObject inactive_objects;
        public override void initialize()
        {
            base.initialize();
            objects_a = new HashSetBaseSimObject();
            objects_b = new HashSetBaseSimObject();
            active_objects = objects_a;
            inactive_objects = objects_b;
        }
        public override void finish()
        {
            Main.instance.pos_show_effect_controller.inactive_all();
            this.gameObject.SetActive(false);
        }

        internal void clear_active()
        {
            this.active_objects.Clear();
            Main.instance.pos_show_effect_controller.inactive_all();
        }

        public override void CreatRectangle(Vector3 start, Vector3 end)
        {
            base.CreatRectangle(start, end);
            int x_off = end.x >= start.x ? 1 : -1;
            int y_off = end.y >= start.y ? 1 : -1;
            int start_x = (int)start.x;
            int start_y = (int)start.y;
            int len_x = Math.Abs((int)end.x - start_x);
            int len_y = Math.Abs((int)end.y - start_y);
            for (int i = 0; i < len_x; i++)
            {
                for (int j = 0; j < len_y; j++)
                {
                    WorldTile tile = World.world.GetTile(start_x + i * x_off, start_y + j * y_off);

                    if (tile == null) continue;
                    if(type_to_select == MapObjectType.Actor)
                    {
                        if(Game.instance.player==null)inactive_objects.UnionWith(tile._units);
                        else
                        {
                            foreach(Actor unit in tile._units)
                            {
                                if(unit.isAlive()&&unit.kingdom==Game.instance.player) inactive_objects.Add(unit);
                            }
                        }
                    }
                    else if(tile.building!=null)
                    {
                        if(Game.instance.player==null || tile.building.kingdom == Game.instance.player)inactive_objects.Add(tile.building);
                    }
                }
            }
            List<PosShowEffect> effects = Main.instance.pos_show_effect_controller.get_active_effects();
            foreach (PosShowEffect pos_show_effect in effects)
            {
                if (!inactive_objects.Contains(pos_show_effect.selected_object))
                {
                    pos_show_effect.stop();
                }
            }
            foreach(BaseSimObject obj in inactive_objects)
            {
                if (!active_objects.Contains(obj))
                {
                    Main.instance.pos_show_effect_controller.get().show(obj, obj.currentScale, -1);
                }
            }
            (active_objects, inactive_objects) = (inactive_objects, active_objects);
            inactive_objects.Clear();
        }
    }
}
