using System;
using System.Collections.Generic;
using System.Linq;
using GodTools.Utils;
using NCMS.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.UI;

internal class ModifierEditorElement : MonoBehaviour
{
    public Text   text;
    public Button up;
    public Button down;
    public Button left;
    public Button right;
    public ItemAsset modifier;

    private void OnEnable()
    {
        if (text == null) return;
        up = transform.Find("Up").GetComponent<Button>();
        down = transform.Find("Down").GetComponent<Button>();
        left = transform.Find("Left").GetComponent<Button>();
        right = transform.Find("Right").GetComponent<Button>();
        up.onClick.RemoveAllListeners();
        up.onClick.AddListener(() =>
        {
            var same_type_modifiers = WindowItemEditor.instance.valid_modifiers[modifier.mod_type];
            load(same_type_modifiers[(same_type_modifiers.IndexOf(modifier) + 1) % same_type_modifiers.Count]);
        });
        down.onClick.RemoveAllListeners();
        down.onClick.AddListener(() =>
        {
            if (modifier.mod_rank == 1)
            {
                WindowItemEditor.instance.del_modifier(this);
                return;
            }

            var same_type_modifiers = WindowItemEditor.instance.valid_modifiers[modifier.mod_type];
            load(same_type_modifiers[
                (same_type_modifiers.IndexOf(modifier) + same_type_modifiers.Count - 1) % same_type_modifiers.Count]);
        });
        right.onClick.RemoveAllListeners();
        right.onClick.AddListener(() =>
        {
            WindowItemEditor.instance.replace_modifier(this,
                WindowItemEditor.instance.valid_modifiers[
                    WindowItemEditor.instance.cur_valid_modifier_types[0]][0]);
        });
        left.onClick.RemoveAllListeners();
        left.onClick.AddListener(() =>
        {
            WindowItemEditor.instance.replace_modifier(this,
                WindowItemEditor.instance.valid_modifiers[
                    WindowItemEditor.instance.cur_valid_modifier_types[
                        WindowItemEditor.instance.cur_valid_modifier_types.Count - 1]][0]);
        });
    }

    public void load(ItemAsset modifier, bool apply = true)
    {
        name = modifier.id;
        this.modifier = modifier;
        text.text = LocalizedTextManager.getText(modifier.translation_key) + " " + modifier.mod_rank;
        if (apply) WindowItemEditor.instance.apply_modifier(this.modifier);
    }
}

internal class NewItemButton : MonoBehaviour
{
    public ActorEquipmentSlot slot;

    private void OnEnable()
    {
        var button = GetComponent<Button>();
        if (button == null) return;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            var slot = this.slot;
            slot.data = new ItemData();
            switch (slot.type)
            {
                case EquipmentType.Weapon:
                    slot.data.id = "stick";
                    slot.data.material = "wood";
                    break;
                case EquipmentType.Helmet:
                case EquipmentType.Armor:
                case EquipmentType.Boots:
                case EquipmentType.Ring:
                case EquipmentType.Amulet:
                    slot.data.id = AssetManager.items.getEquipmentID(slot.type);
                    slot.data.material = "copper";
                    break;
                default:
                    throw new Exception("Unexpected Equipment Type");
            }

            WindowItemEditor.instance.replace_slot_with_item(this);
        });
        button.OnHover(() =>
        {
            Tooltip.show(gameObject, "normal", new TooltipData { tip_name = slot.type.ToString() });
        });
    }

    public void load(ActorEquipmentSlot slot)
    {
        this.slot = slot;
    }
}

internal class WindowItemEditor : MonoBehaviour
{
    private static readonly List<string> equipment_blacklist = new()
        { "claws", "jaws", "base", "hands", "fire_hands", "bite", "rocks", "snowball" };

    private static bool             initialized;
    public static  WindowItemEditor instance;
    public static  GameObject       entry_button;
    public         List<string>     cur_valid_modifier_types = new();

    private readonly List<EquipmentButton> equipment_buttons = new();
    private readonly List<ModifierEditorElement> modifier_elements = new();

    private readonly List<string> valid_material = new();
    private readonly List<ItemAsset> valid_templates = new();

    private Actor      actor;
    private InputField by_editor;
    private RectTransform content_rect;
    private GameObject cur_material_icon;

    private int cur_material_idx;
    private GameObject cur_template_icon;
    private int cur_template_idx;
    private GameObject empty_modifier_editor;
    private InputField from_editor;
    private Transform inner_bg;

    private Transform modifier_container;
    private GameObject modifier_editor_prefab;

