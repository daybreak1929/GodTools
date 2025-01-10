using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GodTools.UI.Prefabs;

public class TextButton : APrefab<TextButton>
{
    public Button Button => button;
    public Text Text => text;
    public LocalizedText Localization => localizedText;

    public void Setup(string localization_key, UnityAction action)
    {
        Localization.autoField = true;
        Localization.setKeyAndUpdate(localization_key);
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(action);
    }

    public void SetupLocalized(string localized_text, UnityAction action)
    {
        Text.text = localized_text;
        Localization.autoField = false;
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(action);
    }

    private static void _init()
    {
        GameObject obj = new GameObject(nameof(TextButton), typeof(Button), typeof(Text),
            typeof(LocalizedText),
            typeof(ContentSizeFitter));
        obj.transform.SetParent(Main.prefabs);


        var text = obj.GetComponent<Text>();
        text.alignment = TextAnchor.MiddleCenter;
        text.font = LocalizedTextManager.currentFont;
        text.fontSize = 8;

        var fitter = obj.GetComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;


        Prefab = obj.AddComponent<TextButton>();
        Prefab.button = Prefab.GetComponent<Button>();
        Prefab.text = Prefab.GetComponent<Text>();
        Prefab.localizedText = Prefab.GetComponent<LocalizedText>();
    }

    [SerializeField]
    private Button button;
    [SerializeField]
    private Text text;
    [SerializeField]
    private LocalizedText localizedText;
}