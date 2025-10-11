using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.Utils;

internal static class GeneralTools
{
    public static string GetDate(this WorldLogMessage msg)
    {
        return $"y:{Date.getYear(msg.timestamp)}, m:{Date.getMonth(msg.timestamp)}";
    }
    public static Sprite get_inner_sliced()
    {
        return SpriteTextureLoader.getSprite("ui/special/windowInnerSliced");
    }

    public static Sprite get_button()
    {
        return SpriteTextureLoader.getSprite("ui/special/button");
    }
    public static readonly JsonSerializerSettings private_members_visit_settings = new()
    {
        ContractResolver = new DefaultContractResolver
        {
#pragma warning disable 618
            DefaultMembersSearchFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
#pragma warning restore 618
        }
    };
    public static void text_basic_setting(Text text)
    {
        text.font = LocalizedTextManager.current_font;
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