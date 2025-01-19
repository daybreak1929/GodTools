
using System;
using System.Collections.Generic;
using System.Linq;
using GodTools.Abstract;
using GodTools.UI.Prefabs;
using GodTools.UI.WindowTopComponents;
using NeoModLoader.api.attributes;
using NeoModLoader.General;
using NeoModLoader.General.Game.extensions;
using UnityEngine;
using UnityEngine.UI;

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
        asset_filter_grid = new_filter_grid("asset");
        profession_filter_grid = new_filter_grid("profession");
        kingdom_filter_grid = new_filter_grid("kingdom");
        filter_button_in_grid_pool_dict[kingdom_filter_grid] = new MonoObjPool<FilterButtonInGrid>(
            FilterButtonInGrid.Prefab, kingdom_filter_grid.Grid.transform,
            obj =>
            {
                var icon_part = new GameObject("Icon", typeof(Image));
                icon_part.transform.SetParent(obj.transform);
                icon_part.transform.localPosition = Vector3.zero;
                icon_part.transform.localScale = Vector3.one;
                icon_part.GetComponent<Image>().raycastTarget = false;
            });
        
        new_filter(profession_filter_grid, "baby", "ui/icons/iconDamage", x=>x.isProfession(UnitProfession.Baby));
        new_filter(profession_filter_grid, "unit", "ui/icons/iconDamage", x=>x.isProfession(UnitProfession.Unit));
        new_filter(profession_filter_grid, "warrior", "ui/icons/iconDamage", x=>x.isProfession(UnitProfession.Warrior));
        new_filter(profession_filter_grid, "king", "ui/icons/iconDamage", x=>x.isProfession(UnitProfession.King));
        new_filter(profession_filter_grid, "leader", "ui/icons/iconDamage", x=>x.isProfession(UnitProfession.Leader));

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
    private TitledGrid asset_filter_grid;
    private TitledGrid profession_filter_grid;
    private TitledGrid kingdom_filter_grid;
    [Hotfixable]
    private void CheckDynamicGrid_VANILLA()
    {
        if (filter_button_in_grid_pool_dict.TryGetValue(asset_filter_grid, out var pool))
        {
            pool.Clear();
        }
        if (filter_button_in_grid_pool_dict.TryGetValue(kingdom_filter_grid, out pool))
        {
            pool.Clear();
        }
        
        var asset_set = new HashSet<string>();
        var kingdom_set = new HashSet<string>();
        foreach (var actor in World.world.units.getSimpleList())
        {
            if (actor != null && actor.data != null && actor.isAlive() && !actor.object_destroyed && actor.asset.canBeInspected)
            {
                asset_set.Add(actor.asset.id);
                if (actor.kingdom != null)
                    kingdom_set.Add(actor.kingdom.data.id);
            }
        }
        
        foreach (var asset_id in asset_set)
        {
            var asset = AssetManager.actor_library.get(asset_id);
            if (asset.id.StartsWith("_")) continue;
            var icon_path = string.IsNullOrEmpty(asset.icon) || SpriteTextureLoader.getSprite($"ui/icons/{asset.icon}") == null ?
                "ui/icons/iconQuestionMark" : $"ui/icons/{asset.icon}";
            LM.AddToCurrentLocale($"{C.mod_prefix}.ui.filter.asset.{asset.nameLocale}",
                LM.Get(asset.nameLocale));
            new_filter(asset_filter_grid, asset.nameLocale, icon_path, a => a.asset.id == asset_id);
        }

        foreach (var kingdom_id in kingdom_set)
        {
            if (World.world.kingdoms.dict_hidden.ContainsKey(kingdom_id)) continue;
            var kingdom = World.world.kingdoms.get(kingdom_id);
            LM.AddToCurrentLocale($"{C.mod_prefix}.ui.filter.kingdom.{kingdom_id}",
                kingdom.data.name);
            var filter_button_in_grid = new_filter(kingdom_filter_grid, kingdom_id, "ui/icons/iconDamage", a => a.kingdom.data.id == kingdom_id);

            var banner_container = BannerGenerator.dict[kingdom.race.banner_id];

            filter_button_in_grid.icon.sprite = banner_container.backrounds[kingdom.data.banner_background_id];
            filter_button_in_grid.icon.color = kingdom.kingdomColor.getColorMain2();
            var sub_icon = filter_button_in_grid.transform.Find("Icon").GetComponent<Image>();
            sub_icon.sprite = banner_container.icons[kingdom.data.banner_icon_id];
            sub_icon.color = kingdom.kingdomColor.getColorBanner();
            sub_icon.rectTransform.sizeDelta = kingdom_filter_grid.Grid.cellSize;
        }

    }
}