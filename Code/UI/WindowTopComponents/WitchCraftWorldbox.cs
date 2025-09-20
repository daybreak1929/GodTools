#if WITCHCRAFT_WORLDBOX
using GodTools.UI.Prefabs;

// ReSharper disable CheckNamespace

namespace GodTools.UI;

public partial class WindowTops
{
    private void CreateGrid_WITCHCRAFT_WORLDBOX()
    {
        TitledGrid wushu_keyword_grid = new_keyword_grid("wushu");
        new_keyword(wushu_keyword_grid, "yuanneng", "ui/yuanneng", (a, b) =>
        {
            a.data.get("wushu.yuannengNum", out int a_level);
            b.data.get("wushu.yuannengNum", out int b_level);
            var res = a_level.CompareTo(b_level);
            return res;
        }, a =>
        {
            a.data.get("wushu.yuannengNum", out int a_level);
            return $"{a_level} 源能";
        });

        new_keyword(wushu_keyword_grid, "meditation", "ui/Meditation", (a, b) =>
        {
            a.data.get("wushu.meditationNum", out float a_meditation);
            b.data.get("wushu.meditationNum", out float b_meditation);
            var res = a_meditation.CompareTo(b_meditation);
            return res;
        }, a =>
        {
            a.data.get("wushu.meditationNum", out float a_meditation);
            return $"{a_meditation:F1} 无根之源";
        });
    }
}
#endif