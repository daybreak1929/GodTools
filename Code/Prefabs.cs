using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace GodTools.Code
{
    public class ActionWithCostButton : MonoBehaviour
    {
        public TipButton tip_button;
        public Button button;
        public Image icon;
        private TooltipData _tooltip_data;
        private bool _initialized = false;

        private void Awake()
        {
            if(_initialized) return;
            _initialized = true;
            tip_button.hoverAction = () =>
            {
                Tooltip.show(this.gameObject, C.action_with_cost_tooltip, _tooltip_data);
            };
            button.OnHover(() =>
            {
                icon.transform.localScale = new(1.1f, 1.1f);
                icon.transform.DOKill(true);
                icon.transform.DOScale(1f, 0.1f).SetEase(Ease.InBack);
            });
        }

        public void load(Sprite button_icon, Vector2 size, TooltipData tooltip_data, UnityAction action)
        {
            icon.sprite = button_icon;
            transform.GetComponent<RectTransform>().sizeDelta = size;
            icon.GetComponent<RectTransform>().sizeDelta = size - new Vector2(2, 2);
            _tooltip_data = tooltip_data;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action);
        }
    }
    public static class Prefabs
    {
        /// <summary>
        /// 带资源消耗量的行为按钮
        /// </summary>
        public static ActionWithCostButton action_with_cost_button_prefab;
        public static Tooltip action_with_cost_tooltip_prefab;
        
        public static void init()
        {
            set_action_with_cost_button();
            set_action_with_cost_tooltip_prefab();
        }

        private static void set_action_with_cost_tooltip_prefab()
        {
            action_with_cost_tooltip_prefab =
                Object.Instantiate(Resources.Load<Tooltip>("tooltips/tooltip_normal"), Main.prefab_library);
            action_with_cost_tooltip_prefab.gameObject.name = C.action_with_cost_tooltip;
        }

        private static void set_action_with_cost_button()
        {
            GameObject action_with_cost_button_obj = new GameObject("ActionWithCostButton Prefab", typeof(Image), typeof(Button));
            action_with_cost_button_obj.transform.SetParent(Main.prefab_library);
            action_with_cost_button_obj.SetActive(false);
            action_with_cost_button_obj.GetComponent<Image>().sprite = Helper.get_button();
            action_with_cost_button_obj.GetComponent<Image>().type = Image.Type.Sliced;
            action_with_cost_button_obj.GetComponent<RectTransform>().sizeDelta = new Vector2(36, 36);
            action_with_cost_button_prefab = action_with_cost_button_obj.AddComponent<ActionWithCostButton>();
            action_with_cost_button_prefab.tip_button = action_with_cost_button_obj.AddComponent<TipButton>();
            action_with_cost_button_prefab.tip_button.type = C.action_with_cost_tooltip;
            action_with_cost_button_prefab.button = action_with_cost_button_obj.GetComponent<Button>();
            
            GameObject button_icon = new GameObject("Icon", typeof(Image));
            button_icon.transform.SetParent(action_with_cost_button_obj.transform);
            button_icon.transform.localScale = new(1, 1);
            button_icon.transform.localPosition = new(0, 0);
            button_icon.GetComponent<RectTransform>().sizeDelta = new(34, 34);
            button_icon.GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("ui/icons/iconAbout");
            action_with_cost_button_prefab.icon = button_icon.GetComponent<Image>();
        }
    }
}