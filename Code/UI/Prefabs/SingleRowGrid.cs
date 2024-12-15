using GodTools.Utils;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.UI.Prefabs;

public class SingleRowGrid : APrefab<SingleRowGrid>
{
    public Text          Title { get; private set; }
    public RectTransform Grid  { get; private set; }

    protected override void Init()
    {
        if (Initialized) return;
        base.Init();
        Title = transform.Find(nameof(Title)).GetComponent<Text>();
        Grid = transform.Find(nameof(Grid)).GetComponent<RectTransform>();
    }

    public override void SetSize(Vector2 pSize)
    {
        Init();
        base.SetSize(pSize);
        Title.rectTransform.sizeDelta = new Vector2(pSize.x, pSize.y);
        Grid.sizeDelta = new Vector2(pSize.x,                pSize.y);
    }

    public void Setup(string title_key, Vector2 size, Vector2 cellsize)
    {
        Init();
        SetSize(size);
        Title.GetComponent<LocalizedText>().setKeyAndUpdate(title_key);
        Grid.GetComponent<GridLayoutGroup>().cellSize = cellsize;
    }

    private static void _init()
    {
        GameObject obj = new(nameof(SingleRowGrid), typeof(Image));
        obj.transform.SetParent(Main.prefabs);

        var bg = obj.GetComponent<Image>();
        bg.sprite = SpriteTextureLoader.getSprite("ui/special/windowInnerSliced");
        bg.type = Image.Type.Sliced;

        var title = new GameObject(nameof(Title), typeof(Text), typeof(LocalizedText), typeof(Shadow));
        title.transform.SetParent(obj.transform);
        var text = title.GetComponent<Text>();
        GeneralTools.text_basic_setting(text);
        text.color = new Color(1, 0.607f, 0.11f, 0.18f);
        var localized_text = title.GetComponent<LocalizedText>();
        localized_text.autoField = true;

        var grid = new GameObject(nameof(Grid), typeof(GridLayoutGroup), typeof(FlexibleOneRowGrid));
        grid.transform.SetParent(obj.transform);
        grid.GetComponent<GridLayoutGroup>().childAlignment = TextAnchor.MiddleLeft;

        Prefab = obj.AddComponent<SingleRowGrid>();
    }
}