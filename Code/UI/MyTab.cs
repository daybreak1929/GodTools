using System.Collections.Generic;
using NCMS.Utils;
using NeoModLoader.General.UI.Tab;
using UnityEngine;
using UnityEngine.Events;

namespace GodTools.UI;

internal enum Tab_Button_Type
{
    INFO,
    TOOL,
    OTHERS
}

internal static class MyTab
{
    public static PowersTab powers_tab;

    internal static void create_tab()
    {
        powers_tab = TabManager.CreateTab("GodTools", "GodTools", "GodTools Description",
            SpriteTextureLoader.getSprite("gt_windows/iconTab"));
        powers_tab.SetLayout(new List<string>
        {
            Tab_Button_Type.INFO.ToString(),
            Tab_Button_Type.TOOL.ToString(),
            Tab_Button_Type.OTHERS.ToString()
        });
    }

    internal static void add_buttons()
    {
        PowerButton button;
        add_button(
            create_button(
                C.about_this, "ui/Icons/iconabout",
                () => ScrollWindow.showWindow("gt_window_about_this")
            ),
            Tab_Button_Type.INFO
        );
        add_button(
            create_button(
                C.world_law, "ui/Icons/iconworldlaws",
                null
            ),
            Tab_Button_Type.TOOL
        );
        add_button(
            create_button(
                C.force_attack, $"ui/Icons/{C.icon_GP_force_attack}",
                null, ButtonType.GodPower
            ),
            Tab_Button_Type.TOOL
        );
        /*
        add_button(
            create_button(
                C.new_game, $"ui/Icons/{C.icon_GP_force_attack}",
                Main.game.start
            )
        );
        */
    }

    public static PowerButton create_button(string id, string sprite_path, UnityAction action,
        ButtonType button_type = ButtonType.Click)
    {
        return PowerButtons.CreateButton(
            id,
            Resources.Load<Sprite>(sprite_path),
            LocalizedTextManager.stringExists(id + C.title_postfix)
                ? LocalizedTextManager.getText(id + C.title_postfix)
                : id,
            LocalizedTextManager.stringExists(id + C.desc_postfix)
                ? LocalizedTextManager.getText(id + C.desc_postfix)
                : "",
            Vector2.zero,
            button_type,
            null,
            action);
    }

    public static PowerButton add_button(PowerButton button, Tab_Button_Type button_type = Tab_Button_Type.OTHERS)
    {
        powers_tab.AddPowerButton(button_type.ToString(), button);
        return button;
    }

    internal static void apply_buttons()
    {
        powers_tab.UpdateLayout();
    }
}