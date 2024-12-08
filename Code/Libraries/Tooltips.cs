using NeoModLoader.General;

namespace GodTools.Libraries;

internal static class Tooltips
{
    public static TooltipAsset mood;

    public static void init()
    {
        mood = AssetManager.tooltips.clone(nameof(mood), "tip");
        mood.callback = (tooltip, type, data) =>
        {
            Actor actor = data.actor;
            MoodAsset mood_asset = AssetManager.moods.get(actor.data.mood);
            tooltip.name.text = LM.Get($"mood_{mood_asset.id}");
            tooltip.showBaseStats(mood_asset.base_stats);
        };
    }
}