namespace GodTools.Features;

public class SubspeciesSavedList : MetaObjectSavedList<SubspeciesSavedList, Subspecies, SubspeciesData>
{
    protected override DropsAction GetDropAction()
    {
        return (tile, id) =>
        {
            var subspecies = GetObject(SelectedData);
            foreach (Actor actor in Finder.getUnitsFromChunk(tile, 1, 3f, false))
            {
                if (actor.subspecies?.getActorAsset() == subspecies.getActorAsset())
                {
                    actor.setSubspecies(subspecies);
                }
            }
        };
    }

    protected override MetaSystemManager<Subspecies, SubspeciesData> GetSystemManager()
    {
        return World.world.subspecies;
    }
}