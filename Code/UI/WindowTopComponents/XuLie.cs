#if XULIE
using GodTools.UI.Prefabs;

// ReSharper disable CheckNamespace

namespace GodTools.UI;

public partial class WindowTops
{
    private void CreateGrid_XULIE()
    {
        TitledGrid guimi_keyword_grid = new_keyword_grid("guimi_mod");
        
        new_keyword(guimi_keyword_grid, "xulie", "XuLie", (a, b) =>
        {
            a.data.get("xulie.xuLieNum", out float a_xulie, 0f);
            b.data.get("xulie.xuLieNum", out float b_xulie, 0f);
            var res = a_xulie.CompareTo(b_xulie);
            return res;
        }, a =>
        {
            a.data.get("xulie.xuLieNum", out float a_xulie, 0f);
            return $"{a_xulie:F1} 灵性";
        });
        new_keyword(guimi_keyword_grid, "jin_bang", "JinBang", (a, b) =>
        {
            a.data.get("xulie.jinBangNum", out float a_jinbang, 0f);
            b.data.get("xulie.jinBangNum", out float b_jinbang, 0f);
            var res = a_jinbang.CompareTo(b_jinbang);
            return res;
        }, a =>
        {
            a.data.get("xulie.jinBangNum", out float a_jinbang, 0f);
            return $"{a_jinbang:F1} 金磅";
        });
    }
}
#endif