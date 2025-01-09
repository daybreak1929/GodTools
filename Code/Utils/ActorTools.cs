using System.Collections.Generic;

namespace GodTools.Utils;

public static class ActorTools
{
    private const string title_guid_key = $"{C.mod_prefix}.title_guid";
    public static string GetTitle(this Actor actor, string title_guid)
    {
        actor.data.get(title_guid, out string title, "头衔");
        return title;
    }
    public static List<string> GetTitleGuids(this Actor actor)
    {
        actor.data.get(title_guid_key, out string title_guids_str);
        if (string.IsNullOrEmpty(title_guids_str))
        {
            return new List<string>();
        }
        return new List<string>(title_guids_str.Split(','));
    }
    public static void AddTitleGuid(this Actor actor, string title_guid)
    {
        var title_guids = GetTitleGuids(actor);
        if (!title_guids.Contains(title_guid))
        {
            title_guids.Add(title_guid);
            actor.data.set(title_guid_key, string.Join(",", title_guids));
        }
    }
    public static void RemoveTitleGuid(this Actor actor, string title_guid)
    {
        var title_guids = GetTitleGuids(actor);
        if (title_guids.Contains(title_guid))
        {
            title_guids.Remove(title_guid);
            actor.data.set(title_guid_key, string.Join(",", title_guids));
        }
    }
}