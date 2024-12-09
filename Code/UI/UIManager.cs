using GodTools.Abstract;
using GodTools.Libraries;

namespace GodTools.UI;

[Dependency(typeof(GodPowers))]
internal class UIManager : IManager
{
    public void Initialize()
    {
        WindowModInfo.CreateWindow($"{C.mod_prefix}.{nameof(WindowModInfo)}", nameof(WindowModInfo));
        WindowCreatureSpriteEditor.CreateWindow(nameof(WindowCreatureSpriteEditor), nameof(WindowCreatureSpriteEditor));
        WindowCreatureDataEditor.CreateWindow(nameof(WindowCreatureDataEditor), nameof(WindowCreatureDataEditor));
        WindowCreatureSearch.CreateWindow(nameof(WindowCreatureSearch), nameof(WindowCreatureSearch));
        WindowCreatureSavedList.CreateAndInit(nameof(WindowCreatureSavedList));
        WindowTops.CreateAndInit($"{C.mod_prefix}.{nameof(WindowTops)}");
    }
}