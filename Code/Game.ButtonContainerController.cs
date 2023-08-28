using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.Game
{
    internal class ButtonContainerController : MonoBehaviour
    {
        private static ButtonContainer button_container_prefab;
        private string curr_container_id;
        private bool container_changed = false;
        private Dictionary<string, ButtonContainer> containers = new Dictionary<string, ButtonContainer>();
        public void init(List<string> container_ids)
        {
            if (button_container_prefab == null) create_button_container_prefab();
            foreach(string id in container_ids)
            {
                ButtonContainer __button_container = Instantiate(button_container_prefab, this.transform);
                __button_container.name = id;
                __button_container.transform.localPosition = new Vector3(160, -20);
                containers[id] = __button_container;
                __button_container.gameObject.SetActive(false);
            }
        }
        public void add_button(string container_id, string page_id, GameObject button_obj)
        {
            containers[container_id].add_button(button_obj, page_id);
        }
        public void switch_to_container(string container_id)
        {
            if (!containers.ContainsKey(container_id)) return;
            if (curr_container_id == container_id) return;
            container_changed = true;
            foreach (ButtonContainer container in containers.Values) container.gameObject.SetActive(false);
            containers[container_id].gameObject.SetActive(true);
            curr_container_id = container_id;
        }
        private void create_button_container_prefab()
        {
            button_container_prefab = new GameObject("Button Container Prefab", typeof(ButtonContainer), typeof(RectTransform), typeof(GridLayoutGroup)).GetComponent<ButtonContainer>();
            GridLayoutGroup layout_group = button_container_prefab.GetComponent<GridLayoutGroup>();
            layout_group.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            layout_group.spacing = new(10, 10);
            layout_group.cellSize = new(60, 60);
            layout_group.startCorner = GridLayoutGroup.Corner.UpperLeft;
            button_container_prefab.GetComponent<RectTransform>().sizeDelta = new(1560, 200);
        }

        public void switch_to_page(string page_id)
        {
            ButtonContainer button_container = containers[curr_container_id];
            
            List<string> active_page_buttons = Game.current_data.get_page(page_id);

            if (container_changed || !button_container.contains_all_active_buttons(active_page_buttons))
            {
                container_changed = false;
                button_container.inactivate_all_buttons();
            }
            foreach (string button_id in active_page_buttons)
            {
                button_container.try_to_activate_button(button_id);
            }
        }
    }
}
