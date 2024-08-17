using NeoModLoader.api.attributes;
using UnityEngine;

namespace GodTools.Utils;

public static class UIHelper
{
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
}