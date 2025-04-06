using System.Text.RegularExpressions;
using GodTools.Utils;
using NeoModLoader.api.attributes;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GodTools.UI.Prefabs;

public class CreatureTitleItem : APrefab<CreatureTitleItem>
{
    public TextInput Input => input;
    
    [SerializeField]
    private TextInput input;
    
    public SimpleButton DeleteButton => deleteButton;
    [SerializeField]
    private SimpleButton deleteButton;

    [SerializeField]
    private SimpleButton colorButton;
    [SerializeField]
    private SimpleButton boldButton;
    [SerializeField]
    private SimpleButton italicButton;
    [SerializeField]
    private SimpleButton sizeIncButton;
    [SerializeField]
    private SimpleButton sizeDecButton;
    private Actor _target;
    private string _title_uid;
    private bool _bold;
    private bool _italic;
    private int _size = 8;
    private Color _color = Color.white;

    private void UpdateTitle(string s=null)
    {
        var unwrapped = UnwrapText(s??input.input.text);
        var wrapped = WrapText(unwrapped);
        _target.data.set(_title_uid, wrapped);
        input.input.SetTextWithoutNotify(wrapped);
    }
    protected override void Init()
    {
        input.Setup("头衔", UpdateTitle);
        deleteButton.Setup(() =>
        {
            _target.data.removeString(_title_uid);
            _target.RemoveTitleGuid(_title_uid);
            _on_delete?.Invoke(gameObject);
        }, SpriteTextureLoader.getSprite("ui/icons/iconClose"), pSize: new(18, 18));
        colorButton.Setup([Hotfixable]() =>
        {
            var color = new Color(Randy.randomFloat(0,1), Randy.randomFloat(0,1), Randy.randomFloat(0,1), Randy.randomFloat(0,1));
            _color = color;
            UpdateTitle();
        }, SpriteTextureLoader.getSprite("ui/icons/iconCustomWorld"), pSize: new(18, 18));
        boldButton.Setup([Hotfixable]() =>
        {
            _bold = !_bold;
            UpdateTitle();
        }, SpriteTextureLoader.getSprite("ui/icons/iconBold"), pSize: new(18, 18));
        italicButton.Setup([Hotfixable]() =>
        {
            _italic = !_italic;
            UpdateTitle();
        }, SpriteTextureLoader.getSprite("ui/icons/iconItalic"), pSize: new(18, 18));
        sizeIncButton.Setup([Hotfixable]() =>
        {
            _size++;
            UpdateTitle();
        }, SpriteTextureLoader.getSprite("gt_windows/up_arrow"), pSize: new(18, 18));
        sizeDecButton.Setup([Hotfixable]() =>
        {
            _size--;
            UpdateTitle();
        }, SpriteTextureLoader.getSprite("gt_windows/down_arrow"), pSize: new(18, 18));
        input.text.resizeTextForBestFit = false;
        input.text.fontSize = 8;
        input.text.verticalOverflow = VerticalWrapMode.Overflow;
    }

    private string UnwrapText(string text)
    { 
        if (string.IsNullOrEmpty(text))
            return text;
        return Regex.Replace(text, @"<.*?>", string.Empty);
    }
    [Hotfixable]
    private string WrapText(string text)
    {
        var result = text;
        if (_bold)
        {
            result = $"<b>{result}</b>";
        }
        if (_italic)
        {
            result = $"<i>{result}</i>";
        }

        if (_color != Color.white)
        {
            result = $"<color=#{ColorUtility.ToHtmlStringRGB(_color)}>{result}</color>";
        }
        if (_size != 8)
            result = $"<size={_size}>{result}</size>";
        
        return result;
    }
    private UnityAction<GameObject> _on_delete;
    [Hotfixable]
    public void Setup(Actor actor, string title_uid, UnityAction<GameObject> on_delete)
    {
        Init();
        _target = actor;
        _title_uid = title_uid;
        _on_delete = on_delete;
        input.input.SetTextWithoutNotify(actor.GetTitle(title_uid));
    }
    [Hotfixable]
    public override void SetSize(Vector2 pSize)
    {
        base.SetSize(pSize);
        topPart.sizeDelta = new(pSize.x, pSize.y / 2-1);
        bottomPart.sizeDelta = new(pSize.x, pSize.y / 2-1);
        var icon_size = new Vector2((pSize.y-8) / 2, (pSize.y-8) / 2);
        colorButton.SetSize(icon_size);
        boldButton.SetSize(icon_size);
        italicButton.SetSize(icon_size);
        sizeIncButton.SetSize(icon_size);
        sizeDecButton.SetSize(icon_size);
        input.SetSize(new(pSize.x - icon_size.x - 8, icon_size.y));
        deleteButton.SetSize(icon_size);
    }

    private static void _init()
    {
        var obj = new GameObject(nameof(CreatureTitleItem), typeof(Image), typeof(VerticalLayoutGroup));
        obj.transform.SetParent(Main.prefabs);
        obj.GetComponent<RectTransform>().sizeDelta = new(190, 48);
        
        var vertical_layout = obj.GetComponent<VerticalLayoutGroup>();
        vertical_layout.childControlWidth = false;
        vertical_layout.childControlHeight = false;
        vertical_layout.childForceExpandWidth = false;
        vertical_layout.childForceExpandHeight = false;
        vertical_layout.spacing = 2;
        
        var top_part = new GameObject("TopPart", typeof(HorizontalLayoutGroup));
        top_part.transform.SetParent(obj.transform);
        top_part.transform.localScale = Vector3.one;
        var layout = top_part.GetComponent<HorizontalLayoutGroup>();
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.spacing = 4;
        var bottom_part = Instantiate(top_part, obj.transform);
        bottom_part.name = "BottomPart";
        bottom_part.transform.localScale = Vector3.one;
        
        var bg = obj.GetComponent<Image>();
        bg.sprite = SpriteTextureLoader.getSprite("ui/special/windowInnerSliced");
        bg.type = Image.Type.Sliced;

        var color_button = SimpleButton.Instantiate(bottom_part.transform, pName: "ColorButton");
        var bold_button = SimpleButton.Instantiate(bottom_part.transform, pName: "BoldButton");
        var italic_button = SimpleButton.Instantiate(bottom_part.transform, pName: "ItalicButton");
        var size_inc_button = SimpleButton.Instantiate(bottom_part.transform, pName: "SizeIncButton");
        var size_dec_button = SimpleButton.Instantiate(bottom_part.transform, pName: "SizeDecButton");
        var input = TextInput.Instantiate(top_part.transform, pName: nameof(Input));
        var delete_button = SimpleButton.Instantiate(top_part.transform, pName: "DeleteButton");

        Prefab = obj.AddComponent<CreatureTitleItem>();
        Prefab.input = input;
        Prefab.deleteButton = delete_button;
        Prefab.colorButton = color_button;
        Prefab.boldButton = bold_button;
        Prefab.italicButton = italic_button;
        Prefab.sizeIncButton = size_inc_button;
        Prefab.sizeDecButton = size_dec_button;
        Prefab.topPart = top_part.GetComponent<RectTransform>();
        Prefab.bottomPart = bottom_part.GetComponent<RectTransform>();
    }
    [SerializeField]
    private RectTransform topPart;
    [SerializeField]
    private RectTransform bottomPart;
}