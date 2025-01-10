using NeoModLoader.api.attributes;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.Utils;

public static class UITools
{
    public static Vector2 WorldToScreenPosition(Vector3 position, RectTransform canvas_rect)
    {
        Vector2 vector = World.world.camera.WorldToViewportPoint(position);
        return new Vector2(
            vector.x                                        * canvas_rect.sizeDelta.x -
            canvas_rect.sizeDelta.x * 0.5f,
            vector.y                                        * canvas_rect.sizeDelta.y -
            canvas_rect.sizeDelta.y * 0.5f);
    }

    [Hotfixable]
    public static Vector2 ClampPosition(Vector2 position, RectTransform rect_transform)
    {
        var x = Mathf.Clamp(
            position.x, rect_transform.position.x + rect_transform.rect.x * rect_transform.lossyScale.x,
            rect_transform.position.x +
            (rect_transform.rect.x + rect_transform.rect.width) * rect_transform.lossyScale.x);
        var y = Mathf.Clamp(
            position.y, rect_transform.position.y + rect_transform.rect.y * rect_transform.lossyScale.y,
            rect_transform.position.y +
            (rect_transform.rect.y + rect_transform.rect.height) * rect_transform.lossyScale.y);
        return new Vector2(x, y);
    }

    [Hotfixable]
    public static bool IsInRect(Vector2 position, RectTransform rect_transform)
    {
        return position.x >= rect_transform.position.x + rect_transform.rect.x * rect_transform.lossyScale.x &&
               position.x <= rect_transform.position.x +
               (rect_transform.rect.x + rect_transform.rect.width) * rect_transform.lossyScale.x             &&
               position.y >= rect_transform.position.y + rect_transform.rect.y * rect_transform.lossyScale.y &&
               position.y <= rect_transform.position.y +
               (rect_transform.rect.y + rect_transform.rect.height) * rect_transform.lossyScale.y;
    }

    [Hotfixable]
    public static Vector2 GetRelativePosition(Vector2 position, RectTransform rect_transform)
    {
        return new Vector2(
            (position.x - (rect_transform.position.x + rect_transform.rect.x * rect_transform.lossyScale.x)) /
            rect_transform.lossyScale.x,
            (position.y - (rect_transform.position.y + rect_transform.rect.y * rect_transform.lossyScale.y)) /
            rect_transform.lossyScale.y);
    }

    public static void SetSimpleVertLayout(this VerticalLayoutGroup layout, float spacing = 4)
    {
        layout.spacing = spacing;
        layout.childControlHeight = false;
        layout.childControlWidth = false;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = false;
    }
}