using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GodTools.UI.Prefabs;

public class SavedActorDataCard : APrefab<SavedActorDataCard>
{
    private ActorData            _data;
    public  SavedActorDataAvatar Avatar { get; private set; }
    public  RawText              Name   { get; private set; }
    public  Button               Button { get; private set; }

    public void Setup(ActorData data, UnityAction on_click, UnityAction delete_action)
    {
        Init();

        _data = data;
        Avatar.Setup(data);
        Name.Setup(data.name);
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(on_click);
    }

    protected override void Init()
    {
        if (Initialized) return;
        base.Init();

        Avatar = transform.Find(nameof(Avatar)).GetComponent<SavedActorDataAvatar>();
        Name = transform.Find(nameof(Name)).GetComponent<RawText>();
        Button = GetComponent<Button>();
    }

    private static void _init()
    {
        GameObject obj = new(nameof(SavedActorDataCard), typeof(Image), typeof(Button));
        obj.transform.SetParent(Main.prefab_library);
        var rect_transform = obj.GetComponent<RectTransform>();

        var button = obj.GetComponent<Button>();
        button.OnHover(ShowTooltip);

        var bg = obj.GetComponent<Image>();
        bg.sprite = SpriteTextureLoader.getSprite("ui/special/windowInnerSliced");
        bg.type = Image.Type.Sliced;
        rect_transform.sizeDelta = new Vector2(180, 48);

        SavedActorDataAvatar avatar = Instantiate(SavedActorDataAvatar.Prefab, obj.transform);
        avatar.name = nameof(Avatar);
        avatar.transform.localPosition = new Vector3(-70, 0);
        avatar.transform.localScale = Vector3.one;
        avatar.SetSize(new Vector2(36, 36));


        RawText name = Instantiate(RawText.Prefab, obj.transform);
        name.SetSize(new Vector2(120, 12));
        name.name = nameof(Name);
        name.transform.localPosition = new Vector3(20, 12);
        name.transform.localScale = Vector3.one;


        Prefab = obj.AddComponent<SavedActorDataCard>();
    }

    private static void ShowTooltip()
    {
    }
}