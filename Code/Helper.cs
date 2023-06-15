using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
namespace GodTools
{
    internal class Helper
    {
        private static Sprite window_inner_sliced;
        private static Sprite button;
        private static Sprite window_bg;
        private static GameObject button_with_bg_prefab;
        private static GameObject button_without_bg_prefab;
        public static GameObject create_button_with_bg(string name, string path_to_img, UnityEngine.Events.UnityAction action, string key=C.GT_about_this)
        {
            if(button_with_bg_prefab == null)
            {
                button_with_bg_prefab = new GameObject("Button(BG) Prefab", typeof(Image));
                button_with_bg_prefab.GetComponent<Image>().sprite = get_button();
                button_with_bg_prefab.GetComponent<Image>().type = Image.Type.Sliced;
                GameObject __button = new GameObject("Button", typeof(Image), typeof(Button), typeof(TipButton));
                __button.GetComponent<TipButton>().textOnClick = C.success;
                __button.transform.SetParent(button_with_bg_prefab.transform);

            }
            GameObject ret = GameObject.Instantiate(button_with_bg_prefab);
            ret.name = name;
            ret.GetComponent<Image>().sprite = get_button();
            ret.transform.Find("Button").GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);
            ret.transform.Find("Button").GetComponent<Image>().sprite = Resources.Load<Sprite>(path_to_img);
            ret.transform.Find("Button").GetComponent<Button>().onClick.AddListener(action);
            if (LocalizedTextManager.stringExists(key + C.title_postfix)) ret.transform.Find("Button").GetComponent<TipButton>().textOnClick = key+C.title_postfix;
            if (LocalizedTextManager.stringExists(key + C.desc_postfix)) ret.transform.Find("Button").GetComponent<TipButton>().textOnClickDescription = key + C.desc_postfix;
            return ret;
        }
        public static GameObject create_button_with_bg(string name, Sprite icon, UnityEngine.Events.UnityAction action, string key = C.GT_about_this)
        {
            if (button_with_bg_prefab == null)
            {
                button_with_bg_prefab = new GameObject("Button(BG) Prefab", typeof(Image));
                button_with_bg_prefab.GetComponent<Image>().sprite = get_button();
                button_with_bg_prefab.GetComponent<Image>().type = Image.Type.Sliced;
                GameObject __button = new GameObject("Button", typeof(Image), typeof(Button), typeof(TipButton));
                __button.GetComponent<TipButton>().textOnClick = C.success;
                __button.transform.SetParent(button_with_bg_prefab.transform);

            }
            GameObject ret = GameObject.Instantiate(button_with_bg_prefab);
            ret.name = name;
            ret.GetComponent<Image>().sprite = get_button();
            ret.transform.Find("Button").GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);
            ret.transform.Find("Button").GetComponent<Image>().sprite = icon;
            ret.transform.Find("Button").GetComponent<Button>().onClick.AddListener(action);
            if (LocalizedTextManager.stringExists(key + C.title_postfix)) ret.transform.Find("Button").GetComponent<TipButton>().textOnClick = key + C.title_postfix;
            if (LocalizedTextManager.stringExists(key + C.desc_postfix)) ret.transform.Find("Button").GetComponent<TipButton>().textOnClickDescription = key + C.desc_postfix;
            return ret;
        }
        public static GameObject create_button_without_bg(string name, string path_to_img, UnityEngine.Events.UnityAction action, string key = C.GT_about_this)
        {
            if (button_without_bg_prefab == null)
            {
                button_without_bg_prefab = new GameObject("Button(NOBG) Prefab", typeof(Image), typeof(Button), typeof(TipButton));
                button_without_bg_prefab.GetComponent<TipButton>().textOnClick = C.success;
            }
            GameObject ret = GameObject.Instantiate(button_with_bg_prefab);
            ret.name = name;
            ret.GetComponent<Image>().sprite = Resources.Load<Sprite>(path_to_img);
            ret.GetComponent<Button>().onClick.AddListener(action);
            if (LocalizedTextManager.stringExists(key + C.title_postfix)) ret.GetComponent<TipButton>().textOnClick = key + C.title_postfix;
            if(LocalizedTextManager.stringExists(key + C.desc_postfix))ret.GetComponent<TipButton>().textOnClickDescription = key + C.desc_postfix;
            return ret;
        }
        public static Sprite get_window_bg()
        {
            if (window_bg == null)
            {
                window_bg = Resources.Load<Sprite>("gt_windows/window_big_bg");
            }
            return window_bg;
        }
        public static Sprite get_inner_sliced()
        {
            if(window_inner_sliced == null)
            {
                window_inner_sliced = GameObject.Find("/Canvas Container Main/Canvas - Windows/windows/welcome/Background/BottomBg").GetComponent<Image>().sprite;
                //window_inner_sliced = SpriteTextureLoader.getSprite("gt_windows/window_inner_sliced");
            }
            return window_inner_sliced;
        }
        public static Sprite get_button()
        {
            if (button == null)
            {
                button = CanvasMain.instance.canvas_ui.transform.Find("CanvasParent/CanvasRight/DebugButton").GetComponent<Image>().sprite;
            }
            return button;
        }
        public static void text_basic_setting(Text text)
        {
            text.font = LocalizedTextManager.currentFont;
            text.fontStyle = FontStyle.Bold;
            text.fontSize = 14;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMaxSize = 14;
            text.resizeTextMinSize = 2;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.text = "文本";
            text.color = new Color(1, 0.607f, 0.110f, 1);
        }
        public static Actor get_actor_near_mouse(Kingdom kingdom = null)
        {
            float min_dist = 999;
            Actor ret = null;
            Vector2 center = World.world.getMousePos();
            int x_start = (int)center.x - C.find_near_unit_size;
            int y_start = (int)center.y - C.find_near_unit_size;
            int len = 2 * C.find_near_unit_size + 1;
            for(int x = 0; x < len; x++)
            {
                for(int y = 0; y < len; y++)
                {
                    WorldTile tile = World.world.GetTile(x + x_start, y + y_start);
                    if (tile == null) continue;
                    float dist = Toolbox.Dist(tile.x, tile.y, center.x, center.y);
                    if (dist >= min_dist) continue;
                    foreach(Actor unit in tile._units)
                    {
                        if ((kingdom == null || unit.kingdom == kingdom)&&unit.isAlive()&&unit.asset.canBeInspected&&!unit.isInsideSomething())
                        {
                            ret = unit;
                            min_dist = dist;
                            break;
                        }
                    }
                }
            }
            return ret;
        }
        public static Building get_building_near_mouse(Kingdom kingdom = null, bool ignore_dec = true)
        {
            float min_dist = 999;
            Building ret = null;
            Vector2 center = World.world.getMousePos();
            int x_start = (int)center.x - C.find_near_unit_size;
            int y_start = (int)center.y - C.find_near_unit_size;
            int len = 2 * C.find_near_unit_size + 1;
            for (int x = 0; x < len; x++)
            {
                for (int y = 0; y < len; y++)
                {
                    WorldTile tile = World.world.GetTile(x + x_start, y + y_start);
                    if (tile == null || tile.building==null || (kingdom!=null&&tile.building.kingdom!=kingdom) || (ignore_dec && tile.building.asset.buildingType==BuildingType.Plant)) continue;
                    float dist = Toolbox.Dist(tile.x, tile.y, center.x, center.y);
                    if (dist >= min_dist) continue;
                    min_dist = dist;
                    ret = tile.building;
                }
            }
            return ret;
        }
    }
}
