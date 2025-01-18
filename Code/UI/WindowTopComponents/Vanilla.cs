
using System;
using GodTools.UI.Prefabs;
using NeoModLoader.api.attributes;
using NeoModLoader.General;
using NeoModLoader.General.Game.extensions;

// ReSharper disable CheckNamespace

namespace GodTools.UI;

public partial class WindowTops
{
    private void CreateGrid_VANILLA()
    {
        TitledGrid race_filter_grid = new_filter_grid("race");
        AssetManager.raceLibrary.ForEach<Race, RaceLibrary>(race_asset =>
        {
            if (race_asset.nature) return;
            var local_race_id = race_asset.id;
            try
            {
                LM.AddToCurrentLocale($"{C.mod_prefix}.ui.filter.race.{race_asset.nameLocale}",
                    LM.Get(race_asset.nameLocale));
                new_filter(race_filter_grid, race_asset.nameLocale, race_asset.path_icon, a => a.isRace(local_race_id));
            }
            catch (Exception)
            {
                // ignored
            }
        });

        TitledGrid vanilla_keyword_grid = new_keyword_grid("vanilla");
        new_keyword(vanilla_keyword_grid, "level", "ui/icons/iconLevels", (a, b) =>
        {
            var res = a.data.level.CompareTo(b.data.level);

            return res;
        }, a => $"{a.data.level} 级");
        new_keyword(vanilla_keyword_grid, "kills", "ui/icons/iconSkulls",
            (a, b) => a.data.kills.CompareTo(b.data.kills), a=> $"{a.data.kills} 击杀");
        new_keyword(vanilla_keyword_grid, "birth", "ui/icons/iconAge",
            (a, b) => a.data.getAge().CompareTo(b.data.getAge()), a => $"{a.data.getAge()} 岁");
        new_keyword(vanilla_keyword_grid, "max_health", "ui/icons/iconHealth",
            (a, b) => a.stats[S.health].CompareTo(b.stats[S.health]), a => $"{a.stats[S.health]} 最大生命");
        new_keyword(vanilla_keyword_grid, "damage", "ui/icons/iconDamage",
            (a, b) => a.stats[S.damage].CompareTo(b.stats[S.damage]), a => $"{a.stats[S.damage]} 攻击");

    }
}