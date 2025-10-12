using System;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.UI.Prefabs;

public abstract class SavedMetaObjectCard<TCard, TObject, TData> : APrefab<TCard> 
    where TData : MetaObjectData, new()
    where TObject : MetaObject<TData>, new()
    where TCard : SavedMetaObjectCard<TCard, TObject, TData>
{
    public abstract void Setup(TData data, Action on_place, Action on_delete);

    protected static GameObject CreateBaseCardPrefab(string name)
    {
        var obj = new GameObject(name, typeof(Image));
        obj.transform.SetParent(Main.prefabs);

        var bg_img = obj.GetComponent<Image>();
        bg_img.sprite = SpriteTextureLoader.getSprite("ui/special/windowInnerSliced");
        bg_img.type = Image.Type.Sliced;
        
        obj.GetComponent<RectTransform>().sizeDelta = new(190, 24);

        return obj;
    }
}