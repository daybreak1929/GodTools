using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GodTools.Code
{
    internal class StatusEffectGroupElement : MonoBehaviour
    {
        public Text title;
        public List<StatusEffect> statuses = new List<StatusEffect>();
        public static List<StatusEffectButton> all_buttons = new List<StatusEffectButton>();
        public void add_status(StatusEffect status, StatusEffectButton prefab)
        {
            statuses.Add(status);
            StatusEffectButton status_button = GameObject.Instantiate(prefab, transform);
            all_buttons.Add(status_button);
            status_button.load(new StatusEffectData(null, status));
            status_button.button.onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
            {
                Actor actor = Config.selectedUnit;
                WindowStatusEffectEditor.instance.display_status_button.load(status_button.data);
                WindowStatusEffectEditor.instance.selected_status_button = status_button;
                WindowStatusEffectEditor.instance.status_effect_time_text.text = ((int)(float)status_button.data.getRemainingTime()) + "s";
                WindowStatusEffectEditor.instance.status_effect_time_slider.value = Mathf.Log10((float)status_button.data.getRemainingTime()) / 5;
                WindowStatusEffectEditor.instance.reload_statuses();
            }));
            if (WindowStatusEffectEditor.instance.display_status_button.data == null)
            {
                WindowStatusEffectEditor.instance.display_status_button.load(status_button.data);
                WindowStatusEffectEditor.instance.selected_status_button = status_button;
            }
        }
    }
    internal class WindowStatusEffectEditor : MonoBehaviour
    {
        private Actor actor
        {
            get { return Config.selectedUnit; }
        }
        private static bool initialized = false;
        public static WindowStatusEffectEditor instance;
        private StatusEffectButton prefab_status_button;
        private UiUnitAvatarElement avatar_element;
        private StatusEffectGroupElement basic_statuses;
        private StatusEffectGroupElement advance_statuses;
        private Transform curr_statuses_transform;
        internal Slider status_effect_time_slider;
        internal Text status_effect_time_text;
        internal StatusEffectButton display_status_button;
        internal StatusEffectButton selected_status_button;
        public static void init(WindowCreatureInfo creature_window)
        {
            if (initialized) return;
            add_entry_button(creature_window);

            ScrollWindow scroll_window = NCMS.Utils.Windows.CreateNewWindow(C.status_effect_editor_id, "人物状态编辑器");
            //scroll_window.gameObject.SetActive(false);
            instance = scroll_window.gameObject.AddComponent<WindowStatusEffectEditor>();
            Transform background_transform = scroll_window.transform.Find("Background");
            background_transform.Find("Scroll View").gameObject.SetActive(true);
            Transform content_transform = background_transform.Find("Scroll View/Viewport/Content");

            Transform creature_content_transform = creature_window.transform.Find("Background/Scroll View/Viewport/Content");
            instance.display_status_button = GameObject.Instantiate(creature_window.prefabStatus, content_transform);
            instance.display_status_button.button.onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
            {
                Actor selected_actor = instance.actor;
                StatusEffectData data = instance.display_status_button.data;
                StatusEffectButton selected_button = instance.selected_status_button;

                if (selected_actor.hasStatus(data.asset.id))
                {
                    selected_actor.finishStatusEffect(data.asset.id);
                }
                else
                {
                    selected_actor.addStatusEffect(data.asset.id, (float)data.getRemainingTime());
                }
                selected_actor.updateStatusEffects(0);
                WindowStatusEffectEditor.instance.reload_statuses();
            }));
            instance.prefab_status_button = GameObject.Instantiate(creature_window.prefabStatus);
            GameObject SelectedIcon = new GameObject("SelectedIcon", typeof(Image));
            SelectedIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("ui/icons/iconWhiteCircle");
            SelectedIcon.GetComponent<Image>().color = new Color(0.584f, 0.867f, 0.365f, 1);
            SelectedIcon.SetActive(true);
            SelectedIcon.transform.SetParent(instance.prefab_status_button.transform);
            SelectedIcon.transform.localScale = new Vector3(0.33f, 0.33f);
            SelectedIcon.transform.SetAsFirstSibling();

            GameObject TopPart = new GameObject("TopPart", typeof(RectTransform));
            TopPart.transform.SetParent(content_transform);
            instance.avatar_element = GameObject.Instantiate(creature_window.avatarElement, TopPart.transform);
            TopPart.transform.localScale = new Vector3(1, 1);
            TopPart.GetComponent<RectTransform>().sizeDelta = new Vector2(193.28f, 26.30f);
            instance.avatar_element.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(48, 48);
            instance.avatar_element.transform.localPosition = new Vector3(-83.84f, 0.68f);
            instance.avatar_element.transform.localScale = new Vector3(0.6f, 0.6f);

            GameObject Statuses_Background = new GameObject("Statuses Background", typeof(Image));
            Statuses_Background.GetComponent<Image>().sprite = Helper.get_inner_sliced();
            Statuses_Background.GetComponent<Image>().type = Image.Type.Sliced;
            Statuses_Background.transform.SetParent(TopPart.transform);
            Statuses_Background.GetComponent<RectTransform>().sizeDelta = new Vector2(164.86f, 24);
            Statuses_Background.transform.localPosition = new Vector3(14.21f, 0.68f);
            Statuses_Background.transform.localScale = new Vector3(1, 1);
            GameObject statuses_title = new GameObject("Title", typeof(LocalizedText), typeof(Text));
            statuses_title.transform.SetParent(Statuses_Background.transform);
            statuses_title.GetComponent<LocalizedText>().key = "statuses";
            Text statuses_title_text = statuses_title.GetComponent<Text>();
            statuses_title_text.font = creature_window.text_description.font;
            statuses_title_text.fontStyle = FontStyle.Bold;
            statuses_title_text.resizeTextForBestFit = true;
            statuses_title_text.resizeTextMaxSize = 10;
            statuses_title_text.resizeTextMinSize = 1;
            statuses_title_text.color = new Color(1.000f, 0.607f, 0.110f, 0.180f);
            statuses_title_text.alignment = TextAnchor.MiddleCenter;
            statuses_title.GetComponent<LocalizedText>().text = statuses_title_text;
            statuses_title.transform.localPosition = new Vector3(0, 0);
            statuses_title.transform.localScale = new Vector3(1, 1);
            
            GameObject statuses_grid = new GameObject("Grid", typeof(GridLayoutGroup), typeof(FlexibleOneRowGrid));
            statuses_grid.transform.SetParent(Statuses_Background.transform);
            statuses_grid.transform.localPosition = new Vector3(0, 0);
            statuses_grid.transform.localScale = new Vector3(1, 1);
            statuses_grid.GetComponent<RectTransform>().sizeDelta = new Vector2(164.86f, 24);
            GridLayoutGroup statuses_grid_layout_group = statuses_grid.GetComponent<GridLayoutGroup>();
            statuses_grid_layout_group.cellSize = new Vector2(28, 28);
            statuses_grid_layout_group.spacing = new Vector2(0, 0);
            statuses_grid_layout_group.childAlignment = TextAnchor.MiddleLeft;
            instance.curr_statuses_transform = statuses_grid.transform;

            GameObject __statuses_group = new GameObject("Statuses Group", typeof(Image));
            __statuses_group.GetComponent<Image>().sprite = Helper.get_inner_sliced();
            __statuses_group.GetComponent<Image>().type = Image.Type.Sliced;
            RectTransform rect = __statuses_group.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(191.33f, 30f);
            GameObject __statuses_grid = new GameObject("Grid", typeof(StatusEffectGroupElement), typeof(GridLayoutGroup), typeof(ContentSizeFitter));
            __statuses_grid.transform.SetParent(__statuses_group.transform);
            StatusEffectGroupElement group_element = __statuses_grid.GetComponent<StatusEffectGroupElement>();
            GridLayoutGroup grid_layout_group = __statuses_grid.GetComponent<GridLayoutGroup>();
            grid_layout_group.cellSize = new Vector2(28, 28);
            grid_layout_group.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid_layout_group.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid_layout_group.constraintCount = 6;

            Transform basic_statuses = GameObject.Instantiate(__statuses_group, content_transform).transform;
            Transform advance_statuses = GameObject.Instantiate(__statuses_group, content_transform).transform;
            basic_statuses.gameObject.name = "Basic Statuses";
            advance_statuses.gameObject.name = "Advance Statuses";
            instance.basic_statuses = basic_statuses.Find("Grid").GetComponent<StatusEffectGroupElement>();
            instance.advance_statuses = advance_statuses.Find("Grid").GetComponent<StatusEffectGroupElement>();

            Text basic_statuses_text = new GameObject("Basic Statuses Title", typeof(Text)).GetComponent<Text>();
            basic_statuses_text.font = creature_window.text_description.font;
            basic_statuses_text.fontStyle = FontStyle.Bold;
            basic_statuses_text.fontSize = 24;
            basic_statuses_text.alignment = TextAnchor.MiddleCenter;
            basic_statuses_text.resizeTextForBestFit = false;
            basic_statuses_text.text = "基础";
            basic_statuses_text.color = new Color(0.847f, 0.847f, 0.847f, 1);
            basic_statuses_text.transform.SetParent(content_transform);
            basic_statuses_text.transform.localPosition = new Vector3(129.4f, -80.5f);
            instance.basic_statuses.title = basic_statuses_text;

            Text advance_statuses_text = new GameObject("Advance Statuses Title", typeof(Text)).GetComponent<Text>();
            advance_statuses_text.font = creature_window.text_description.font;
            advance_statuses_text.fontStyle = FontStyle.Bold;
            advance_statuses_text.fontSize = 24;
            advance_statuses_text.alignment = TextAnchor.MiddleCenter;
            advance_statuses_text.resizeTextForBestFit = false;
            advance_statuses_text.text = "高级";
            advance_statuses_text.color = new Color(1, 0.420f, 0.525f, 1);
            advance_statuses_text.transform.SetParent(content_transform);
            instance.advance_statuses.title = advance_statuses_text;

            foreach (StatusEffect status in AssetManager.status.list)
            {
                if(status.tier == StatusTier.Basic)
                {
                    instance.basic_statuses.add_status(status, instance.prefab_status_button);
                }
                else if(status.tier == StatusTier.Advanced)
                {
                    instance.advance_statuses.add_status(status, instance.prefab_status_button);
                }
            }
            RectTransform basic_statuses_rect = basic_statuses.GetComponent<RectTransform>();
            RectTransform advance_statuses_rect = advance_statuses.GetComponent<RectTransform>(); 



            content_transform.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 197+ (instance.basic_statuses.statuses.Count - 1) / 6 * 28 + (instance.advance_statuses.statuses.Count - 1) / 6 * 28);

            instance.display_status_button.transform.localPosition = new Vector3(45, -60);
            TopPart.transform.localPosition = new Vector3(129.4f, -30.94f);

            basic_statuses_rect.sizeDelta = new Vector2(191.33f, 30 + (instance.basic_statuses.statuses.Count-1) / 6 * 28);
            basic_statuses.transform.localPosition = new Vector3(129.4f, -103.5f - (instance.basic_statuses.statuses.Count - 1) / 6 * 14);
            basic_statuses_text.transform.localPosition = new Vector3(129.4f, basic_statuses.transform.localPosition.y + basic_statuses_rect.sizeDelta.y / 2 + 8);
            instance.basic_statuses.transform.localPosition = new Vector3(-78, basic_statuses_rect.sizeDelta.y / 2);

            advance_statuses_rect.sizeDelta = new Vector2(191.33f, 30 + (instance.advance_statuses.statuses.Count-1) / 6 * 28);
            advance_statuses.transform.localPosition = new Vector3(129.4f, basic_statuses.transform.localPosition.y - basic_statuses_rect.sizeDelta.y / 2 - 30f - advance_statuses_rect.sizeDelta.y / 2);
            advance_statuses_text.transform.localPosition = new Vector3(129.4f, advance_statuses.transform.localPosition.y + advance_statuses_rect.sizeDelta.y / 2 + 8);
            instance.advance_statuses.transform.localPosition = new Vector3(-78, advance_statuses_rect.sizeDelta.y / 2);



            GameObject effect_time_slider = new GameObject("effect_time_slider", typeof(Slider));
            GameObject slider_background = new GameObject("background", typeof(Image));
            GameObject slider_fill = new GameObject("fill", typeof(Image));
            GameObject slider_handle = new GameObject("handle", typeof(Image));
            GameObject slider_val_shower = new GameObject("val", typeof(Text));
            effect_time_slider.transform.SetParent(content_transform);
            slider_background.transform.SetParent(effect_time_slider.transform);
            slider_fill.transform.SetParent(effect_time_slider.transform);
            slider_handle.transform.SetParent(effect_time_slider.transform);
            slider_val_shower.transform.SetParent(content_transform);
            RectTransform slider_bg_rect = slider_background.GetComponent<RectTransform>();
            RectTransform slider_fill_rect = slider_fill.GetComponent<RectTransform>();
            RectTransform slider_handle_rect = slider_handle.GetComponent<RectTransform>();
            effect_time_slider.transform.localScale = new Vector3(1, 1);
            effect_time_slider.transform.localPosition = new Vector3(122f, -60.5f);
            Vector2 slider_size = new Vector2(135, 15);
            effect_time_slider.GetComponent<RectTransform>().sizeDelta = slider_size;
            slider_bg_rect.sizeDelta = slider_size;
            slider_fill_rect.sizeDelta = new Vector2(0, 0);
            slider_handle_rect.sizeDelta = new Vector2(slider_size.y, 0);
            slider_background.GetComponent<Image>().sprite = Helper.get_inner_sliced();
            slider_background.GetComponent<Image>().type = Image.Type.Sliced;
            slider_handle.GetComponent<Image>().sprite = Resources.Load<Sprite>("ui/icons/iconClock");
            slider_fill.GetComponent<Image>().sprite = Resources.Load<Sprite>("gt_windows/window_bar");
            slider_fill.GetComponent<Image>().color = Color.gray;
            Text slider_val_text = slider_val_shower.GetComponent<Text>();
            slider_val_text.text = "0s";
            slider_val_text.font = creature_window.text_description.font;
            slider_val_text.fontSize = 10;
            slider_val_text.fontStyle = FontStyle.Bold;
            slider_val_text.resizeTextForBestFit = false;
            slider_val_text.alignment = TextAnchor.MiddleRight;
            slider_val_text.color = new Color(1.000f, 0.607f, 0.110f, 1f);
            slider_val_text.alignment = TextAnchor.MiddleCenter;
            slider_val_text.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 20);
            slider_val_text.transform.localPosition = new Vector3(210.8f, -60.5f);
            slider_val_text.transform.localScale = new Vector3(0.8f, 0.8f);

            instance.status_effect_time_text = slider_val_text;
            instance.status_effect_time_slider = effect_time_slider.GetComponent<Slider>();
            instance.status_effect_time_slider.fillRect = slider_fill_rect;
            instance.status_effect_time_slider.handleRect = slider_handle_rect;
            instance.status_effect_time_slider.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>((float val) =>
            {
                StatusEffectData status_data = instance.display_status_button.data;
                if (status_data == null) return;
                float new_timer = Mathf.Pow(10, val * 5);

                if(new_timer > status_data.asset.duration)
                {
                    status_data.setTimer(status_data.asset.duration > new_timer ? status_data.asset.duration : new_timer);
                    Actor actor = instance.actor;
                    if (actor.hasStatus(status_data.asset.id))
                    {
                        actor.activeStatus_dict[status_data.asset.id].setTimer((float)status_data.getRemainingTime());
                    }
                }
                else
                {
                    instance.status_effect_time_slider.value = Mathf.Log10(status_data.asset.duration)/5;
                }
                slider_val_text.text = ((int)status_data.getRemainingTime()) + "s";

            }));

            initialized = true;
        }

        private static void add_entry_button(WindowCreatureInfo creature_window)
        {
            GameObject entry = GameObject.Instantiate(creature_window.buttonTraitEditor, creature_window.transform.Find("Background"));
            entry.transform.localPosition = new Vector3(116.8f, -34.5f);
            entry.transform.localScale = new Vector3(1, 1);
            entry.transform.Find("Button Trait/Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("ui/icons/iconMiracleBorn");
            Button button = entry.transform.Find("Button Trait").GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
            {
                ScrollWindow.showWindow(C.status_effect_editor_id);
            }));
            button.GetComponent<TipButton>().textOnClick = "statuses";
        }
        void OnEnable()
        {
            if (actor == null || !initialized) return;
            basic_statuses.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(10, 0);
            advance_statuses.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(10, 0);
            clear_statuses();
            instance.status_effect_time_slider.value = Mathf.Log10((float)instance.display_status_button.data.getRemainingTime()) / 5;
            this.avatar_element.avatarLoader.load(actor);
            load_current_actor_statuses();
        }
        internal void reload_statuses()
        {
            clear_statuses();
            load_current_actor_statuses();
        }
        private void clear_statuses()
        {
            int count = this.curr_statuses_transform.childCount;
            while (count > 0)
            {
                DestroyImmediate(this.curr_statuses_transform.GetChild(this.curr_statuses_transform.childCount-1).gameObject);
                count--;
            }
            foreach (StatusEffectButton button in StatusEffectGroupElement.all_buttons)
            {
                if(selected_status_button == null || button.data.asset.id != selected_status_button.data.asset.id)
                {
                    button.transform.Find("SelectedIcon").GetComponent<Image>().enabled = false;
                }
                else
                {
                    Image image = button.transform.Find("SelectedIcon").GetComponent<Image>();
                    image.enabled = true;
                    image.color = Color.white;
                }
            }
        }

        private void load_current_actor_statuses()
        {
            if (actor.activeStatus_dict != null)
            {
                foreach(StatusEffectData status in actor.activeStatus_dict.Values)
                {
                    load_status_button(status);
                }
            }
        }

        private void load_status_button(StatusEffectData status)
        {
            StatusEffectButton button = GameObject.Instantiate(this.prefab_status_button, this.curr_statuses_transform);
            button.load(status);
            button.name = status.asset.id;
            foreach (StatusEffectButton button_to_select in StatusEffectGroupElement.all_buttons)
            {
                if (button_to_select.data.asset.id != status.asset.id) continue;
                Image image = button_to_select.transform.Find("SelectedIcon").GetComponent<Image>();
                image.enabled = true;
                image.color = new Color(0.584f, 0.867f, 0.365f, 1);
            }

        }
    }
}
