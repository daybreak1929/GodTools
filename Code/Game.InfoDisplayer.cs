using GodTools.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.Game
{
    internal class InfoDisplayer : MonoBehaviour
    {
        public Text title;
        public Image icon;
        public StatBar health;
        public StatBar progress;
        public Text desc;
        public void init()
        {
            this.gameObject.SetActive(false);
            GameObject title_obj = new GameObject("Title", typeof(Text));
            title_obj.transform.SetParent(transform);
            title_obj.transform.localPosition = new Vector3(0, 135);
            title_obj.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 20);
            title = title_obj.GetComponent<Text>();
            Helper.text_basic_setting(title);
            GameObject icon_obj = new GameObject("Icon", typeof(Image));
            icon_obj.transform.SetParent(transform);
            icon = icon_obj.GetComponent<Image>();
            Vector2 bar_shape = new Vector2(100, 10);
            GameObject health_bar_obj = new GameObject("Health Bar", typeof(Image));
            health_bar_obj.SetActive(false);
            health_bar_obj.AddComponent<StatBar>();
            health_bar_obj.transform.SetParent(transform);
            health_bar_obj.GetComponent<Image>().sprite = Helper.get_inner_sliced();
            health_bar_obj.GetComponent<Image>().type = Image.Type.Sliced;
            health_bar_obj.GetComponent<RectTransform>().sizeDelta = bar_shape+new Vector2(30,5);
            GameObject health_bar_bg_obj = new GameObject("Background", typeof(Image));
            health_bar_bg_obj.transform.SetParent(health_bar_obj.transform);
            health_bar_bg_obj.GetComponent<Image>().sprite = Helper.get_inner_sliced();
            health_bar_bg_obj.GetComponent<Image>().type = Image.Type.Sliced;
            health_bar_bg_obj.GetComponent<RectTransform>().sizeDelta = bar_shape;
            GameObject health_bar_mask_obj = new GameObject("Mask", typeof(Image), typeof(Mask));
            health_bar_mask_obj.transform.SetParent(health_bar_obj.transform);
            health_bar_mask_obj.GetComponent<RectTransform>().sizeDelta = bar_shape;
            GameObject health_bar_mask_bar_obj = new GameObject("Bar", typeof(Image));
            health_bar_mask_bar_obj.transform.SetParent(health_bar_mask_obj.transform);
            health_bar_mask_bar_obj.GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("gt_windows/window_bar");
            health_bar_mask_bar_obj.GetComponent<Image>().type = Image.Type.Sliced;
            health_bar_mask_bar_obj.GetComponent<RectTransform>().sizeDelta = bar_shape;
            GameObject health_bar_icon_obj = new GameObject("Icon", typeof(Image), typeof(Shadow));
            health_bar_icon_obj.transform.SetParent(health_bar_obj.transform);
            health_bar_icon_obj.GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("ui/icons/iconHealth");
            health_bar_icon_obj.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
            GameObject health_bar_text_obj = new GameObject("Text", typeof(Text));
            health_bar_text_obj.transform.SetParent(health_bar_obj.transform);
            Helper.text_basic_setting(health_bar_text_obj.GetComponent<Text>());
            health_bar_text_obj.GetComponent<RectTransform>().sizeDelta = bar_shape;

            health = health_bar_obj.GetComponent<StatBar>();
            health.textField = health_bar_text_obj.GetComponent<Text>();
            health.mask = health_bar_mask_obj.GetComponent<RectTransform>();
            health.bar = health_bar_mask_bar_obj.GetComponent<RectTransform>();

            health.transform.localPosition = new Vector3(0, 0);
            health.transform.localScale = new Vector3(1.5f, 1.5f);
            health.textField.transform.localPosition = new Vector3(10, 0);
            health.mask.transform.localPosition = new Vector3(-40, 0);
            health.mask.pivot = new Vector2(0, 0.5f);
            health.mask.GetComponent<Mask>().showMaskGraphic = false;
            health.bar.GetComponent<Image>().color = new Color(0, 1, 0, 1);
            health_bar_icon_obj.transform.localPosition = new Vector3(-52, -1);
            health_bar_bg_obj.transform.localPosition = new Vector3(10, 0);

            progress = Instantiate(health_bar_obj, transform).GetComponent<StatBar>();
            progress.name = "Progress Bar";
            progress.transform.Find("Icon").GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("ui/icons/iconClock");
            progress.transform.localPosition = new Vector3(0, -30);
            progress.transform.localScale = health.transform.localScale;
            progress.textField.transform.localPosition = new Vector3(10, 0);
            progress.mask.transform.localPosition = new Vector3(-40, 0);
            progress.mask.pivot = new Vector2(0, 0.5f);
            progress.mask.GetComponent<Mask>().showMaskGraphic = false;
            progress.bar.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            progress.transform.Find("Icon").localPosition = new Vector3(-52, -1);
            progress.transform.Find("Icon").GetComponent<RectTransform>().sizeDelta = new Vector3(15, 15);
            progress.transform.Find("Background").localPosition = new Vector3(10, 0);

            desc = new GameObject("Desc", typeof(Text)).GetComponent<Text>();
            desc.transform.SetParent(transform);
            desc.transform.localPosition = new Vector3(0, -70);
            desc.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            Helper.text_basic_setting(desc);

            icon.transform.localPosition = new Vector3(0, 70);
        }
        void Update()
        {
            if (!Game.instance.input_controller.enabled || Game.instance.input_controller.curr_main_object==null) return;
            active();
            if(Game.instance.input_controller.curr_main_object.objectType == MapObjectType.Actor)
            {
                Actor actor = Game.instance.input_controller.curr_main_object.a;
                title.text = actor.getName();
                icon.sprite = actor.getSpriteToRender();
                icon.transform.localScale = new Vector3(actor.asset.inspectAvatarScale*0.3f, actor.asset.inspectAvatarScale*0.3f, actor.asset.inspectAvatarScale*0.1f);
                health.setBar(actor.data.health, actor.getMaxHealth(), "/" + actor.getMaxHealth().ToString(), false, false, true, false);
                if(progress.gameObject.activeSelf)progress.gameObject.SetActive(false);
            }
            else if(Game.instance.input_controller.curr_main_object.objectType == MapObjectType.Building)
            {
                Building building = Game.instance.input_controller.curr_main_object.b;
                title.text = "";
                icon.sprite = building.s_main_sprite==null?building.animData.main[0]:building.s_main_sprite;
                icon.transform.localScale = Vector3.one;
                health.setBar(building.data.health, building.getMaxHealth(), "/" + building.getMaxHealth().ToString(), false, false, true, false);
                if (building.data.hasFlag(C.flag_in_progress))
                {
                    building.data.get(C.status_in_progress, out float progress_status, 0);
                    building.data.get(C.progress_id, out string progress_id, "");
                    building.data.get(C.progress_count, out int progress_nr, 0);
                    BuildingProgressAsset asset = Main.building_progresses.get(progress_id);
                    progress.setBar(progress_status, asset.progress, $"/{asset.progress}", false, false, true, false);

                    desc.text = LocalizedTextManager.getText(asset.translate_key)+"×"+ progress_nr;
                    if (asset.translate_action != null)
                    {
                        building.data.get(C.progress_free_str, out string free_str, "");
                        building.data.get(C.progress_free_val, out float free_val, 0);
                        desc.text = asset.translate_action(desc.text, free_str, free_val);
                    }
                }
                else if(progress.gameObject.activeSelf)
                {
                    progress.gameObject.SetActive(false);
                    desc.gameObject.SetActive(false);
                }
            }
        }
        private void active()
        {
            if (!title.gameObject.activeSelf) title.gameObject.SetActive(true);
            if (!icon.gameObject.activeSelf) icon.gameObject.SetActive(true);
            if (!health.gameObject.activeSelf) health.gameObject.SetActive(true);
            if (!progress.gameObject.activeSelf) progress.gameObject.SetActive(true);
            if (!desc.gameObject.activeSelf) desc.gameObject.SetActive(true);
        }
        public void hide()
        {
            title.gameObject.SetActive(false);
            icon.gameObject.SetActive(false);
            health.gameObject.SetActive(false);
            progress.gameObject.SetActive(false);
            desc.gameObject.SetActive(false);
        }
    }
}
