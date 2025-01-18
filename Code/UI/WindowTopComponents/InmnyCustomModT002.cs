#if INMNY_CUSTOMMODT002
using System.Linq;
using CustomModT002;
using GodTools.UI.Prefabs;
using NeoModLoader.api.attributes;

// ReSharper disable CheckNamespace

namespace GodTools.UI;

public partial class WindowTops
{
    private void CreateGrid_INMNY_CUSTOMMODT002()
    {
        TitledGrid inmny_custommodt002_keyword_grid = new_keyword_grid("inmny_custommodt002");
        new_keyword(inmny_custommodt002_keyword_grid, "summon_count", "inmny/custommodt002/icon", [Hotfixable](a, b) =>
        {
            var a_count = 0;
            foreach (var trait in Traits.AllTraits)
            {
                a_count += a.GetSummonList(trait).Count;
            }

            var b_count = 0;
            foreach (var trait in Traits.AllTraits)
            {
                b_count += b.GetSummonList(trait).Count;
            }
            return a_count.CompareTo(b_count);
        }, a => $"{a.data.traits.Sum(trait => a.GetSummonList(trait).Count)} 只召唤物");
        new_keyword(inmny_custommodt002_keyword_grid, "summon_level", "inmny/custommodt002/icon", (a, b) =>
        {
            var a_count = a.GetSummonLevel();
            var b_count = b.GetSummonLevel();
            return a_count.CompareTo(b_count);
        }, a => $"{a.GetSummonLevelName()}");
    }
}
#endif