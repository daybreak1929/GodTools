using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.Game
{
    internal class ButtonContainer : MonoBehaviour
    {
        private Dictionary<string, GameObject> buttons = new();
        private HashSet<string> active_buttons = new();
        public void add_button(GameObject button_obj, string page_id="")
        {
            button_obj.transform.SetParent(transform);
            buttons[button_obj.name] = button_obj;
        }

        public void try_to_activate_button(string button_id)
        {
            if (buttons.ContainsKey(button_id))
            {
                buttons[button_id].SetActive(true);
                active_buttons.Add(button_id);
            }
        }

        public void inactivate_button(string button_id)
        {
            if (buttons.ContainsKey(button_id))
            {
                buttons[button_id].SetActive(false);
                active_buttons.Remove(button_id);
            }
        }

        public void inactivate_all_buttons()
        {
            active_buttons.Clear();
            foreach (GameObject obj in buttons.Values)
            {
                obj.SetActive(false);
            }
        }

        public bool contains_all_active_buttons(List<string> button_list)
        {
            return active_buttons.IsSupersetOf(button_list);
        }
    }
}
