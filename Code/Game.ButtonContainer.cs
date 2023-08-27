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
            }
        }

        public void inactivate_button(string button_id)
        {
            buttons[button_id].SetActive(false);
        }

        public void inactivate_all_buttons()
        {
            foreach (GameObject obj in buttons.Values)
            {
                obj.SetActive(false);
            }
        }
    }
}
