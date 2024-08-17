using NeoModLoader.General.UI.Prefabs;
using NeoModLoader.General.UI.Window.Layout;
using UnityEngine;

namespace GodTools.UI;

public partial class WindowCreatureDataEditor
{
    private TextInput _level_input;

    private void InitializeVanilla()
    {
        AutoVertLayoutGroup tab = CreateTab("Vanilla", SpriteTextureLoader.getSprite("ui/icons/iconAbout"),
                                            OnVanillaEnable);

        _level_input = Instantiate(TextInput.Prefab, tab.transform);
        _level_input.Setup("0", LevelInput, SpriteTextureLoader.getSprite("ui/icons/iconLevel"),
                           SpriteTextureLoader.getSprite("ui/special/windowInnerSliced"));
    }

    private void OnVanillaEnable(string tab)
    {
    }

    private void LevelInput(string value)
    {
        if (!int.TryParse(value, out var v))
        {
            _level_input.text.color = Color.red;
            return;
        }

        if (v < 0)
        {
            _level_input.text.color = Color.red;
            return;
        }

        _level_input.text.color = Color.white;

        _selected_actor.data.level = v;
        _selected_actor.setStatsDirty();
    }
}