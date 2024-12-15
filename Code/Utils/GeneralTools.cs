using UnityEngine;
using UnityEngine.UI;

namespace GodTools.Utils;

internal class GeneralTools
{
    public static Sprite get_inner_sliced()
    {
        return SpriteTextureLoader.getSprite("ui/special/windowInnerSliced");
    }

    public static Sprite get_button()
    {
        return SpriteTextureLoader.getSprite("ui/special/button");
    }

    public static void text_basic_setting(Text text)
    {
        text.font = LocalizedTextManager.currentFont;
        text.fontStyle = FontStyle.Bold;
        text.fontSize = 14;
        text.alignment = TextAnchor.MiddleCenter;
        text.resizeTextForBestFit = true;
        text.resizeTextMaxSize = 14;
        text.resizeTextMinSize = 2;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.text = "文本";
        text.color = new Color(1, 0.607f, 0.110f, 1);
    }
}