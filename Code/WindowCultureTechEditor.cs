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
    internal class WindowCultureTechEditor : MonoBehaviour
    {
        private static bool initialized = false; 
        public static WindowCultureTechEditor instance;
        public const string id = "culture_tech_editor";
        private GameObject ResearchBar;
        private GameObject CultureContainer;
        private GameObject LevelIcon;
        private GameObject Owned_TechCommon;
        private GameObject Owned_TechRare;
        private Text TitleCommon;
        private Text TitleRare;
        public static void init(CultureWindow culture_window)
        {
            if (initialized) return;
            initialized = true;
            add_entry_button(culture_window);
            ScrollWindow scroll_window = NCMS.Utils.Windows.CreateNewWindow(id, "文化科技编辑器");
            instance = scroll_window.gameObject.AddComponent<WindowCultureTechEditor>();
            Transform background_transform = scroll_window.transform.Find("Background");
            background_transform.Find("Scroll View").gameObject.SetActive(true);
            Transform content_transform = background_transform.Find("Scroll View/Viewport/Content");

            Transform culture_content_transform = culture_window.transform.Find("Background/Scroll View/Viewport/Content");

            instance.ResearchBar = GameObject.Instantiate(culture_content_transform.Find("ResearchBar").gameObject, background_transform);
            instance.CultureContainer = GameObject.Instantiate(culture_content_transform.Find("BannerBackground").gameObject, background_transform);
            instance.LevelIcon = GameObject.Instantiate(culture_content_transform.Find("Info Icons/Level").gameObject, background_transform);
            instance.Owned_TechCommon = GameObject.Instantiate(culture_content_transform.Find("Tech Common").gameObject, background_transform);
            instance.Owned_TechRare = GameObject.Instantiate(culture_content_transform.Find("Tech Rare").gameObject, background_transform);
            instance.TitleCommon = GameObject.Instantiate(culture_content_transform.Find("Title Common").gameObject, background_transform).GetComponent<Text>();
            instance.TitleRare = GameObject.Instantiate(culture_content_transform.Find("Title Rare").gameObject, background_transform).GetComponent<Text>();

            instance.ResearchBar.transform.localPosition = new Vector3(3, 90);
            instance.ResearchBar.transform.localScale = new Vector3(0.7f, 0.7f);
            instance.CultureContainer.transform.localPosition = new Vector3(-80, 90);
            instance.CultureContainer.transform.localScale = new Vector3(0.5f, 0.5f);
            instance.LevelIcon.transform.localPosition = new Vector3(55, 90);
            instance.LevelIcon.transform.Find("Icon").localPosition = new Vector3(20, 0);
            instance.LevelIcon.transform.Find("Text").localPosition = new Vector3(35.58f, 0);
            instance.Owned_TechCommon.transform.localPosition = new Vector3(0, 35);
            instance.Owned_TechRare.transform.localPosition = new Vector3(0, -20);
            instance.TitleCommon.transform.localPosition = new Vector3(0, 70.58f);
            instance.TitleRare.transform.localPosition = new Vector3(0, -2);
        }
        private static void add_entry_button(CultureWindow culture_window)
        {
            GameObject editor_entry = GameObject.Instantiate(culture_window.transform.Find("TabBg").gameObject);
            editor_entry.name = "TechEditorEntry";
            editor_entry.transform.Find("Cultures List/Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("ui/icons/iconDivineScar");
            editor_entry.transform.SetParent(culture_window.transform);
            editor_entry.transform.localPosition = new Vector3(117.50f, -40.56f);
            editor_entry.transform.localScale = new Vector3(1, 1);
            Button button = editor_entry.transform.Find("Cultures List").GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
            {
                ScrollWindow.showWindow(id);
            }));
            button.gameObject.GetComponent<EventTrigger>().triggers.Clear();
            button.OnHover(new UnityEngine.Events.UnityAction(() =>
            {
                Tooltip.show(editor_entry, "tip", new TooltipData() { tip_name = "文化科技编辑" });
            }));
        }
    }
}
