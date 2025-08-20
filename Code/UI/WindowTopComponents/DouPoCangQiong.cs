using GodTools.UI.Prefabs;
using NeoModLoader.General;

// ReSharper disable CheckNamespace

namespace GodTools.UI;

public partial class WindowTops
{
    private void CreateGrid_DOUPOCANGQIONG()
    {
        TitledGrid zhetian_keyword_grid = new_keyword_grid("doupocangqiong");
        new_keyword(zhetian_keyword_grid, "talent", "ui/icons/iconDamage", (a, b) =>
        {
            a.data.get("d_talent", out float a_talent);
            b.data.get("d_talent", out float b_talent);
            return a_talent.CompareTo(b_talent);
        }, a =>
        {
            a.data.get("d_talent", out float talent);
            return $"{(int)talent} 斗破天赋";
        });
        new_keyword(zhetian_keyword_grid, "level", "ui/icons/iconDamage", (a, b) =>
        {
            a.data.get("d_level", out int a_level);
            b.data.get("d_level", out int b_level);
            var res = a_level.CompareTo(b_level);
            if (res == 0)
            {
                a.data.get("d_exp", out int a_exp);
                b.data.get("d_exp", out int b_exp);
                res = a_exp.CompareTo(b_exp);
            }

            return res;
        }, a =>
        {
            a.data.get("d_level", out int level);
            return LM.Get($"trait_Grade{level}");
        });
    }
}