    private InputField name_editor;
    private NewItemButton new_item_button_prefab;

    private ObjectPoolGenericMono<EquipmentButton> pool_equipment;
    private ObjectPoolGenericMono<ModifierEditorElement> pool_modifiers;
    private ObjectPoolGenericMono<NewItemButton> pool_new_item;
    private GameObject                           prefab_item;
    private EquipmentButton                      selected_equipment_button;
    public  Dictionary<string, List<ItemAsset>>  valid_modifiers = new();
    private InputField                           year_editor;

    private void OnEnable()
    {
        if (!initialized) return;
        actor = Config.selectedUnit;
        load_unit_items();
        valid_modifiers.Clear();
        foreach (var modifier in AssetManager.items_modifiers.list)
        {
            List<ItemAsset> list;
            if (!valid_modifiers.TryGetValue(modifier.mod_type, out list))
            {
                valid_modifiers[modifier.mod_type] = new List<ItemAsset>();
                list = valid_modifiers[modifier.mod_type];
            }

            list.Add(modifier);
        }

        foreach (var list in valid_modifiers.Values) list.Sort((a, b) => { return a.id.CompareTo(b.id); });
        valid_modifiers.Remove("normal");
        actor.dirty_sprite_item = true;
        actor.setStatsDirty();
    }

    private void OnDisable()
    {
        equipment_buttons.Clear();
        modifier_elements.Clear();
        if (actor != null)
        {
            actor.dirty_sprite_item = true;
            actor?.setStatsDirty();
        }
    }

    public static void init(WindowCreatureInfo creature_window)
    {
        if (initialized) return;
        add_entry_button(creature_window);
        var scroll_window = Windows.CreateNewWindow(C.item_editor_id, "装备编辑器");
        instance = scroll_window.gameObject.AddComponent<WindowItemEditor>();
        var background_transform = scroll_window.transform.Find("Background");

        Transform t;
        Button button;
        var viewport_transform = background_transform.Find("Scroll View/Viewport");
        viewport_transform.gameObject.SetActive(true);
        viewport_transform.GetComponent<RectTransform>().sizeDelta = new Vector2(0, -42.8f);
        viewport_transform.transform.localPosition = new Vector3(-129.4f, 44f);
        background_transform.Find("Scroll View").gameObject.SetActive(true);
        var content_transform = background_transform.Find("Scroll View/Viewport/Content");
        instance.content_rect = content_transform.GetComponent<RectTransform>();
        content_transform.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 260);
        //content_transform.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 500);
        var creature_content_transform = creature_window.transform.Find("Background/Scroll View/Viewport/Content");


        var item_display_grid =
            Instantiate(
                creature_window.transform.Find("Background/Scroll View/Viewport/Content/Part 2/Inventory Background")
                    .gameObject, background_transform);
        item_display_grid.transform.localPosition = new Vector3(20, 90f);

        var item_display_grid_transform = item_display_grid.transform.Find("Equipment Grid");
        item_display_grid.SetActive(false);
        var i = item_display_grid_transform.childCount - 1;
        while (i >= 0)
        {
            var child = item_display_grid_transform.GetChild(i).gameObject;
            child.SetActive(false);
            Destroy(child);
            i--;
        }

        item_display_grid.SetActive(true);

        instance.prefab_item = Instantiate(creature_window.prefabEquipment).gameObject;
        var select_item_bg = new GameObject("SelectBG", typeof(Image));
        select_item_bg.GetComponent<Image>().sprite = Resources.Load<Sprite>("ui/icons/iconWhiteCircle");
        select_item_bg.GetComponent<Image>().color = new Color(0.584f, 0.867f, 0.365f, 1);
        select_item_bg.GetComponent<Image>().enabled = false;
        select_item_bg.SetActive(true);
        select_item_bg.transform.SetParent(instance.prefab_item.transform);
        select_item_bg.transform.SetAsFirstSibling();
        select_item_bg.transform.localScale = new Vector3(0.3f, 0.3f);
        instance.prefab_item.SetActive(false);
        instance.pool_equipment =
            new ObjectPoolGenericMono<EquipmentButton>(instance.prefab_item.GetComponent<EquipmentButton>(),
                item_display_grid_transform);
        instance.selected_equipment_button = Instantiate(creature_window.prefabEquipment, background_transform);
        instance.selected_equipment_button.transform.localPosition = new Vector3(-75, 90);
        var item_display_bg = new GameObject("BG", typeof(Image));
        item_display_bg.GetComponent<Image>().sprite = Helper.get_inner_sliced();
        item_display_bg.GetComponent<Image>().type = Image.Type.Sliced;
        item_display_bg.transform.SetParent(instance.selected_equipment_button.transform);
        item_display_bg.transform.localPosition = new Vector3(0, 0);
        item_display_bg.transform.localScale = new Vector3(1.5f, 1.5f);
        item_display_bg.GetComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
        item_display_bg.transform.SetAsFirstSibling();

