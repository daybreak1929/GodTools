using GodTools.Utils;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.UI.Prefabs;

public class TitledGrid : APrefab<TitledGrid>
{
    public Text            Title { get; private set; }
    public GridLayoutGroup Grid  { get; private set; }

    protected override void Init()
    {
        if (Initialized) return;
        base.Init();

        Title = transform.Find(nameof(Title)).GetComponent<Text>();
        Grid = transform.Find(nameof(Grid)).GetComponent<GridLayoutGroup>();
    }

    public override void SetSize(Vector2 pSize)
    {
        Init();
        base.SetSize(pSize);
        Title.rectTransform.sizeDelta = new Vector2(pSize.x,                10);
        Grid.GetComponent<RectTransform>().sizeDelta = new Vector2(pSize.x, 0);
    }

    public void Setup(string title_key, float width, Vector2 cell_size, Vector2 spacing)
    {
        Init();
        SetSize(new Vector2(width, 0));
        Title.GetComponent<LocalizedText>().setKeyAndUpdate(title_key);

        Grid.spacing = spacing;
        Grid.cellSize = cell_size;

        var padding = new RectOffset();

        var column_count = (int)((width - spacing.x) / (cell_size.x + spacing.x));
        Grid.constraintCount = column_count;

        padding.left = (int)((width - (column_count * cell_size.x + (column_count - 1) * spacing.x)) / 2);
        padding.right = padding.left;
        padding.top = (int)(spacing.y / 2);
        padding.bottom = padding.top;
        Grid.padding = padding;
    }

    private static void _init()
    {
        var obj = new GameObject(nameof(TitledGrid), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        obj.transform.SetParent(Main.prefabs);

        var vert_layout = obj.GetComponent<VerticalLayoutGroup>();
        vert_layout.childAlignment = TextAnchor.UpperCenter;
        vert_layout.childControlHeight = false;
        vert_layout.childControlWidth = false;
        vert_layout.childForceExpandHeight = false;
        vert_layout.childForceExpandWidth = false;

        obj.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.MinSize;

        var title = new GameObject(nameof(Title), typeof(Text), typeof(LocalizedText));
        title.transform.SetParent(obj.transform);
        title.GetComponent<LocalizedText>().autoField = true;
        var text = title.GetComponent<Text>();
        Helper.text_basic_setting(text);

        var grid = new GameObject(nameof(Grid), typeof(Image), typeof(GridLayoutGroup), typeof(ContentSizeFitter));
        grid.transform.SetParent(obj.transform);
        grid.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1);
        var bg = grid.GetComponent<Image>();
        bg.sprite = SpriteTextureLoader.getSprite("ui/special/windowInnerSliced");
        bg.type = Image.Type.Sliced;
        var grid_layout = grid.GetComponent<GridLayoutGroup>();
        grid_layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        var fitter = grid.GetComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.MinSize;

        Prefab = obj.AddComponent<TitledGrid>();
    }
}