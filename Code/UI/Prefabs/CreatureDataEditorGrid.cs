using System;
using System.Collections.Generic;
using GodTools.Utils;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.UI.Prefabs;

public class CreatureDataEditorGrid : APrefab<CreatureDataEditorGrid>
{
    public virtual void EnabledWith(Actor actor)
    {
        throw new NotImplementedException();
    }

    public virtual void Setup()
    {
        throw new NotImplementedException();
    }

    public string TitleKey
    {
        get => titleLocalizedText.key;
        set => titleLocalizedText.setKeyAndUpdate(value);
    }
    private static Dictionary<Type, GameObject> _prefabs = new();

    [SerializeField]
    private LocalizedText titleLocalizedText;
    internal static T GetPrefab<T>() where T : CreatureDataEditorGrid
    {
        if (_prefabs.ContainsKey(typeof(T)))
            return _prefabs[typeof(T)].GetComponent<T>();
        
        var obj = new GameObject(typeof(T).Name, typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        obj.transform.SetParent(Main.prefabs);

        obj.GetComponent<VerticalLayoutGroup>().SetSimpleVertLayout();
        obj.GetComponent<VerticalLayoutGroup>().childControlWidth = true;
        obj.GetComponent<VerticalLayoutGroup>().childForceExpandWidth = true;
        obj.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.MinSize;

        var title = new GameObject("Title", typeof(Text), typeof(LocalizedText), typeof(ContentSizeFitter));
        title.transform.SetParent(obj.transform);
        title.GetComponent<LocalizedText>().autoField = true;
        title.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        var text = title.GetComponent<Text>();
        GeneralTools.text_basic_setting(text);
        text.resizeTextForBestFit = false;
        text.fontSize = 6;
        

        var prefab = obj.AddComponent<T>();
        prefab.titleLocalizedText = title.GetComponent<LocalizedText>();
        _prefabs.Add(typeof(T), obj);
        return prefab;
    }
}