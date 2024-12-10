using NeoModLoader.api.attributes;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.UI.Prefabs;

public class SavedActorDataAvatar : APrefab<SavedActorDataAvatar>
{
    public Image image { get; private set; }

    private static void _init()
    {
        GameObject obj = new(nameof(SavedActorDataAvatar), typeof(Image));
        obj.transform.SetParent(Main.prefabs);
        Prefab = obj.AddComponent<SavedActorDataAvatar>();
    }

    protected override void Init()
    {
        if (Initialized) return;
        base.Init();
        image = GetComponent<Image>();
    }

    [Hotfixable]
    public void Setup(ActorData data)
    {
        Init();
        image.sprite = GetSprite(data);
    }

    private Sprite GetSprite(ActorData data)
    {
        ActorAsset asset = AssetManager.actor_library.get(data.asset_id);
        if (asset == null) return SpriteTextureLoader.getSprite("ui/icons/iconQuestionMark");

        return SpriteTextureLoader.getSprite($"ui/icons/{asset.icon}");
    }
}