using GodTools.UI.Prefabs;
using NeoModLoader.General;
using NeoModLoader.General.UI.Window;
using NeoModLoader.General.UI.Window.Layout;
using NeoModLoader.General.UI.Window.Utils.Extensions;
using UnityEngine;

namespace GodTools.UI;

public class WindowCreatureSearch : SingleAutoLayoutWindow<WindowCreatureSearch>
{
    private ObjectPoolGenericMono<PrefabUnitElement> _pool;
    private AutoVertLayoutGroup                      display_group;

    protected override void Init()
    {
        AutoVertLayoutGroup search_group = this.BeginVertGroup();
        InputBar search_bar = Instantiate(InputBar.Prefab, search_group.transform);
        search_bar.SetSize(new Vector2(180, 24));
        search_bar.Setup("", ApplySearch);

        display_group = this.BeginVertGroup(pSpacing: 8);
        display_group.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
        _pool = new ObjectPoolGenericMono<PrefabUnitElement>(
            ResourcesFinder.FindResource<PrefabUnitElement>("list_element_favorites"), display_group.transform);
    }

    private void ApplySearch(string name)
    {
        _pool.clear();
        if (string.IsNullOrEmpty(name)) return;
        var list = World.world.units.getSimpleList();
        foreach (Actor unit in list)
            if (unit.getName().Contains(name))
            {
                PrefabUnitElement element = _pool.getNext();
                element.show(unit);
            }
    }
}