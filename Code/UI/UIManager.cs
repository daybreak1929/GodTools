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
        WindowCreatureDataEditor.CreateAndInit(nameof(WindowCreatureDataEditor), pSize: new(300, 250));
        WindowCreatureSearch.CreateWindow(nameof(WindowCreatureSearch), nameof(WindowCreatureSearch));
        WindowCreatureTitleEditor.CreateWindow(nameof(WindowCreatureTitleEditor), nameof(WindowCreatureTitleEditor));
        WindowCreatureSavedList.CreateAndInit(nameof(WindowCreatureSavedList));
        WindowTops.CreateAndInit($"{C.mod_prefix}.{nameof(WindowTops)}");
        WindowSubWorldCreator.CreateWindow($"{C.mod_prefix}.{nameof(WindowSubWorldCreator)}",
            $"{C.mod_prefix}.{nameof(WindowSubWorldCreator)}");
        WindowDetailedHistory.CreateAndInit(nameof(WindowDetailedHistory));
        WindowWorldScript.CreateAndInit(nameof(WindowWorldScript));
        CanvasCreatureTitle.Init();
    }
}