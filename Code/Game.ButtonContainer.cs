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
        private static ButtonContainerPage page_prefab;
        private Dictionary<string, ButtonContainerPage> pages = new Dictionary<string, ButtonContainerPage>();
        public void create_page(string page_id)
        {
            if (page_prefab == null)
            {
                create_page_prefab();
            }
            ButtonContainerPage page = Instantiate(page_prefab, this.transform);
            page.name = page_id;
            pages[page_id] = page;
        }

        private void create_page_prefab()
        {
            page_prefab = new GameObject("Button Container Page Prefab", typeof(ButtonContainerPage), typeof(GridLayoutGroup)).GetComponent<ButtonContainerPage>();
            GridLayoutGroup grid_layout_group = page_prefab.GetComponent<GridLayoutGroup>();
            grid_layout_group.cellSize = new Vector2(60, 60);
            grid_layout_group.spacing = new Vector2(10, 10);
            grid_layout_group.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            page_prefab.GetComponent<RectTransform>().sizeDelta = new Vector2(1560, 200);
        }

        public void add_button(GameObject button_obj, string page_id="")
        {
            if (!this.pages.ContainsKey(page_id)) create_page(page_id);
            this.pages[page_id].add_button(button_obj);
        }

        internal void switch_to_page(string @default)
        {
            if (!pages.ContainsKey(@default)) create_page(@default);
            foreach(ButtonContainerPage page in pages.Values)
            {
                page.gameObject.SetActive(false);
            }
            pages[@default].gameObject.SetActive(true);
        }
    }
}
