#if 启源_启源核心
using System.Collections.Generic;
using System.Linq;
using Cultivation_Way.Core;
using Cultivation_Way.Extension;
using Cultivation_Way.Library;
using GodTools.UI.Prefabs;
using NeoModLoader.General.UI.Window.Layout;
using UnityEngine;

namespace GodTools.UI;

public partial class WindowCreatureDataEditor
{
    private ElementEditor _element_editor;
    private SpellEditor   _spell_editor;

    private void InitializeCwCore()
    {
        AutoVertLayoutGroup tab = CreateTab("CwCore", SpriteTextureLoader.getSprite("ui/cw_icons/iconAwardUnit"),
                                            OnCwCoreEnable);

        _element_editor = Instantiate(ElementEditor.Prefab, tab.transform);
        _element_editor.name = "ElementEditor";
        _element_editor.transform.localScale = Vector3.one;
        _element_editor.Setup(ApplyElement);

        _spell_editor = Instantiate(SpellEditor.Prefab, tab.transform);
        _spell_editor.name = nameof(SpellEditor);
        _spell_editor.transform.localScale = Vector3.one;
        _spell_editor.Setup(ApplySpells);
    }

    private void OnCwCoreEnable(string tab)
    {
    }

    private void ApplyElement(CW_Element pElement)
    {
        CW_Actor actor = _selected_actor.CW();
        if (actor == null) return;
        actor.data.SetElement(pElement);
        actor.setStatsDirty();
    }

    private void ApplySpells(List<CW_SpellAsset> pSpells)
    {
        CW_Actor actor = _selected_actor.CW();
        if (actor == null) return;
        actor.data.SetSpells(new HashSet<string>(pSpells.Select(x => x.id)));
        actor.setStatsDirty();
    }
}

#endif