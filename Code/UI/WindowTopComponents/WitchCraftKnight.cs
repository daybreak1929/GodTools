#if WITCHCRAFT_KNIGHT
using GodTools.UI.Prefabs;

// ReSharper disable CheckNamespace

namespace GodTools.UI;

public partial class WindowTops
{
    private void CreateGrid_WITCHCRAFT_KNIGHT()
    {
        TitledGrid knight_keyword_grid = new_keyword_grid("knight");
        new_keyword(knight_keyword_grid, "knight", "Knight", (a, b) =>
        {
            float a_knight = a.data["Knight"];
            float b_knight = b.data["Knight"];
            var res = a_knight.CompareTo(b_knight);
            return res;
        }, a =>
        {
            float a_knight = a.data["Knight"];
            return $"{a_knight:F1} 生命之力";
        });
    }
}
#endif