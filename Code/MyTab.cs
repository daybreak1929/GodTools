using NCMS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GodTools.Code
{
    internal enum Tab_Button_Type
    {
        INFO,
        TOOL,
        OTHERS
    }
    internal static class MyTab
    {
        public static GameObject tab;
        public static PowersTab powers_tab;
        private static Dictionary<Tab_Button_Type, List<PowerButton>> buttons_to_apply;
        private static float cur_x = 72f;
        private static float step_x = 36f;
        private const float up_y = 18f;
        private const float down_y = -18f;
        private const float line_step = 23f;
        private static bool to_add_to_up = true;
        private static GameObject origin_line;

        internal static void create_tab()
        {
            GameObject origin_tab_button = GameObjects.FindEvenInactive("Button_Other");
            GameObject tab_button = GameObject.Instantiate(origin_tab_button);
            tab_button.name = "Button_GodTools";
            tab_button.transform.SetParent(origin_tab_button.transform.parent);
            tab_button.transform.localScale = new Vector3(1f, 1f);
            tab_button.transform.localPosition = new Vector3(-150f, 49.62f);//x轴待调整
            tab_button.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("gt_windows/iconTab");
            //设置栏内元素

            GameObject origin_tab = GameObjects.FindEvenInactive("Tab_Other");
            //暂时禁用copyTab内元素
            foreach (Transform transform in origin_tab.transform)
            {
                transform.gameObject.SetActive(false);
            }

            tab = GameObject.Instantiate(origin_tab);
            tab.SetActive(false);
            //删除复制来的无用元素
            foreach (Transform transform in tab.transform)
            {
                if (transform.gameObject.name == "tabBackButton" || transform.gameObject.name == "-space")
                {
                    transform.gameObject.SetActive(true);
                }
                else
                {
                    GameObject.Destroy(transform.gameObject);
                }
            }
            //恢复copyTab内元素
            foreach (Transform transform in origin_tab.transform)
            {
                transform.gameObject.SetActive(true);
            }

            tab.name = "Tab_GodTools";
            tab.transform.SetParent(origin_tab.transform.parent);

            //子内容设置
            Button buttonComponent = tab_button.GetComponent<Button>();
            powers_tab = tab.GetComponent<PowersTab>();
            powers_tab.powerButton = buttonComponent;
            powers_tab.powerButton.onClick = new Button.ButtonClickedEvent();
            powers_tab.powerButton.onClick.AddListener(tab_button_click);
            powers_tab.parentObj = origin_tab.transform.parent.parent.gameObject;
            powers_tab.powerButtons.Clear();

            tab_button.GetComponent<TipButton>().textOnClick = "工具箱";
            tab_button.GetComponent<TipButton>().text_description_2 = "";

            buttons_to_apply = new Dictionary<Tab_Button_Type, List<PowerButton>>();
            buttons_to_apply[Tab_Button_Type.INFO] = new List<PowerButton>();
            buttons_to_apply[Tab_Button_Type.TOOL] = new List<PowerButton>();
            buttons_to_apply[Tab_Button_Type.OTHERS] = new List<PowerButton>();
            origin_line = GameObject.Find("CanvasBottom/BottomElements/BottomElementsMover/CanvasScrollView/Scroll View/Viewport/Content/buttons/Tab_Other/LINE");
        }
        internal static void add_buttons()
        {
            PowerButton button;
            add_button(
                create_button(
                    C.about_this, "ui/Icons/iconabout",
                    show_window_about_this
                    ),
                Tab_Button_Type.INFO
                );
            add_button(
                create_button(
                    C.world_law, "ui/Icons/iconworldlaws",
                    null
                    ),
                Tab_Button_Type.TOOL
                );
            add_button(
                create_button(
                    C.force_attack, $"ui/Icons/{C.icon_GP_force_attack}",
                    null, ButtonType.GodPower
                    ),
                Tab_Button_Type.TOOL
                );
            add_button(
                create_button(
                    C.new_game, $"ui/Icons/{C.icon_GP_force_attack}",
                    Main.game.start
                    ),
                Tab_Button_Type.OTHERS
                );
        }

        public static PowerButton create_button(string id, string sprite_path, UnityAction action, ButtonType button_type = ButtonType.Click)
        {
            return PowerButtons.CreateButton(
                id, 
                Resources.Load<Sprite>(sprite_path), 
                LocalizedTextManager.stringExists(id + C.title_postfix) ? LocalizedTextManager.getText(id + C.title_postfix) : id, 
                LocalizedTextManager.stringExists(id + C.desc_postfix) ? LocalizedTextManager.getText(id + C.desc_postfix) : "", 
                Vector2.zero, 
                button_type, 
                tab.transform, 
                action);
        }
        public static PowerButton add_button(PowerButton button, Tab_Button_Type button_type = Tab_Button_Type.OTHERS)
        {
            buttons_to_apply[button_type].Add(button);
            return button;
        }
        internal static void apply_buttons()
        {
            foreach (List<PowerButton> buttons in buttons_to_apply.Values)
            {
                to_add_to_up = true;
                foreach (PowerButton button in buttons)
                {
                    button.transform.localScale = Vector3.one;
                    button.transform.localPosition = new Vector3(cur_x, to_add_to_up ? up_y : down_y);
                    to_add_to_up = !to_add_to_up;
                    if (to_add_to_up) cur_x += step_x;
                }
                add_line(to_add_to_up);
            }
            tab.SetActive(true);
        }
        private static void add_line(bool to_add_to_up)
        {
            GameObject line = GameObject.Instantiate(origin_line, powers_tab.transform);
            line.transform.localPosition = new Vector3(cur_x + line_step - (to_add_to_up ? step_x : 0), line.transform.localPosition.y);
            cur_x += 2 * line_step - (to_add_to_up ? step_x : 0);
        }
        private static void tab_button_click()
        {
            GameObject tab = GameObjects.FindEvenInactive("Tab_GodTools");
            PowersTab powersTab = tab.GetComponent<PowersTab>();
            powersTab.showTab(powersTab.powerButton);
        }
        private static void show_window_about_this()
        {
            Windows.ShowWindow("gt_window_about_this");
        }
    }
}
