namespace GodTools.Features;

public class CultureSavedList : MetaObjectSavedList<CultureSavedList, Culture, CultureData>
{
    protected override DropsAction GetDropAction()
    {
        return (tile, id) =>
        {
            var culture = GetObject(SelectedData);
            foreach (Actor actor in Finder.getUnitsFromChunk(tile, 1, 3f, false))
            {
                actor.setCulture(culture);
            }
        };
    }

    protected override MetaSystemManager<Culture, CultureData> GetSystemManager()
    {
        return World.world.cultures;
    }
}