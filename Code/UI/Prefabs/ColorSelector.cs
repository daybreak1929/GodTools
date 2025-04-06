using System;
using System.Collections.Generic;
using GodTools.Utils;
using NeoModLoader.api.attributes;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GodTools.UI.Prefabs;

public class ColorSelector : APrefab<ColorSelector>
{
    private ObjectPoolGenericMono<SimpleButton> _fast_color_option_pool;
    private HueSelector _hue_selector;
    private SubColorSelector _sub_color_selector;
    public  RectTransform HueSelectorRect => _hue_selector.transform as RectTransform;
    public  RectTransform SubColorSelectorRect => _sub_color_selector.transform as RectTransform;
    public  RectTransform FastOptionRect => _fast_color_option_pool._parent_transform as RectTransform;

    protected override void Init()
    {
        if (Initialized) return;
        base.Init();
        _hue_selector = transform.Find(nameof(HueSelector)).GetComponent<HueSelector>();
        _sub_color_selector = transform.Find(nameof(SubColorSelector)).GetComponent<SubColorSelector>();
        _fast_color_option_pool =
            new ObjectPoolGenericMono<SimpleButton>(SimpleButton.Prefab, transform.Find("FastColorOptions"));
    }

    public void Setup(Color                         initial_color, Action<Color> on_color_selected,
                      KeyValuePair<Color, string>[] fast_color_options = null)
    {
        Init();
        Color.RGBToHSV(initial_color, out var h, out var s, out var v);
        _hue_selector.Setup(new Vector2(20,        100), h, hue => _sub_color_selector.UpdateStauration(hue));
        _sub_color_selector.Setup(new Vector2(100, 100), h, s, v, on_color_selected);

        _hue_selector.transform.localPosition = new Vector3(55, 0);
        _hue_selector.transform.localScale = Vector3.one;
        _sub_color_selector.transform.localPosition = new Vector3(-15, 0);
        _sub_color_selector.transform.localScale = Vector3.one;
        _fast_color_option_pool._parent_transform.localPosition = new Vector3(-15, -105);
        _fast_color_option_pool._parent_transform.localScale = Vector3.one;

        _fast_color_option_pool.clear();
        if (fast_color_options != null)
            for (var i = 0; i < fast_color_options.Length; i++)
            {
                Color color = fast_color_options[i].Key;
                SimpleButton button = _fast_color_option_pool.getNext();

                button.Setup(() => on_color_selected(color), null, null, new Vector2(10, 10), "tip", new TooltipData
                {
                    tip_name = fast_color_options[i].Value
                });
                button.Background.enabled = false;
                button.Icon.color = color;
            }
    }

    private static void _init()
    {
        GameObject obj = new(nameof(ColorSelector));
        obj.transform.SetParent(Main.prefabs);

        HueSelector hue_selector = Instantiate(HueSelector.Prefab, obj.transform);
        hue_selector.name = nameof(HueSelector);
        SubColorSelector sub_color_selector = Instantiate(SubColorSelector.Prefab, obj.transform);
        sub_color_selector.name = nameof(SubColorSelector);

        var fast_color_option_pool = new GameObject("FastColorOptions", typeof(RectTransform), typeof(GridLayoutGroup));
        fast_color_option_pool.transform.SetParent(obj.transform);
        fast_color_option_pool.GetComponent<GridLayoutGroup>().cellSize = new Vector2(10, 10);
        fast_color_option_pool.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.Flexible;

        Prefab = obj.AddComponent<ColorSelector>();
    }

    private class HueSelector : APrefab<HueSelector>, IPointerDownHandler, IDragHandler
    {
        private Image         _image;
        private Transform     _pos;
        private Texture2D     _texture;
        private Color[]       _texture_pixels;
        private RectTransform _transform;
        private Action<float> on_hue_changed;

        private float y;
        private int   width  => (int)_transform.sizeDelta.x;
        private int   height => (int)_transform.sizeDelta.y;

        [Hotfixable]
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 rel_pos =
                UITools.GetRelativePosition(UITools.ClampPosition(eventData.position, _transform), _transform);
            y = rel_pos.y;
            UpdateHue();
        }

