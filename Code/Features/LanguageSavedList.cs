namespace GodTools.Features;

public class LanguageSavedList : MetaObjectSavedList<LanguageSavedList, Language, LanguageData>
{
    protected override DropsAction GetDropAction()
    {
        
        return (tile, id) =>
        {
            var lang = GetObject(SelectedData);
            foreach (Actor actor in Finder.getUnitsFromChunk(tile, 1, 3f, false))
            {
                actor.setLanguage(lang);
            }
        };
    }

    protected override MetaSystemManager<Language, LanguageData> GetSystemManager()
    {
        return World.world.languages;
    }
}