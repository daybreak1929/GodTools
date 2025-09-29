using System;
using GodTools.Features;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.UI.Prefabs;

public class ConsistentCreaturePowerElement : APrefab<ConsistentCreaturePowerElement>
{
    protected override void Init()
    {
        if (Initialized) return;
        place.Button.onClick.RemoveAllListeners();
        place.Button.onClick.AddListener(selectTargetToPlace);
        inspect.Button.onClick.RemoveAllListeners();
        inspect.Button.onClick.AddListener(inspectTarget);
        base.Init();
    }

    public void Setup(ConsistentCreaturePowerTop.CreaturePowerData data, int rank, Action onClickPlace)
    {
        Init();
        this.onClickPlace = onClickPlace;
        _id = data.ID;
        icon.sprite = data.Sprite;
        title.text = data.Name;
        if (data.Power > 1e6)
        {
            power.text = $"{data.Power:g2}";
        }
        else
        {
            power.text = $"{data.Power:F1}";
        }
        if (World.world.units.get(data.ID) != null)
        {
            inspect.gameObject.SetActive(true);
        }
        else
        {
            inspect.gameObject.SetActive(false);
        }
        this.rank.text = $"{rank+1}";
        switch (rank)
        {
            case 0:
                frame.sprite = SpriteTextureLoader.getSprite("ui/special/windowAvatarElement_king");
                break;
            case 1:
                frame.sprite = SpriteTextureLoader.getSprite("ui/special/windowAvatarElement_leader");
                break;
            default:
                frame.sprite = SpriteTextureLoader.getSprite("ui/special/windowAvatarElement");
                break;
        }
    }
    private Action onClickPlace;
    private void inspectTarget()
    {
        ActionLibrary.openUnitWindow(World.world.units.get(_id));
    }

    private void selectTargetToPlace()
    {
        ConsistentCreaturePowerTop.SelectCreatureToPlace(_id);
        onClickPlace?.Invoke();
    }

    private long _id;
    
    [SerializeField]
    private Text rank;
    [SerializeField]
    private Image frame;
    [SerializeField]
    private Image icon;
    [SerializeField]
    private SimpleButton inspect;
    [SerializeField]
    private SimpleButton place;
    [SerializeField]
    private Text title;
    [SerializeField]
    private Text power;
    private static void _init()
    {
        var obj = new GameObject(nameof(ConsistentCreaturePowerElement), typeof(Image), typeof(HorizontalLayoutGroup));
        obj.transform.SetParent(Main.prefabs);
        obj.GetComponent<RectTransform>().sizeDelta = new Vector2(190, 48);
        obj.GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("ui/special/backgroundKingdomElement");

        var horizon_layout = obj.GetComponent<HorizontalLayoutGroup>();
        horizon_layout.childControlWidth = false;
        horizon_layout.childControlHeight = false;
        horizon_layout.childForceExpandWidth = false;
        horizon_layout.childForceExpandHeight = false;
        horizon_layout.spacing = 2;

        var avatar_item = new GameObject("Avatar", typeof(Image));
        avatar_item.transform.SetParent(obj.transform);
        avatar_item.GetComponent<RectTransform>().sizeDelta = new Vector2(48,48);
        avatar_item.GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("ui/special/windowAvatarElement");
        
        var avatar_frame = new GameObject("Background", typeof(Image));
        avatar_frame.transform.SetParent(avatar_item.transform);
        avatar_frame.GetComponent<RectTransform>().sizeDelta = new Vector2(48,48);
        avatar_frame.GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("ui/special/windowAvatarElement");

        var avatar_icon = new GameObject("Icon", typeof(Image));
        avatar_icon.transform.SetParent(avatar_item.transform);
        avatar_icon.GetComponent<RectTransform>().sizeDelta = new Vector2(32, 32);
        
        var avatar_rank = RawText.Instantiate(avatar_item.transform, pName: "Rank");
        avatar_rank.Setup("", Color.white, TextAnchor.MiddleCenter);

        var vert_group = new GameObject("VertGroup", typeof(VerticalLayoutGroup));
        vert_group.transform.SetParent(obj.transform);
        var vert_layout = vert_group.GetComponent<VerticalLayoutGroup>();
        vert_layout.childControlWidth = false;
        vert_layout.childControlHeight = false;
        vert_layout.childForceExpandWidth = false;
        vert_layout.childForceExpandHeight = false;
        vert_layout.spacing = 0;

        var top_part = new GameObject("TopPart", typeof(RectTransform));
        top_part.transform.SetParent(vert_group.transform);
        top_part.GetComponent<RectTransform>().sizeDelta = new(100, 24);
        top_part.GetComponent<RectTransform>().pivot = new(0, 0.5f);
        var name_text = RawText.Instantiate(top_part.transform, pName: "Title");
        var inspect_button = SimpleButton.Instantiate(top_part.transform, pName: "Inspect");
        
        var bottom_part = new GameObject("BottomPart", typeof(RectTransform));
        bottom_part.transform.SetParent(vert_group.transform);
        bottom_part.GetComponent<RectTransform>().sizeDelta = new(100, 24);
        bottom_part.GetComponent<RectTransform>().pivot = new(0, 0.5f);
        var power_text = RawText.Instantiate(bottom_part.transform, pName: "Power");
        var place_button = SimpleButton.Instantiate(bottom_part.transform, pName: "Place");

        var element = obj.AddComponent<ConsistentCreaturePowerElement>();
        element.icon = avatar_icon.GetComponent<Image>();
        element.inspect = inspect_button;
        element.place = place_button;
        element.title = name_text.Text;
        element.power = power_text.Text;
        element.rank = avatar_rank.Text;
        element.frame = avatar_frame.GetComponent<Image>();
        
        name_text.Setup("", Color.white, TextAnchor.MiddleLeft);
        name_text.RectTransform.pivot = new(0, 0.5f);
        name_text.RectTransform.sizeDelta = new(100, 24);
        name_text.RectTransform.localPosition = new(0, 0, 0);
        power_text.Setup("", Color.white, TextAnchor.MiddleLeft);
        power_text.RectTransform.pivot = new(0, 0.5f);
        power_text.RectTransform.sizeDelta = new(100, 24);
        power_text.RectTransform.localPosition = new(0, 0, 0);
        avatar_rank.RectTransform.sizeDelta = new(24,24);
        avatar_rank.RectTransform.pivot = new(0.5f, 0.5f);
        avatar_rank.RectTransform.localPosition = new(-32, 0, 0);
        inspect_button.Setup(element.inspectTarget, SpriteTextureLoader.getSprite("ui/icons/iconinspect"), pSize:new Vector2(16, 16));
        inspect_button.transform.localPosition = new(129, 0, 0);
        place_button.Setup(element.selectTargetToPlace, SpriteTextureLoader.getSprite("ui/icons/iconFinger"), pSize:new Vector2(16, 16));
        place_button.transform.localPosition = new(129, 0, 0);

        Prefab = element;
    }
}