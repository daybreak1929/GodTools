using GodTools.UI.Prefabs;
// ReSharper disable CheckNamespace

namespace GodTools.UI;

public partial class WindowTops
{
    private void CreateGrid_INMNY_CUSTOMMODT001()
    {
        TitledGrid inmny_custommodt001_keyword_grid = new_keyword_grid("inmny_custommodt001");
        new_keyword(inmny_custommodt001_keyword_grid, "talent", "inmny/custommodt001/talent", (a, b) =>
        {
            a.data.get("inmny.custommodt001.talent", out float a_talent);
            b.data.get("inmny.custommodt001.talent", out float b_talent);
            return a_talent.CompareTo(b_talent);
        }, a =>
        {
            a.data.get("inmny.custommodt001.talent", out float talent);
            return $"{(int)talent} 武道天赋";
        });
        new_keyword(inmny_custommodt001_keyword_grid, "level", "inmny/custommodt001/cultilevel", (a, b) =>
        {
            a.data.get("inmny.custommodt001.wudao_level", out int a_level);
            b.data.get("inmny.custommodt001.wudao_level", out int b_level);
            var res = a_level.CompareTo(b_level);
            if (res == 0)
            {
                a.data.get("inmny.custommodt001.wudao_exp", out int a_exp);
                b.data.get("inmny.custommodt001.wudao_exp", out int b_exp);
                res = a_exp.CompareTo(b_exp);
            }

            return res;
        });
    }
}