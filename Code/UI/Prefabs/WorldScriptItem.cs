using System;
using GodTools.Features.WorldScript;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GodTools.UI.Prefabs;

public class WorldScriptItem : APrefab<WorldScriptItem>
{
    public TextButton switchButton;
    public TextButton deleteButton;
    public RawText name;
    public ScriptInstance script;
    private void Start()
    {
        switchButton.Button.onClick.AddListener(toggle);
        deleteButton.Button.onClick.AddListener(deleteSelf);
    }

    private void toggle()
    {
        script.Enabled = !script.Enabled;
        switchButton.Localization.setKeyAndUpdate($"{C.mod_prefix}.ui.world_script.{(script.Enabled ? "enabled" : "disabled")}");
    }

    private void deleteSelf()
    {
        
    }

    public void Setup(ScriptInstance script)
    {
        this.script = script;
        name.Value = script.Name;
    }
    private static void _init()
    {
        var obj = new GameObject(nameof(WorldScriptItem), typeof(HorizontalLayoutGroup));
        obj.transform.SetParent(Main.prefabs, false);

        var switch_button = TextButton.Instantiate(obj.transform, pName: nameof(switchButton));
        var name_obj = RawText.Instantiate(obj.transform, pName: nameof(name));
        var delete_button = TextButton.Instantiate(obj.transform, pName: nameof(deleteButton));
        
        name_obj.Setup("新脚本", alignment: TextAnchor.UpperCenter);
        switch_button.Localization.key = $"{C.mod_prefix}.ui.world_script.enabled";
        delete_button.Localization.key = $"{C.mod_prefix}.ui.world_script.delete";
        switch_button.Localization.autoField = true;
        delete_button.Localization.autoField = true;


        Prefab = obj.AddComponent<WorldScriptItem>();
        Prefab.switchButton = switch_button;
        Prefab.deleteButton = delete_button;
        Prefab.name = name_obj;
    }
}