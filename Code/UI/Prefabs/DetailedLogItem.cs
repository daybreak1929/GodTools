using System;
using NeoModLoader.api.attributes;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.UI.Prefabs;

public class DetailedLogItem : APrefab<DetailedLogItem>
{
    public RawText Date => date;
    [SerializeField]
    private RawText date;
    
    public RawText Message => message;
    [SerializeField]
    private RawText message;

    public SimpleButton Inspect => inspect;
    [SerializeField]
    private SimpleButton inspect;
    
    public SimpleButton Focus => focus;
    [SerializeField]
    private SimpleButton focus;
    public Image TypeIcon => typeIcon;
    [SerializeField]
    private Image typeIcon;
    public RectTransform RectTransform => GetComponent<RectTransform>();

    private void Start()
    {
        date.Text.resizeTextMaxSize = 6;
        date.Text.resizeTextMinSize = 1;
        message.Text.resizeTextMinSize = 4;
        message.Text.resizeTextMaxSize = 6;
        inspect.Setup(TryInspect, SpriteTextureLoader.getSprite("ui/icons/iconWorldInfo"), pSize: new(16,16));
        focus.Setup(TryFocus, SpriteTextureLoader.getSprite("ui/icons/iconGPS"), pSize: new(16,16));
    }

    public override void SetSize(Vector2 pSize)
    {
        throw new NotImplementedException();
    }
    private WorldLogMessage _msg;
    private Action<WorldLogMessage> _inspect_action;
    [Hotfixable]
    public void Setup(WorldLogMessage msg, Action<WorldLogMessage> inspect_action)
    {
        _msg = msg;
        date.Value = $"y: {(global::Date.getYear(msg.timestamp))}, m: {(global::Date.getMonth(msg.timestamp))}";
        message.Value = msg.getFormatedText(message.Text);
        var icon_path = msg.getAsset().path_icon;
        if (string.IsNullOrEmpty(icon_path))
        {
            typeIcon.gameObject.SetActive(false);
        }
        else
        {
            typeIcon.gameObject.SetActive(true);
            typeIcon.sprite = SpriteTextureLoader.getSprite(icon_path);
        }
        _inspect_action = inspect_action;
    }
    private static void _init()
    {
        var obj = new GameObject(nameof(DetailedLogItem), typeof(Image));
        obj.transform.SetParent(Main.prefabs);
        obj.GetComponent<RectTransform>().sizeDelta = new(190, 24);
        
        var bg = obj.GetComponent<Image>();
        bg.sprite = SpriteTextureLoader.getSprite("ui/special/windowInnerSliced");
        bg.type = Image.Type.Sliced;
        
        var type_icon = new GameObject("TypeIcon", typeof(Image));
        type_icon.transform.SetParent(obj.transform);
        var icon_rect = type_icon.GetComponent<RectTransform>();
        icon_rect.anchorMin = new(0, 0.5f);
        icon_rect.anchorMax = new(0, 0.5f);
        icon_rect.anchoredPosition = new(8, 0);
        icon_rect.sizeDelta = new(16, 16);
        type_icon.GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("ui/icons/iconAbout");
        
        var message = RawText.Instantiate(obj.transform, pName: nameof(Message));
        message.RectTransform.anchorMin = new(0, 0.5f);
        message.RectTransform.anchorMax = new(0, 0.5f);
        message.RectTransform.pivot = new(0, 0.5f);
        message.RectTransform.anchoredPosition = new(16, 0);
        message.RectTransform.sizeDelta = new(108, 16);
        message.Text.alignment = TextAnchor.MiddleLeft;

        
        var right_part = new GameObject("RightPart", typeof(HorizontalLayoutGroup), typeof(ContentSizeFitter));
        right_part.transform.SetParent(obj.transform);
        var layout = right_part.GetComponent<HorizontalLayoutGroup>();
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;
        layout.spacing = 4;
        layout.childAlignment = TextAnchor.MiddleRight;
        right_part.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        right_part.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        var right_rect = right_part.GetComponent<RectTransform>();
        right_rect.anchorMin = new(1, 0.5f);
        right_rect.anchorMax = new(1, 0.5f);
        right_rect.anchoredPosition = new(-4, 0);
        right_rect.pivot = new(1, 0.5f);
        
        
        var date_obj = new GameObject("Date", typeof(Image), typeof(HorizontalLayoutGroup));
        date_obj.transform.SetParent(right_part.transform);
        date_obj.GetComponent<RectTransform>().sizeDelta = new(30, 12);
        var date_bg = date_obj.GetComponent<Image>();
        date_bg.sprite = SpriteTextureLoader.getSprite("ui/special/windowInnerSliced");
        date_bg.type = Image.Type.Sliced;
        date_bg.color = new(0.641f, 0.641f, 0.641f, 1);
        var date = RawText.Instantiate(date_obj.transform, pName: nameof(Date));
        date.Text.alignment = TextAnchor.MiddleCenter;
        date.Color = new(1, 0.6073f, 0.1103f, 1);
        date.GetComponent<RectTransform>().sizeDelta = new(30, 12);
        
        var inspect = SimpleButton.Instantiate(right_part.transform, pName: nameof(Inspect));
        
        var focus = SimpleButton.Instantiate(right_part.transform, pName: nameof(Focus));
        
        Prefab = obj.AddComponent<DetailedLogItem>();
        Prefab.typeIcon = type_icon.GetComponent<Image>();
        Prefab.date = date;
        Prefab.message = message;
        Prefab.inspect = inspect;
        Prefab.focus = focus;
    }
    private void TryFocus()
    {
        if (_msg.hasFollowLocation())
        {
            ScrollWindow.moveAllToLeftAndRemove();
            _msg.followLocation();
        }
        else if (_msg.hasLocation())
        {
            ScrollWindow.moveAllToLeftAndRemove();
            _msg.jumpToLocation();
        }
    }
    [Hotfixable]
    private void TryInspect()
    {
        _inspect_action?.Invoke(_msg);
    }
}