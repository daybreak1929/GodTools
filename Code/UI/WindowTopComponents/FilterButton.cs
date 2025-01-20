using Cultiway;
using GodTools.UI.Prefabs;
using NeoModLoader.api.attributes;
using NeoModLoader.General;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GodTools.UI.WindowTopComponents;

public class FilterButton : APrefab<FilterButton>
{
    private FilterSetting _filter;
    [SerializeField]
    private RawText _type;
    void Start()
    {
        var trigger_event = new EventTrigger.TriggerEvent();
        trigger_event.AddListener(OnClick);
        var trigger = GetComponent<EventTrigger>();
        trigger.triggers.Add( new EventTrigger.Entry()
        {
            eventID = EventTriggerType.PointerClick,
            callback = trigger_event
        });
    }
    public void Setup(FilterSetting filter)
    {
        _filter = filter;
        GetComponent<Image>().sprite = filter.Icon;
        GetComponent<TipButton>().textOnClick = filter.ID;
        _type.Text.text = LM.Get($"{C.mod_prefix}.ui.filter.{filter.Type.ToString().ToLower()}");
    }
    [Hotfixable]
    private void OnClick(BaseEventData eventData)
    {
        if (eventData is PointerEventData pointer_event_data)
        {
            if (pointer_event_data.button == PointerEventData.InputButton.Right)
            {
                _filter.ToBeRemoved = true;
            }
            else if (pointer_event_data.button == PointerEventData.InputButton.Left)
            {
                if (_filter == null) return;
                _filter.Type = (FilterType)(((int)_filter.Type + 1) % 3);
                _type.Text.text = LM.Get($"{C.mod_prefix}.ui.filter.{_filter.Type.ToString().ToLower()}");
            }
        }
    }

    private static void _init()
    {
        var obj = new GameObject(nameof(FilterButton), typeof(Image), typeof(Button), typeof(TipButton), typeof(EventTrigger));
        obj.transform.SetParent(Main.prefabs);
        
        
        RawText type = Instantiate(RawText.Prefab, obj.transform);
        type.name = nameof(_type);
        type.transform.localPosition = new Vector3(0, 8f);
        type.transform.localScale = Vector3.one;
        type.SetSize(new Vector2(30, 10));
        
        Prefab = obj.AddComponent<FilterButton>();
        Prefab._type = type;
        obj.GetComponent<TipButton>().textOnClickDescription = "inmny.godtools.ui.filter.right_click";
        
    }
}