        var window_description = new GameObject("Description", typeof(Text));
        window_description.transform.SetParent(background_transform);
        window_description.transform.localPosition = new Vector3(4, 60);
        window_description.transform.localScale = new Vector3(0.8f, 0.8f);
        var description_text = window_description.GetComponent<Text>();
        Helper.text_basic_setting(description_text);
        description_text.resizeTextForBestFit = false;
        description_text.fontSize = 12;
        description_text.text = LocalizedTextManager.getText(C.item_editor_description);
        var rect = window_description.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(175, 50);


        var inner_bg = new GameObject("InnerBG", typeof(Image));
        inner_bg.GetComponent<Image>().sprite = Helper.get_inner_sliced();
        inner_bg.GetComponent<Image>().type = Image.Type.Sliced;
        inner_bg.GetComponent<Image>().enabled = false;
        inner_bg.transform.SetParent(content_transform);
        inner_bg.transform.localScale = new Vector3(1, 1);
        inner_bg.GetComponent<RectTransform>().sizeDelta = new Vector2(175, 350);
        var y_offset = (inner_bg.GetComponent<RectTransform>().sizeDelta.y - 150) / 2f;
        inner_bg.transform.localPosition = new Vector3(130, -83.5f - y_offset);
        instance.inner_bg = inner_bg.transform;

