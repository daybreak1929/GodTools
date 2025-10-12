namespace GodTools.Features;

public class ClanSavedList : MetaObjectSavedList<ClanSavedList, Clan, ClanData>
{
    protected override DropsAction GetDropAction()
    {
        return (tile, id) =>
        {
            var clan = GetObject(SelectedData);
            foreach (Actor actor in Finder.getUnitsFromChunk(tile, 1, 3f, false))
            {
                actor.setClan(clan);
            }
        };
    }

    protected override MetaSystemManager<Clan, ClanData> GetSystemManager()
    {
        return World.world.clans;
    }
}