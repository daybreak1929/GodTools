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
            foreach (ButtonContainer container in containers.Values) container.gameObject.SetActive(false);
            containers[container_id].gameObject.SetActive(true);
            containers[container_id].switch_to_page(C._default);
            curr_container_id = container_id;
        }
        private void create_button_container_prefab()
        {
            button_container_prefab = new GameObject("Button Container Prefab", typeof(ButtonContainer)).GetComponent<ButtonContainer>();
        }
        public void switch_to_page(string page_id)
        {
            containers[curr_container_id].switch_to_page(page_id);
        }
    }
}
