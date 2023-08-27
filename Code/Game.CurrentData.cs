using System.Collections.Generic;
using System.Linq;

namespace GodTools.Game;

public class CurrentData
{
    private Dictionary<string, List<string>> units_cur_allowed_actions = new();
    private Dictionary<string, List<string>> buildings_cur_allowed_actions = new();
    private Dictionary<string, List<string>> _union_cur_allowed_actions = new();
    public void reset(NormalData normal_data)
    {
        _union_cur_allowed_actions.Clear();
        foreach (string actor_asset_id in normal_data.units_all_allowed_actions.Keys)
        {
            units_cur_allowed_actions[actor_asset_id] = new();
            _union_cur_allowed_actions[actor_asset_id] = units_cur_allowed_actions[actor_asset_id];
        }

        foreach (string building_asset_id in normal_data.buildings_all_allowed_actions.Keys)
        {
            buildings_cur_allowed_actions[building_asset_id] = new();
            _union_cur_allowed_actions[building_asset_id] = buildings_cur_allowed_actions[building_asset_id];
        }

        _union_cur_allowed_actions[C.first_in] = normal_data.first_in_allowed_actions;
    }

    public List<string> get_page(string id)
    {
        return _union_cur_allowed_actions.TryGetValue(id, out List<string> action) ? action : new List<string>();
    }
}