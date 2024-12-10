using System.Collections.Generic;
using NeoModLoader.api.attributes;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.UI.Prefabs;

public delegate void OnSpriteDisplayClick(SpriteDisplay sprite_display, int x, int y);

public delegate void OnPixelDisplayClick(SinglePixelDisplay pixel_display);

public class SinglePixelDisplay : APrefab<SinglePixelDisplay>
{
    private Button _button;
    private Image  _image;
    public  int    X     { get; private set; }
    public  int    Y     { get; private set; }
    public  Color  color { get; private set; }

    protected override void Init()
    {
        if (Initialized) return;
        base.Init();

        _image = GetComponent<Image>();
        _button = GetComponent<Button>();
    }

    public void Setup(int x, int y, Color color, OnPixelDisplayClick on_click)
    {
        Init();
        X = x;
        Y = y;
        UpdateColor(color);
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => on_click(this));
    }

    public void UpdateColor(Color color)
    {
        _image.color = color;
        this.color = color;
    }

    private static void _init()
    {
        GameObject obj = new(nameof(SinglePixelDisplay), typeof(Image), typeof(Button));
        obj.transform.SetParent(Main.prefabs);

        Prefab = obj.AddComponent<SinglePixelDisplay>();
    }
}

public class SpriteDisplay : APrefab<SpriteDisplay>
{
    private Color[,]                                  _origin_colors;
    private ObjectPoolGenericMono<SinglePixelDisplay> _pixel_pool;
    private SinglePixelDisplay[,]                     _pixels;
    private RectTransform                             _sprite_container;
    private GridLayoutGroup                           _sprite_grid;

    protected override void Init()
    {
        if (Initialized) return;
        base.Init();

        _sprite_container = transform.Find("SpriteContainer")?.GetComponent<RectTransform>();
        _sprite_grid = _sprite_container?.GetComponent<GridLayoutGroup>();
        _pixel_pool = new ObjectPoolGenericMono<SinglePixelDisplay>(SinglePixelDisplay.Prefab, _sprite_container);
    }

    public override void SetSize(Vector2 pSize)
    {
        Init();
        base.SetSize(pSize);
        _sprite_container.sizeDelta = pSize;
    }

    public Color GetPixelColor(int x, int y)
    {
        SinglePixelDisplay pixel = _pixels[x, y];
        return pixel.color;
    }

    public Color GetPixelOriginColor(int x, int y)
    {
        return _origin_colors[x, y];
    }

    public void SetPixelColor(int x, int y, Color color)
    {
        SinglePixelDisplay pixel = _pixels[x, y];
        pixel.UpdateColor(color);
    }

    [Hotfixable]
    public void ReplaceColor(Color origin_color, Color target_color)
    {
        foreach (SinglePixelDisplay pixel in _pixels)
            if (_origin_colors[pixel.X, pixel.Y] == origin_color)
                pixel.UpdateColor(target_color);
    }

    public Dictionary<Color, Color> GetColorMap()
    {
        Dictionary<Color, Color> color_map = new();
        for (var x = 0; x < _pixels.GetLength(0); x++)
        for (var y = 0; y < _pixels.GetLength(1); y++)
        {
            Color origin_color = _origin_colors[x, y];
            Color current_color = GetPixelColor(x, y);
            if (origin_color != current_color) color_map[origin_color] = current_color;
        }

        return color_map;
    }

    public Sprite GetCurrentSprite(Sprite ref_sprite)
    {
        var texture = new Texture2D(_pixels.GetLength(0), _pixels.GetLength(1));
        texture.filterMode = ref_sprite.texture.filterMode;
        for (var x = 0; x < _pixels.GetLength(0); x++)
        for (var y = 0; y < _pixels.GetLength(1); y++)
            texture.SetPixel(x, y, GetPixelColor(x, y));

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
            new Vector2(ref_sprite.pivot.x / texture.width, ref_sprite.pivot.y / texture.height),
            ref_sprite.pixelsPerUnit);
    }

    [Hotfixable]
    public void Setup(Sprite origin_sprite, OnSpriteDisplayClick on_click)
    {
        Init();
        _pixel_pool.clear();

        Texture2D texture = origin_sprite.texture;
        Rect rect = origin_sprite.rect;

        Vector2 size = new(_sprite_container.sizeDelta.x, _sprite_container.sizeDelta.y);
        if (rect.width > rect.height)
            size.y = _sprite_container.sizeDelta.y * (rect.height / rect.width);
        else
            size.x = _sprite_container.sizeDelta.x * (rect.width / rect.height);

        _sprite_grid.cellSize = new Vector2(size.x / rect.width, size.y / rect.height);
        _sprite_grid.padding = new RectOffset((int)((_sprite_container.sizeDelta.x - size.x) / 2),
            (int)((_sprite_container.sizeDelta.x - size.x) / 2),
            (int)((_sprite_container.sizeDelta.y - size.y) / 2),
            (int)((_sprite_container.sizeDelta.y - size.y) / 2));
        _pixels = new SinglePixelDisplay[(int)rect.width, (int)rect.height];
        _origin_colors = new Color[(int)rect.width, (int)rect.height];
        for (var x = 0; x < rect.width; x++)
        for (var y = 0; y < rect.height; y++)
        {
            Color color = texture.GetPixel((int)rect.x + x, (int)rect.y + y);
            SinglePixelDisplay pixel = _pixel_pool.getNext();
            pixel.Setup(x, y, color,
                pixel_display => { on_click?.Invoke(this, pixel_display.X, pixel_display.Y); });
            pixel.transform.SetAsLastSibling();
            _pixels[x, y] = pixel;
            _origin_colors[x, y] = color;
        }
    }

    private static void _init()
    {
        GameObject obj = new(nameof(SpriteDisplay), typeof(RectTransform));
        obj.transform.SetParent(Main.prefabs);

        var sprite_obj = new GameObject("SpriteContainer", typeof(RectTransform), typeof(GridLayoutGroup));
        sprite_obj.transform.SetParent(obj.transform);
        sprite_obj.transform.localPosition = Vector3.zero;
        sprite_obj.transform.localScale = Vector3.one;

        var grid = sprite_obj.GetComponent<GridLayoutGroup>();
        grid.startCorner = GridLayoutGroup.Corner.LowerLeft;
        grid.startAxis = GridLayoutGroup.Axis.Vertical;

        Prefab = obj.AddComponent<SpriteDisplay>();
    }
}