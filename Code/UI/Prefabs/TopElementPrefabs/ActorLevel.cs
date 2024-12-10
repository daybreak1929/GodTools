using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.UI.Prefabs.TopElementPrefabs;

public class ActorLevel : APrefab<ActorLevel>
{
    public RawText Name { get; private set; }

    protected override void Init()
    {
        if (Initialized) return;
        base.Init();

        Name = transform.Find(nameof(Name)).GetComponent<RawText>();
    }

    public void Setup(Actor actor, int order)
    {
        Init();

        Name.Setup($"{order}, {actor.getName()}: {actor.data.level}-{actor.data.experience}");
    }

    private static void _init()
    {
        var obj = new GameObject(nameof(ActorLevel), typeof(Image));
        obj.transform.SetParent(Main.prefabs);

        var bg = obj.GetComponent<Image>();
        bg.sprite = SpriteTextureLoader.getSprite("ui/special/windowInnerSliced");
        bg.type = Image.Type.Sliced;

        var rect_transform = obj.GetComponent<RectTransform>();
        rect_transform.sizeDelta = new Vector2(160, 30);

        RawText name_text = Instantiate(RawText.Prefab, rect_transform);
        name_text.name = nameof(Name);
        name_text.SetSize(rect_transform.sizeDelta);
        name_text.transform.localPosition = Vector3.zero;
        name_text.transform.localScale = Vector3.one;


        Prefab = obj.AddComponent<ActorLevel>();
    }
}