#if WITCHCRAFT_WORLDBOX
using GodTools.UI.Prefabs;

// ReSharper disable CheckNamespace

namespace GodTools.UI;

public partial class WindowTops
{
    private void CreateGrid_WITCHCRAFT_WORLDBOX()
    {
        TitledGrid wushu_keyword_grid = new_keyword_grid("wushu");
        new_keyword(wushu_keyword_grid, "yuanneng", "ui/allactor_yuanneng", (a, b) =>
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
    }
}
#endif