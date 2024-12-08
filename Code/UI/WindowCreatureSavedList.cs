using System;
using System.Collections.Generic;
using System.IO;
using GodTools.Libraries;
using GodTools.UI.Prefabs;
using NeoModLoader.api;
using NeoModLoader.api.attributes;
using NeoModLoader.General;
using Newtonsoft.Json;
using UnityEngine;

namespace GodTools.UI;

public class WindowCreatureSavedList : AbstractWindow<WindowCreatureSavedList>
{
    private const float single_element_height = 56;
    private const float start_y               = -single_element_height / 2;

    private static readonly string _save_path = Path.Combine(Application.persistentDataPath, "saved_actors.json");

    private static ActorData _selected_data;

    private List<ActorData>                           _list;
    private ObjectPoolGenericMono<SavedActorDataCard> _pool;
    private PowerButton                               _power_button;

    private RectTransform                 content_rect;
    private Dictionary<string, ActorData> saved_actors;
    private RectTransform                 scroll_view_rect;

    private void Update()
    {
        OnUpdate();
    }

    private string ParseIDforActorData(ActorData data)
    {
        return JsonConvert.SerializeObject(data).GetHashCode().ToString();
    }

    public bool ActorSaved(Actor actor)
    {
        actor.prepareForSave();
        return saved_actors.ContainsKey(ParseIDforActorData(actor.data));
    }

    public void SaveActor(Actor actor)
    {
        actor.prepareForSave();
        saved_actors[ParseIDforActorData(actor.data)] = Copy(actor.data);

        File.WriteAllText(_save_path, JsonConvert.SerializeObject(saved_actors));
    }

    public void UnsaveActor(Actor actor)
    {
        actor.prepareForSave();
        saved_actors.Remove(ParseIDforActorData(actor.data));
        File.WriteAllText(_save_path, JsonConvert.SerializeObject(saved_actors));
    }

    private void UnsaveActorData(ActorData data)
    {
        saved_actors.Remove(ParseIDforActorData(data));
        File.WriteAllText(_save_path, JsonConvert.SerializeObject(saved_actors));
    }

    private static ActorData Copy(ActorData data)
    {
        var copied_data = JsonConvert.DeserializeObject<ActorData>(JsonConvert.SerializeObject(data));
        if (copied_data.custom_data_bool?.dict == null) copied_data.custom_data_bool = null;

        if (copied_data.custom_data_float?.dict == null) copied_data.custom_data_float = null;

        if (copied_data.custom_data_int?.dict == null) copied_data.custom_data_int = null;

        if (copied_data.custom_data_string?.dict == null) copied_data.custom_data_string = null;

        return copied_data;
    }

    protected override void Init()
    {
        content_rect = ContentTransform.GetComponent<RectTransform>();
        scroll_view_rect = content_rect.parent.parent.GetComponent<RectTransform>();
        content_rect.pivot = new Vector2(0.5f, 1);
        _pool = new ObjectPoolGenericMono<SavedActorDataCard>(SavedActorDataCard.Prefab, ContentTransform);
        _power_button = PowerButtonCreator.CreateGodPowerButton(MyPowers.place_saved_actor.id,
            SpriteTextureLoader.getSprite("gt_windows/save_actor_list"), Main.prefab_library);

        try
        {
            saved_actors = JsonConvert.DeserializeObject<Dictionary<string, ActorData>>(File.ReadAllText(_save_path));
        }
        catch (Exception)
        {
            saved_actors = new Dictionary<string, ActorData>();
        }
    }

    private void OnUpdate()
    {
        content_rect.sizeDelta = new Vector2(0, _list.Count * single_element_height);
        Vector3 curr_position = ContentTransform.localPosition;

        var view_y_start = curr_position.y;
        var view_y_end = curr_position.y + scroll_view_rect.sizeDelta.y;

        var view_start_idx = (int)(view_y_start / single_element_height);
        var view_end_idx = Math.Min((int)(view_y_end / single_element_height) + 1, _list.Count - 1);

        _pool.clear();
        for (var i = view_start_idx; i <= view_end_idx; i++)
        {
            var idx = i;
            ActorData data = _list[idx];
            SavedActorDataCard card = _pool.getNext();
            card.Setup(data, () =>
            {
                ScrollWindow.hideAllEvent();
                _selected_data = data;
                _power_button.clickActivePower();
            }, () =>
            {
                UnsaveActorData(data);
                _list.RemoveAt(idx);
                Main.LogInfo("delete");
            });
            card.transform.localPosition = new Vector2(0, start_y - i * single_element_height);
        }
    }

    public override void OnNormalEnable()
    {
        _list = new List<ActorData>(saved_actors.Values);
    }

    [Hotfixable]
    public static bool SpawnSelectedSavedActor(WorldTile tile, string power_id)
    {
        ActorData data = Copy(_selected_data);
        data.id = World.world.mapStats.getNextId("unit");
        if (World.world.cities.get(data.cityID) == null) data.cityID = "";

        if (World.world.clans.get(data.clan) == null) data.clan = "";

        if (World.world.cultures.get(data.culture) == null) data.culture = "";

        data.homeBuildingID = "";
        data.transportID = "";
        data.x = tile.pos.x;
        data.y = tile.pos.y;
        World.world.units.loadObject(data);
        return true;
    }
}