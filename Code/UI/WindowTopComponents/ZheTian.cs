using GodTools.UI.Prefabs;
using NeoModLoader.General;

// ReSharper disable CheckNamespace

namespace GodTools.UI;

public partial class WindowTops
{
    private void CreateGrid_ZHETIAN()
    {
        TitledGrid zhetian_keyword_grid = new_keyword_grid("zhetian");
        new_keyword(zhetian_keyword_grid, "talent", "ui/icons/iconDamage", (a, b) =>
        {
            a.data.get("StartBoard.custommodt001.talent", out float a_talent);
            b.data.get("StartBoard.custommodt001.talent", out float b_talent);
            return a_talent.CompareTo(b_talent);
        }, a =>
        {
            a.data.get("StartBoard.custommodt001.talent", out float talent);
            return $"{(int)talent} 遮天天赋";
        });
        new_keyword(zhetian_keyword_grid, "level", "ui/icons/iconDamage", (a, b) =>
        {
            a.data.get("StartBoard.custommodt001.zhetian_level", out int a_level);
            b.data.get("StartBoard.custommodt001.zhetian_level", out int b_level);
            var res = a_level.CompareTo(b_level);
            if (res == 0)
            {
                a.data.get("StartBoard.custommodt001.zhetian_exp", out int a_exp);
                b.data.get("StartBoard.custommodt001.zhetian_exp", out int b_exp);
                res = a_exp.CompareTo(b_exp);
            }

            return res;
        }, a =>
        {
            a.data.get("StartBoard.custommodt001.zhetian_level", out int level);
            return LM.Get($"StartBoard.custommodt001.zhetian.{level}");
        });
    }
}