        var name_editor = new GameObject("Name", typeof(Image));
        name_editor.transform.SetParent(inner_bg.transform);
        name_editor.transform.localScale = new Vector3(1, 1);
        name_editor.transform.localPosition = new Vector3(0, y_offset + 60.4f);
        name_editor.GetComponent<Image>().sprite = Helper.get_inner_sliced();
        name_editor.GetComponent<Image>().type = Image.Type.Sliced;
        name_editor.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 15);
        var name_input_field = new GameObject("InputField", typeof(Text), typeof(InputField));
        name_input_field.transform.SetParent(name_editor.transform);
        name_input_field.transform.localPosition = new Vector3(0, 0);
        name_input_field.transform.localScale = new Vector3(1, 1);
        name_input_field.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 10);
        var name_input_text_component = name_input_field.GetComponent<Text>();
        name_input_field.GetComponent<InputField>().textComponent = name_input_text_component;
        Helper.text_basic_setting(name_input_text_component);
        name_input_text_component.supportRichText = true;
        instance.name_editor = name_input_field.GetComponent<InputField>();
        name_input_field.GetComponent<InputField>().onEndEdit.RemoveAllListeners();
        name_input_field.GetComponent<InputField>().onEndEdit.AddListener(text =>
        {
            var selected_equipment_button = instance.selected_equipment_button;
            if (selected_equipment_button == null || selected_equipment_button.item_data == null) return;
            selected_equipment_button.item_data.name = text;
        });

        var from_editor = Instantiate(name_editor, inner_bg.transform);
        from_editor.name = "From";
        from_editor.transform.localScale = new Vector3(1, 1);
        from_editor.transform.localPosition = new Vector3(-60, y_offset + 25);
        from_editor.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 15);
        t = from_editor.transform.Find("InputField");
        t.localPosition = new Vector3(0, 0);
        t.localScale = new Vector3(1, 1);
        t.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 15);
        instance.from_editor = t.GetComponent<InputField>();
        t.GetComponent<InputField>().onEndEdit.RemoveAllListeners();
        t.GetComponent<InputField>().onEndEdit.AddListener(text =>
        {
            var selected_equipment_button = instance.selected_equipment_button;
            if (selected_equipment_button == null || selected_equipment_button.item_data == null) return;
            selected_equipment_button.item_data.from = text;
        });
        var from_title = new GameObject("Title", typeof(Text));
        Helper.text_basic_setting(from_title.GetComponent<Text>());
        from_title.GetComponent<Text>().text = LocalizedTextManager.getText(C.item_from);
        from_title.transform.SetParent(from_editor.transform);
        from_title.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 15);
        from_title.transform.localPosition = new Vector3(0, 15);
        from_title.transform.localScale = new Vector3(0.8f, 0.8f);


        var by_editor = Instantiate(from_editor, inner_bg.transform);
        by_editor.name = "By";
        by_editor.transform.localScale = new Vector3(1, 1);
        by_editor.transform.localPosition = new Vector3(60, y_offset + 25);
        by_editor.transform.Find("Title").GetComponent<Text>().text = LocalizedTextManager.getText(C.item_by);
        t = by_editor.transform.Find("InputField");
        t.localPosition = new Vector3(0, 0);
        t.localScale = new Vector3(1, 1);
        t.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 15);
        instance.by_editor = t.GetComponent<InputField>();
        t.GetComponent<InputField>().onEndEdit.RemoveAllListeners();
        t.GetComponent<InputField>().onEndEdit.AddListener(text =>
        {
            var selected_equipment_button = instance.selected_equipment_button;
            if (selected_equipment_button == null || selected_equipment_button.item_data == null) return;
            selected_equipment_button.item_data.by = text;
        });
        t = by_editor.transform.Find("Title");
        t.localPosition = new Vector3(0, 15);
        t.localScale = new Vector3(0.8f, 0.8f);

        var year_editor = Instantiate(from_editor, inner_bg.transform);
        year_editor.name = "Year";
        year_editor.transform.localScale = new Vector3(1, 1);
        year_editor.transform.localPosition = new Vector3(0, y_offset + 25);
        year_editor.transform.Find("Title").GetComponent<Text>().text = LocalizedTextManager.getText(C.item_year);
        t = year_editor.transform.Find("InputField");
        t.localPosition = new Vector3(0, 0);
        t.localScale = new Vector3(1, 1);
        t.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 15);
        instance.year_editor = t.GetComponent<InputField>();
        t.GetComponent<InputField>().onEndEdit.RemoveAllListeners();
        t.GetComponent<InputField>().onEndEdit.AddListener(text =>
        {
            var selected_equipment_button = instance.selected_equipment_button;
            if (selected_equipment_button == null || selected_equipment_button.item_data == null) return;
            if (text.Any(ch => { return ch < '0' || ch > '9'; })) return;
            selected_equipment_button.item_data.year = Convert.ToInt32(text);
        });
        t = year_editor.transform.Find("Title");
        t.localPosition = new Vector3(0, 15);
        t.localScale = new Vector3(0.8f, 0.8f);


        var material_selector_title = new GameObject("Material Selector Title", typeof(Text));
        material_selector_title.transform.SetParent(inner_bg.transform);
        material_selector_title.transform.localScale = new Vector3(0.8f, 0.8f);
        material_selector_title.transform.localPosition = new Vector3(0, y_offset + 5);
        var material_selector_title_text = material_selector_title.GetComponent<Text>();
        Helper.text_basic_setting(material_selector_title_text);
        material_selector_title_text.text = LocalizedTextManager.getText(C.item_material);
        material_selector_title.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 25);

        var material_selector = new GameObject("Material Selector", typeof(Image));
        material_selector.GetComponent<Image>().sprite = Helper.get_inner_sliced();
        material_selector.GetComponent<Image>().type = Image.Type.Sliced;
        material_selector.transform.SetParent(inner_bg.transform);
        material_selector.transform.localScale = new Vector3(1, 1);
        material_selector.transform.localPosition = new Vector3(0, y_offset - 20);
        rect = material_selector.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(160, 25);

        var material_icon = new GameObject("Icon",        typeof(Image));
        var material_left_button = new GameObject("Left", typeof(Image), typeof(Button));
        var material_right_button = new GameObject("Right", typeof(Image), typeof(Button));
        instance.cur_material_icon = material_icon;

        material_left_button.GetComponent<Image>().sprite = Resources.Load<Sprite>("gt_windows/left_arrow");
        material_right_button.GetComponent<Image>().sprite = Resources.Load<Sprite>("gt_windows/right_arrow");

        material_icon.transform.SetParent(material_selector.transform);
        material_left_button.transform.SetParent(material_selector.transform);
        material_right_button.transform.SetParent(material_selector.transform);

        material_icon.transform.localPosition = new Vector3(0, 0);
        material_left_button.transform.localPosition = new Vector3(-60, 0);
        material_right_button.transform.localPosition = new Vector3(60, 0);

        material_icon.transform.localScale = new Vector3(1,        1);
        material_left_button.transform.localScale = new Vector3(1, 1);
        material_right_button.transform.localScale = new Vector3(1, 1);

        rect = material_icon.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(25, 25);

        rect = material_left_button.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(25, 25);

        rect = material_right_button.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(25, 25);

        button = material_left_button.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            var selected_equipment_button = instance.selected_equipment_button;
            if (selected_equipment_button == null || selected_equipment_button.item_data == null) return;
            instance.cur_material_idx = (instance.cur_material_idx + instance.valid_material.Count - 1) %
                                        instance.valid_material.Count;
            selected_equipment_button.item_data.material = instance.valid_material[instance.cur_material_idx];
            instance.update_material_selector();
            instance.update_template_selector();
            instance.reload_selected_equipment();
        });
        button = material_right_button.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            var selected_equipment_button = instance.selected_equipment_button;
            if (selected_equipment_button == null || selected_equipment_button.item_data == null) return;
            instance.cur_material_idx = (instance.cur_material_idx + instance.valid_material.Count + 1) %
                                        instance.valid_material.Count;
            selected_equipment_button.item_data.material = instance.valid_material[instance.cur_material_idx];
            instance.update_material_selector();
            instance.update_template_selector();
            instance.reload_selected_equipment();
        });
        var template_selector_title = Instantiate(material_selector_title, inner_bg.transform);
        template_selector_title.name = "Template Selector Title";
        template_selector_title.GetComponent<Text>().text = LocalizedTextManager.getText(C.item_template);
        template_selector_title.transform.localPosition = new Vector3(template_selector_title.transform.localPosition.x,
            template_selector_title.transform.localPosition.y - 50);
        var template_selector = Instantiate(material_selector, inner_bg.transform);
        template_selector.name = "Template Selector";
        template_selector.transform.localPosition = new Vector3(template_selector.transform.localPosition.x,
            template_selector.transform.localPosition.y - 50);
        instance.cur_template_icon = template_selector.transform.Find("Icon").gameObject;

        button = template_selector.transform.Find("Left").GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            var selected_equipment_button = instance.selected_equipment_button;
            if (selected_equipment_button == null || selected_equipment_button.item_data == null) return;
            instance.cur_template_idx = (instance.cur_template_idx + instance.valid_templates.Count - 1) %
                                        instance.valid_templates.Count;
            selected_equipment_button.item_data.id = instance.valid_templates[instance.cur_template_idx].id;
            instance.update_template_selector();
            instance.reset_valid_material();
            instance.reload_selected_equipment();
        });
        button = template_selector.transform.Find("Right").GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            var selected_equipment_button = instance.selected_equipment_button;
            if (selected_equipment_button == null || selected_equipment_button.item_data == null) return;
            instance.cur_template_idx = (instance.cur_template_idx + instance.valid_templates.Count + 1) %
                                        instance.valid_templates.Count;
            selected_equipment_button.item_data.id = instance.valid_templates[instance.cur_template_idx].id;
            instance.update_template_selector();
            instance.reset_valid_material();
            instance.reload_selected_equipment();
        });

        var modifier_editor_title = Instantiate(material_selector_title, inner_bg.transform);
        modifier_editor_title.name = "Modifier Editor Title";
        modifier_editor_title.GetComponent<Text>().text = LocalizedTextManager.getText(C.item_modifier);
        modifier_editor_title.transform.localPosition = new Vector3(modifier_editor_title.transform.localPosition.x,
            modifier_editor_title.transform.localPosition.y - 100);

        var modifier_editor_container = new GameObject("Modifier Editor Container", typeof(VerticalLayoutGroup));
        modifier_editor_container.transform.SetParent(content_transform);
        modifier_editor_container.transform.localScale = new Vector3(1, 1);
        modifier_editor_container.transform.localPosition = new Vector3(130, -205);
        modifier_editor_container.GetComponent<RectTransform>().sizeDelta = new Vector2(160, 30);
        modifier_editor_container.GetComponent<VerticalLayoutGroup>().spacing = 10;
        instance.modifier_container = modifier_editor_container.transform;

        var modifier_editor = new GameObject("Modifier Editor Element", typeof(Image), typeof(ModifierEditorElement));
        instance.modifier_editor_prefab = modifier_editor;
        modifier_editor.transform.SetParent(inner_bg.transform);
        modifier_editor.GetComponent<Image>().sprite = Helper.get_inner_sliced();
        modifier_editor.GetComponent<Image>().type = Image.Type.Sliced;
        rect = modifier_editor.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(160, 20);
        modifier_editor.transform.localScale = new Vector3(1, 1);

        var modifier_text = new GameObject("Text",          typeof(Text));
        var modifier_left_button = new GameObject("Left",   typeof(Image), typeof(Button));
        var modifier_right_button = new GameObject("Right", typeof(Image), typeof(Button));
        var modifier_level_up_button = new GameObject("Up", typeof(Image), typeof(Button));
        var modifier_level_down_button = new GameObject("Down", typeof(Image), typeof(Button));

        Helper.text_basic_setting(modifier_text.GetComponent<Text>());
        var mee = modifier_editor.GetComponent<ModifierEditorElement>();
        mee.text = modifier_text.GetComponent<Text>();
        mee.text.resizeTextMaxSize = 10;

        modifier_left_button.GetComponent<Image>().sprite = Resources.Load<Sprite>("gt_windows/left_arrow");
        modifier_right_button.GetComponent<Image>().sprite = Resources.Load<Sprite>("gt_windows/right_arrow");
        modifier_level_up_button.GetComponent<Image>().sprite = Resources.Load<Sprite>("gt_windows/up_arrow");
        modifier_level_down_button.GetComponent<Image>().sprite = Resources.Load<Sprite>("gt_windows/down_arrow");

        modifier_left_button.transform.SetParent(modifier_editor.transform);
        modifier_right_button.transform.SetParent(modifier_editor.transform);
        modifier_level_up_button.transform.SetParent(modifier_editor.transform);
        modifier_level_down_button.transform.SetParent(modifier_editor.transform);
        modifier_text.transform.SetParent(modifier_editor.transform);


        modifier_text.transform.localPosition = new Vector3(0,              0);
        modifier_left_button.transform.localPosition = new Vector3(-30,     0);
        modifier_right_button.transform.localPosition = new Vector3(30,     0);
        modifier_level_up_button.transform.localPosition = new Vector3(-60, 0);
        modifier_level_down_button.transform.localPosition = new Vector3(60, 0);

        modifier_text.transform.localScale = new Vector3(1,            1);
        modifier_left_button.transform.localScale = new Vector3(1,     1);
        modifier_right_button.transform.localScale = new Vector3(1,    1);
        modifier_level_up_button.transform.localScale = new Vector3(1, 1);
        modifier_level_down_button.transform.localScale = new Vector3(1, 1);

        rect = modifier_text.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(50, 25);

        rect = modifier_left_button.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(25, 25);

        rect = modifier_right_button.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(25, 25);

        rect = modifier_level_up_button.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(25, 25);

        rect = modifier_level_down_button.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(25, 25);
        instance.modifier_editor_prefab = modifier_editor;
        instance.pool_modifiers = new ObjectPoolGenericMono<ModifierEditorElement>(
            instance.modifier_editor_prefab.GetComponent<ModifierEditorElement>(), modifier_editor_container.transform);

        var empty_modifier_editor = new GameObject("Empty Modifier", typeof(Image));
        empty_modifier_editor.GetComponent<Image>().sprite = Helper.get_inner_sliced();
        empty_modifier_editor.GetComponent<Image>().type = Image.Type.Sliced;
        empty_modifier_editor.transform.SetParent(modifier_editor_container.transform);
        empty_modifier_editor.GetComponent<RectTransform>().sizeDelta = new Vector2(160, 25);
        empty_modifier_editor.transform.localScale = new Vector3(1, 1);
        var new_modifier_obj = new GameObject("New Modifier", typeof(Image), typeof(Button));
        new_modifier_obj.GetComponent<Image>().sprite = Resources.Load<Sprite>("gt_windows/plus");
        new_modifier_obj.GetComponent<Button>().onClick.AddListener(() => { instance.new_modifier(); });
        new_modifier_obj.transform.SetParent(empty_modifier_editor.transform);
        instance.empty_modifier_editor = empty_modifier_editor;

        var new_item = new GameObject("New Item", typeof(Image), typeof(Button), typeof(NewItemButton));
        new_item.GetComponent<Image>().sprite = Resources.Load<Sprite>("gt_windows/plus");
        instance.new_item_button_prefab = new_item.GetComponent<NewItemButton>();
        instance.pool_new_item =
            new ObjectPoolGenericMono<NewItemButton>(instance.new_item_button_prefab, item_display_grid_transform);

        scroll_window.gameObject.SetActive(false);
        initialized = true;
    }

    private static void add_entry_button(WindowCreatureInfo creature_window)
    {
        var entry = Instantiate(creature_window.buttonTraitEditor, creature_window.transform.Find("Background"));
        entry.transform.localPosition = new Vector3(116.8f, -74.5f);
        entry.transform.localScale = new Vector3(1, 1);
        entry.transform.Find("Button Trait/Icon").GetComponent<Image>().sprite =
            Resources.Load<Sprite>("gt_windows/iconItemEditor");
        var button = entry.transform.Find("Button Trait").GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(() => { ScrollWindow.showWindow(C.item_editor_id); });
        button.GetComponent<TipButton>().textOnClick = "equipments";
        entry_button = entry;
    }

    private void load_unit_items()
    {
        pool_equipment.clear();
        pool_modifiers.clear();
        pool_new_item.clear();
        //List<ActorEquipmentSlot> slots = ActorEquipment.getList(actor.equipment);
        if (actor.equipment == null) actor.equipment = new ActorEquipment();
        for (var i = EquipmentType.Weapon; i <= EquipmentType.Amulet; i++)
        {
            var slot = actor.equipment.getSlot(i);
            if (slot.data != null) load_item(slot.data);
            else
                load_item_adder(slot);
        }
    }

    private void load_item_adder(ActorEquipmentSlot slot)
    {
        pool_new_item.getNext().load(slot);
    }

    private void load_item(ItemData data)
    {
        var equipment_button = pool_equipment.getNext();
        equipment_button.load(data);
        equipment_button.GetComponent<Button>().onClick.AddListener(() => { instance.select_item(equipment_button); });
        equipment_buttons.Add(equipment_button);
    }

    private void select_item(EquipmentButton button)
    {
        foreach (var each_button in equipment_buttons)
            each_button.transform.Find("SelectBG").GetComponent<Image>().enabled = false;
        button.transform.Find("SelectBG").GetComponent<Image>().enabled = true;
        selected_equipment_button.load(button.item_data);
        by_editor.text = button.item_data.by;
        from_editor.text = button.item_data.from;
        year_editor.text = button.item_data.year.ToString();
        name_editor.text = button.item_data.name;

        valid_material.Clear();
        valid_templates.Clear();
        cur_valid_modifier_types.Clear();
        cur_valid_modifier_types.AddRange(valid_modifiers.Keys);

        var item_asset = AssetManager.items.get(button.item_data.id);

        valid_material.AddRange(item_asset.materials);
        switch (item_asset.equipmentType)
        {
            case EquipmentType.Weapon:
                foreach (var item in AssetManager.items.list)
                    if (item.equipmentType == EquipmentType.Weapon && !item.id.StartsWith("_") &&
                        !equipment_blacklist.Contains(item.id))
                        valid_templates.Add(item);
                break;
            case EquipmentType.Helmet:
                foreach (var item in AssetManager.items.list)
                    if (item.equipmentType == EquipmentType.Helmet && !item.id.StartsWith("_") &&
                        !equipment_blacklist.Contains(item.id))
                        valid_templates.Add(item);
                break;
            case EquipmentType.Armor:
                foreach (var item in AssetManager.items.list)
                    if (item.equipmentType == EquipmentType.Armor && !item.id.StartsWith("_") &&
                        !equipment_blacklist.Contains(item.id))
                        valid_templates.Add(item);
                break;
            case EquipmentType.Boots:
                foreach (var item in AssetManager.items.list)
                    if (item.equipmentType == EquipmentType.Boots && !item.id.StartsWith("_") &&
                        !equipment_blacklist.Contains(item.id))
                        valid_templates.Add(item);
                break;
            case EquipmentType.Amulet:
                foreach (var item in AssetManager.items.list)
                    if (item.equipmentType == EquipmentType.Amulet && !item.id.StartsWith("_") &&
                        !equipment_blacklist.Contains(item.id))
                        valid_templates.Add(item);
                break;
            case EquipmentType.Ring:
                foreach (var item in AssetManager.items.list)
                    if (item.equipmentType == EquipmentType.Ring && !item.id.StartsWith("_") &&
                        !equipment_blacklist.Contains(item.id))
                        valid_templates.Add(item);
                break;
            default:
                throw new Exception("Unexpected Equipment Type");
        }

        cur_material_idx = valid_material.IndexOf(selected_equipment_button.item_data.material);
        cur_template_idx = valid_templates.IndexOf(AssetManager.items.get(selected_equipment_button.item_data.id));

        update_material_selector();
        update_template_selector();
        update_modifier_editor();
    }

    private void update_modifier_editor()
    {
        pool_modifiers.clear();
        modifier_elements.Clear();
        foreach (var modifier_id in selected_equipment_button.item_data.modifiers)
        {
            cur_valid_modifier_types.Remove(AssetManager.items_modifiers.get(modifier_id).mod_type);
            var modifier_elm = pool_modifiers.getNext();
            modifier_elm.load(AssetManager.items_modifiers.get(modifier_id), false);
            modifier_elements.Add(modifier_elm);
        }

        update_modifier_editor_container();
    }

    private void update_modifier_editor_container()
    {
        var rect = modifier_container.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(160, 30 * (modifier_elements.Count + 1));
        content_rect.sizeDelta = new Vector2(0, 30 * (modifier_elements.Count + 1) + 230);
        inner_bg.transform.localPosition =
            new Vector3(130, -83.5f - (inner_bg.GetComponent<RectTransform>().sizeDelta.y - 150) / 2f);
        modifier_container.localPosition = new Vector3(130, -190 - 15 * (modifier_elements.Count + 1));
        empty_modifier_editor.transform.SetAsLastSibling();
    }

    private void update_template_selector()
    {
        if (!valid_templates[cur_template_idx].materials.Contains(selected_equipment_button.item_data.material))
            selected_equipment_button.item_data.material = valid_templates[cur_template_idx].materials[0];
        var icon_image = cur_template_icon.GetComponent<Image>();
        icon_image.sprite = valid_templates[cur_template_idx].getSprite(selected_equipment_button.item_data);
        if (icon_image.sprite == null)
        {
            valid_templates[cur_template_idx].cached_sprite = Resources.Load<Sprite>("ui/icons/iconWhiteCircle");
            icon_image.sprite = valid_templates[cur_template_idx].cached_sprite;
        }
    }

    private void reset_valid_material()
    {
        valid_material.Clear();
        valid_material.AddRange(AssetManager.items.get(selected_equipment_button.item_data.id).materials);
        cur_material_idx = valid_material.IndexOf(selected_equipment_button.item_data.material);
        update_material_selector();
    }

    private void update_material_selector()
    {
        if (!AssetManager.resources.dict.ContainsKey(valid_material[cur_material_idx]))
        {
            cur_material_icon.GetComponent<Image>().sprite = Resources.Load<Sprite>("ui/icons/iconWhiteCircle");
            return;
        }

        cur_material_icon.GetComponent<Image>().sprite =
            AssetManager.resources.get(valid_material[cur_material_idx]).getSprite();
    }

    public void reload_selected_equipment()
    {
        selected_equipment_button.load(selected_equipment_button.item_data);
        foreach (var equipment_button in equipment_buttons)
            if (equipment_button.item_data == selected_equipment_button.item_data)
            {
                equipment_button.load(equipment_button.item_data);
                return;
            }
    }

    private void new_modifier()
    {
        if (cur_valid_modifier_types.Count == 0) return;
        var modifier_elm = pool_modifiers.getNext();
        modifier_elm.load(valid_modifiers[cur_valid_modifier_types[0]][0]);
        modifier_elements.Add(modifier_elm);
        update_modifier_editor_container();
    }

    public void del_modifier(ModifierEditorElement modifier_elm)
    {
        cur_valid_modifier_types.Add(modifier_elm.modifier.mod_type);
        modifier_elements.Remove(modifier_elm);
        selected_equipment_button.item_data.modifiers.Clear();
        foreach (var _modifier_elm in modifier_elements)
            selected_equipment_button.item_data.modifiers.Add(_modifier_elm.modifier.id);
        modifier_elm.gameObject.SetActive(false);
        pool_modifiers._elements_total.Remove(modifier_elm);
        pool_modifiers._elements_inactive.Push(modifier_elm);
        update_modifier_editor_container();
    }

    public void apply_modifier(ItemAsset modifier)
    {
        cur_valid_modifier_types.Remove(modifier.mod_type);
        selected_equipment_button.item_data.modifiers.Clear();
        foreach (var modifier_elm in modifier_elements)
            selected_equipment_button.item_data.modifiers.Add(modifier_elm.modifier.id);
    }

    internal void replace_modifier(ModifierEditorElement modifierEditorElement, ItemAsset itemAsset)
    {
        cur_valid_modifier_types.Add(modifierEditorElement.modifier.mod_type);
        cur_valid_modifier_types.Remove(itemAsset.mod_type);

        modifierEditorElement.load(itemAsset);
        selected_equipment_button.item_data.modifiers.Clear();
        foreach (var modifier_elm in modifier_elements)
            selected_equipment_button.item_data.modifiers.Add(modifier_elm.modifier.id);
    }


    internal void replace_slot_with_item(NewItemButton newItemButton)
    {
        newItemButton.gameObject.SetActive(false);
        pool_new_item._elements_inactive.Push(newItemButton);
        load_item(newItemButton.slot.data);
    }
}