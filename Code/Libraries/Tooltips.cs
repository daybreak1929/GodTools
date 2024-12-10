using GodTools.Abstract;
using NeoModLoader.General;

namespace GodTools.Libraries;

public class Tooltips : ExtendLibrary<TooltipAsset, Tooltips>
{
    [CloneSource("tip")] public static TooltipAsset mood { get; private set; }

    protected override void OnInit()
    {
        RegisterAssets();
        mood.callback = (tooltip, type, data) =>
        {
            Actor actor = data.actor;
            MoodAsset mood_asset = AssetManager.moods.get(actor.data.mood);
            tooltip.name.text = LM.Get($"mood_{mood_asset.id}");
            tooltip.showBaseStats(mood_asset.base_stats);
        };
    }
}