        [Hotfixable]
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!UITools.IsInRect(eventData.position, _transform)) return;

            Vector2 rel_pos = UITools.GetRelativePosition(eventData.position, _transform);
            y = rel_pos.y;
            UpdateHue();
        }

        protected override void Init()
        {
            if (Initialized) return;
            base.Init();
            _image = GetComponent<Image>();
            _transform = GetComponent<RectTransform>();
            _texture = new Texture2D(width, height);
            _texture_pixels = _texture.GetPixels();
            _image.sprite = Sprite.Create(_texture, new Rect(0, 0, width, height), Vector2.zero);
            _pos = transform.Find("Position");
        }

        public void Setup(Vector2 size, float hue, Action<float> on_hue_changed)
        {
            SetSize(size);
            Init();
            y = hue * height;
            this.on_hue_changed = on_hue_changed;
            UpdateHue(false);
        }

        [Hotfixable]
        public void UpdateHue(bool update_color = true)
        {
            for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
                _texture_pixels[j * width + i] = Color.HSVToRGB(j / (float)height, 1, 1);

            _texture.SetPixels(_texture_pixels);
            _texture.Apply();
            _pos.position = new Vector3(_transform.position.x,
                _transform.position.y + (_transform.rect.y + y) * _transform.lossyScale.y);

            if (update_color) on_hue_changed?.Invoke(y / height);
        }

        private static void _init()
        {
            GameObject obj = new(nameof(HueSelector), typeof(Image));
            obj.transform.SetParent(Main.prefabs);

            GameObject pos = new("Position", typeof(Image));
            pos.transform.SetParent(obj.transform);
            pos.GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("ui/icons/iconWhiteCircle");
            pos.GetComponent<RectTransform>().sizeDelta = new Vector2(5, 5);

            Prefab = obj.AddComponent<HueSelector>();
        }
    }

    private class SubColorSelector : APrefab<SubColorSelector>, IPointerDownHandler, IDragHandler
    {
        private Color         _color;
        private Image         _image;
        private Transform     _pos;
        private Texture2D     _texture;
        private Color[]       _texture_pixels;
        private RectTransform _transform;
        private float         hue;
        private Action<Color> on_color_changed;

        private float x;
        private float y;
        private int   width  => (int)_transform.sizeDelta.x;
        private int   height => (int)_transform.sizeDelta.y;

        [Hotfixable]
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 rel_pos =
                UITools.GetRelativePosition(UITools.ClampPosition(eventData.position, _transform), _transform);
            y = rel_pos.y;
            x = rel_pos.x;
            UpdateSelectedColor();
        }

        [Hotfixable]
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!UITools.IsInRect(eventData.position, _transform)) return;

            Vector2 rel_pos = UITools.GetRelativePosition(eventData.position, _transform);
            y = rel_pos.y;
            x = rel_pos.x;
            UpdateSelectedColor();
        }

        protected override void Init()
        {
            if (Initialized) return;
            base.Init();
            _image = GetComponent<Image>();
            _transform = GetComponent<RectTransform>();
            _texture = new Texture2D(width, height);
            _texture_pixels = _texture.GetPixels();
            _image.sprite = Sprite.Create(_texture, new Rect(0, 0, width, height), Vector2.zero);
            _pos = transform.Find("Position");
        }

        public void Setup(Vector2 size, float hue, float s, float v, Action<Color> on_color_changed)
        {
            SetSize(size);
            Init();
            this.hue = hue;
            this.on_color_changed = on_color_changed;
            x = s * width;
            y = v * height;
            UpdateStauration(hue, false);
        }

        public void UpdateStauration(float hue, bool update_color = true)
        {
            this.hue = hue;
            for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
                _texture_pixels[j * width + i] = Color.HSVToRGB(hue, i / (float)width, j / (float)height);

            _texture.SetPixels(_texture_pixels);
            _texture.Apply();
            UpdateSelectedColor(update_color);
        }

        private void UpdateSelectedColor(bool update_color = true)
        {
            _color = Color.HSVToRGB(hue, x / width, y / height);
            if (update_color) on_color_changed?.Invoke(_color);

            _pos.position = new Vector3(_transform.position.x + (_transform.rect.x + x) * _transform.lossyScale.x,
                _transform.position.y + (_transform.rect.y + y) * _transform.lossyScale.y);
        }

        private static void _init()
        {
            GameObject obj = new(nameof(SubColorSelector), typeof(Image));
            obj.transform.SetParent(Main.prefabs);
            GameObject pos = new("Position", typeof(Image));
            pos.transform.SetParent(obj.transform);
            pos.GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("ui/icons/iconWhiteCircle");
            pos.GetComponent<RectTransform>().sizeDelta = new Vector2(5, 5);

            Prefab = obj.AddComponent<SubColorSelector>();
        }
    }
}