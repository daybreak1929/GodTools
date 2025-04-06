using GodTools.Abstract;
using NeoModLoader.General;

namespace GodTools.Libraries;

public class Tooltips : ExtendLibrary<TooltipAsset, Tooltips>
{

    protected override void OnInit()
    {
        RegisterAssets();
    }
}