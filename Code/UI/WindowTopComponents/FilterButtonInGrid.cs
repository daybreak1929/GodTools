using System;
using System.Collections.Generic;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GodTools.UI.WindowTopComponents;

public class FilterButtonInGrid : APrefab<FilterButtonInGrid>
{
    [SerializeField]
    public Image icon;
    [SerializeField]
    private Button button;
    [SerializeField]
    private TipButton tipButton;
    
    private string filter_id;
    private List<FilterSetting> target_list;
    private Func<Actor, bool> filter_action;

    public void Setup(string filter_id, string icon_path, Func<Actor, bool> filter_action, List<FilterSetting> target_list)
    {
        icon.sprite = SpriteTextureLoader.getSprite(icon_path);
        tipButton.textOnClick = filter_id;
        this.filter_id = filter_id;
        this.target_list = target_list;
        this.filter_action = filter_action;
        
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(Toggle);
    }

    private void Toggle()
    {
        
        var idx = target_list.FindIndex(x=>x.ID == filter_id);
        if (idx != -1)
        {
            target_list.RemoveAt(idx);
        }
        else
        {
            target_list.Add(new FilterSetting(filter_id, icon.sprite, filter_action));
        }
    }
    private static void _init()
    {
        var obj = new GameObject(nameof(FilterButtonInGrid), typeof(Image), typeof(Button), typeof(TipButton));
        obj.transform.SetParent(Main.prefabs);
       
        Prefab = obj.AddComponent<FilterButtonInGrid>();
        Prefab.icon = obj.GetComponent<Image>();
        Prefab.button = obj.GetComponent<Button>();
        Prefab.tipButton = obj.GetComponent<TipButton>();
    }
}