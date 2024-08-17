using System.Collections.Generic;
using System.Linq;
using GodTools.UI.Prefabs;
using HarmonyLib;
using NeoModLoader.api.attributes;
using NeoModLoader.General.UI.Prefabs;
using NeoModLoader.General.UI.Window;
using NeoModLoader.General.UI.Window.Layout;
using NeoModLoader.General.UI.Window.Utils.Extensions;
using UnityEngine;

namespace GodTools.UI;

public class WindowCreatureSpriteEditor : AutoLayoutWindow<WindowCreatureSpriteEditor>
{
    private static          bool                                              patched;
    private static readonly Dictionary<string, Dictionary<string, FrameData>> modified_units_sprites = new();
    private                 ColorSelector                                     _head_color_selector;
    private                 AutoVertLayoutGroup                               _head_part;
    private                 SpriteDisplay                                     _head_sprite_display;
    private                 RawText                                           _head_sprite_title;
    private                 ColorSelector                                     _main_color_selector;
    private                 AutoVertLayoutGroup                               _main_part;
    private                 SpriteDisplay                                     _main_sprite_display;
    private                 RawText                                           _main_sprite_title;

    private          Actor                        actor;
    private readonly List<Sprite>                 available_heads = new();
    private readonly List<AnimationContainerUnit> available_mains = new();
    private          Sprite                       cached_head;
    private          AnimationContainerUnit       cached_main;

    protected override void Init()
    {
        _head_part = this.BeginVertGroup();
        _head_sprite_title = Instantiate(RawText.Prefab);
        _head_sprite_title.SetSize(new Vector2(100, 25));
        _head_sprite_title.Setup("头部调整", Color.white, TextAnchor.MiddleCenter);
        _head_sprite_display = Instantiate(SpriteDisplay.Prefab);
        _head_part.AddChild(_head_sprite_title.gameObject);

        SimpleButton head_left = Instantiate(SimpleButton.Prefab);
        SimpleButton head_right = Instantiate(SimpleButton.Prefab);
        head_left.Setup(() => { UpdateActorHead(actor.cached_sprite_head, -1); },
                        SpriteTextureLoader.getSprite("ui/icons/iconArrowBack"), pSize: new Vector2(32, 32),
                        pTipType: "tip",
                        pTipData: new TooltipData
                        {
                            tip_name = "前一个头型"
                        });
        head_right.Setup(() => { UpdateActorHead(actor.cached_sprite_head, 1); },
                         SpriteTextureLoader.getSprite("ui/icons/iconArrowBack"), pSize: new Vector2(32, 32),
                         pTipType: "tip",
                         pTipData: new TooltipData
                         {
                             tip_name = "后一个头型"
                         });
        head_left.Background.enabled = false;
        head_right.Background.enabled = false;
        head_right.Icon.transform.localScale = new Vector3(-1, 1, 1);

        AutoHoriLayoutGroup _head_adjust = _head_part.BeginHoriGroup(pAlignment: TextAnchor.MiddleCenter);
        _head_adjust.AddChild(head_left.gameObject);
        _head_adjust.AddChild(_head_sprite_display.gameObject);
        _head_adjust.AddChild(head_right.gameObject);

        _main_part = this.BeginVertGroup();
        _main_sprite_title = Instantiate(RawText.Prefab);
        _main_sprite_title.SetSize(new Vector2(100, 25));
        _main_sprite_title.Setup("身体调整", Color.white, TextAnchor.MiddleCenter);
        _main_part.AddChild(_main_sprite_title.gameObject);

        _main_sprite_display = Instantiate(SpriteDisplay.Prefab);
        SimpleButton main_left = Instantiate(SimpleButton.Prefab);
        SimpleButton main_right = Instantiate(SimpleButton.Prefab);
        main_left.Setup(() => { UpdateActorMain(actor._last_main_sprite, -1); },
                        SpriteTextureLoader.getSprite("ui/icons/iconArrowBack"), pSize: new Vector2(32, 32),
                        pTipType: "tip",
                        pTipData: new TooltipData
                        {
                            tip_name = "前一个身型"
                        });
        main_right.Setup([Hotfixable]() => { UpdateActorMain(actor._last_main_sprite, 1); },
                         SpriteTextureLoader.getSprite("ui/icons/iconArrowBack"), pSize: new Vector2(32, 32),
                         pTipType: "tip",
                         pTipData: new TooltipData
                         {
                             tip_name = "后一个身型"
                         });
        main_left.Background.enabled = false;
        main_right.Background.enabled = false;
        main_right.Icon.transform.localScale = new Vector3(-1, 1, 1);

        AutoHoriLayoutGroup _main_adjust = _main_part.BeginHoriGroup(pAlignment: TextAnchor.MiddleCenter);
        _main_adjust.AddChild(main_left.gameObject);
        _main_adjust.AddChild(_main_sprite_display.gameObject);
        _main_adjust.AddChild(main_right.gameObject);

        _head_color_selector = Instantiate(ColorSelector.Prefab, BackgroundTransform);
        _head_color_selector.gameObject.SetActive(false);
        _head_color_selector.transform.localPosition = new Vector3(-200, 0);
        _head_color_selector.transform.localScale = Vector3.one;
        _main_color_selector = Instantiate(ColorSelector.Prefab, BackgroundTransform);
        _main_color_selector.gameObject.SetActive(false);
        _main_color_selector.transform.localPosition = new Vector3(-200, 0);
        _main_color_selector.transform.localScale = Vector3.one;
    }

    [Hotfixable]
    public void UpdateActorMain(Sprite curr_main, int offset = 0)
    {
        cached_main =
            available_mains[Mathf.Clamp(available_mains.IndexOf(cached_main) + offset, 0, available_mains.Count - 1)];
        actor._last_main_sprite = cached_main.sprites[curr_main.name];
        _main_sprite_display.Setup(actor._last_main_sprite,
                                   (display, x, y) => CallColorSelector(_main_color_selector, display, x, y));
        _main_color_selector.gameObject.SetActive(false);
        _main_sprite_title.Text.text = $"身体调整({cached_main.id})";
    }

    [Hotfixable]
    public void UpdateActorHead(Sprite curr_head, int offset = 0)
    {
        if (!actor.asset.unit)
        {
            actor.data.head = Mathf.Clamp(actor.data.head + offset, 0, actor.animationContainer.heads.Length - 1);
            actor.dirty_sprite_head = true;
            actor.checkSpriteHead();

            cached_head = actor.cached_sprite_head;
        }
        else
        {
            var idx = Mathf.Clamp(available_heads.IndexOf(curr_head) + offset, 0, available_heads.Count - 1);
            cached_head = available_heads[idx];
            actor.cached_sprite_head = cached_head;
        }

        _head_sprite_display.Setup(cached_head,
                                   (display, x, y) => CallColorSelector(_head_color_selector, display, x, y));
        _head_sprite_title.Text.text = $"头部调整({cached_head.name})";
        _head_color_selector.gameObject.SetActive(false);
    }

    [Hotfixable]
    public override void OnNormalEnable()
    {
        actor = Config.selectedUnit;
        if (actor.asset.has_override_sprite) return;

        _head_sprite_title.gameObject.SetActive(actor.asset.body_separate_part_head);
        _head_sprite_display.gameObject.SetActive(actor.asset.body_separate_part_head);

        available_heads.Clear();
        available_mains.Clear();

        cached_head = actor.cached_sprite_head;
        cached_main = actor.animationContainer;

        Sprite main_sprite = actor._last_main_sprite;
        if (modified_units_sprites.TryGetValue(actor.data.id, out var sprite_dict))
        {
            if (sprite_dict.TryGetValue("head", out FrameData new_head_sprite))
            {
                cached_head = new_head_sprite.sprite;
                if (actor.asset.unit)
                    available_heads.Add(cached_head);
            }

            if (sprite_dict.TryGetValue(actor._last_main_sprite.name, out FrameData new_main_sprite))
            {
                main_sprite = new_main_sprite.sprite;
                cached_main = new AnimationContainerUnit();
                cached_main.id = "custom";
                cached_main.dict_frame_data = new Dictionary<string, AnimationFrameData>();
                cached_main.sprites = new Dictionary<string, Sprite>();
                foreach (var item in sprite_dict)
                {
                    if (item.Key == "head") continue;
                    cached_main.dict_frame_data[item.Key] = item.Value.anim_frame_data;
                    cached_main.sprites[item.Key] = item.Value.sprite;
                }

                available_mains.Add(cached_main);
            }
        }

        if (actor.asset.body_separate_part_head)
        {
            if (actor.asset.unit)
            {
                var sprites = Resources.LoadAll<Sprite>($"actors/races/{actor.asset.race}");
                available_heads.AddRange(sprites.Where(s => s.name.Contains("head_") || s.name.Contains("heads_")));
            }

            _head_sprite_display.Setup(cached_head,
                                       (display, x, y) => CallColorSelector(_head_color_selector, display, x, y));
            _head_sprite_title.Text.text = $"头部调整({cached_head.name})";
        }

        if (actor.asset.unit)
        {
            available_mains.AddRange(actor.race.skin_citizen_male.Select(
                                         s => ActorAnimationLoader.loadAnimationUnit(
                                             $"actors/{actor.race.main_texture_path}{s}", actor.asset)));

            available_mains.AddRange(actor.race.skin_citizen_female.Select(
                                         s => ActorAnimationLoader.loadAnimationUnit(
                                             $"actors/{actor.race.main_texture_path}{s}", actor.asset)));

            available_mains.AddRange(actor.race.skin_warrior.Select(
                                         s => ActorAnimationLoader.loadAnimationUnit(
                                             $"actors/{actor.race.main_texture_path}{s}", actor.asset)));
        }

        _main_sprite_display.Setup(main_sprite,
                                   (display, x, y) => CallColorSelector(_main_color_selector, display, x, y));
        _main_sprite_title.Text.text = $"身体调整({cached_main.id})";
    }

    public override void OnNormalDisable()
    {
        if (actor == null) return;
        ApplyChangeAsFinalSprite(true);
        if (ScrollWindow.get("inspect_unit") is { } scroll_window && Config.selectedUnit != null)
            scroll_window.GetComponent<WindowCreatureInfo>()?.avatarElement?.avatarLoader?.load(Config.selectedUnit);
    }

    public void CallColorSelector(ColorSelector selector, SpriteDisplay display, int x, int y)
    {
        if (selector == _main_color_selector)
            _head_color_selector.gameObject.SetActive(false);
        else
            _main_color_selector.gameObject.SetActive(false);

        selector.gameObject.SetActive(true);

        selector.Setup(display.GetPixelColor(x, y),
                       [Hotfixable](c) => { display.ReplaceColor(display.GetPixelOriginColor(x, y), c); },
                       new KeyValuePair<Color, string>[]
                       {
                           new(Toolbox.color_green_0, "肤色替换0号"),
                           new(Toolbox.color_green_1, "肤色替换1号"),
                           new(Toolbox.color_green_2, "肤色替换2号"),
                           new(Toolbox.color_green_3, "肤色替换3号"),
                           new(Toolbox.color_magenta_0, "国家颜色替换0号"),
                           new(Toolbox.color_magenta_1, "国家颜色替换1号"),
                           new(Toolbox.color_magenta_2, "国家颜色替换2号"),
                           new(Toolbox.color_magenta_3, "国家颜色替换3号"),
                           new(Toolbox.color_magenta_4, "国家颜色替换4号"),
                           new(Toolbox.color_teal_0, "国家颜色替换5号"),
                           new(Toolbox.color_teal_1, "国家颜色替换6号"),
                           new(Toolbox.color_teal_2, "国家颜色替换7号"),
                           new(Toolbox.color_teal_3, "国家颜色替换8号"),
                           new(Toolbox.color_teal_4, "国家颜色替换9号")
                       });
    }

    [Hotfixable]
    public void ApplyChangeAsFinalSprite(bool clean = false)
    {
        if (actor.asset.has_override_sprite) return;
        Dictionary<string, FrameData> sprite_dict = new();
        if (modified_units_sprites.TryGetValue(actor.data.id, out var old_sprite_dict)) sprite_dict = old_sprite_dict;

        if (actor.asset.body_separate_part_head)
        {
            var color_map = _head_sprite_display.GetColorMap();
            actor.dirty_sprite_head = true;
            actor.checkSpriteHead();
            if (color_map.Count > 0)
                sprite_dict["head"] = new FrameData(_head_sprite_display.GetCurrentSprite(cached_head));
            else if (cached_head != actor.cached_sprite_head) sprite_dict["head"] = new FrameData(cached_head);
        }

        var color_map_main = _main_sprite_display.GetColorMap();
        if (color_map_main.Count > 0)
            foreach (var item in cached_main.dict_frame_data)
            {
                Sprite old_sprite = cached_main.sprites[item.Key];
                var texture = new Texture2D((int)old_sprite.rect.width, (int)old_sprite.rect.height,
                                            old_sprite.texture.format, false);
                texture.filterMode = old_sprite.texture.filterMode;
                for (var x = 0; x < texture.width; x++)
                for (var y = 0; y < texture.height; y++)
                {
                    Color origin_color =
                        old_sprite.texture.GetPixel(x + (int)old_sprite.rect.xMin, y + (int)old_sprite.rect.yMin);
                    if (color_map_main.TryGetValue(origin_color, out Color new_color))
                        texture.SetPixel(x, y, new_color);
                    else
                        texture.SetPixel(x, y, origin_color);
                }

                texture.Apply();
                sprite_dict[item.Key] = new FrameData(Sprite.Create(
                                                          texture, new Rect(0, 0, texture.width, texture.height),
                                                          new Vector2(old_sprite.pivot.x / texture.width,
                                                                      old_sprite.pivot.y / texture.height),
                                                          old_sprite.pixelsPerUnit));
                sprite_dict[item.Key].sprite.name = item.Key;
                sprite_dict[item.Key].anim_frame_data = item.Value;
            }
        else if (cached_main != actor.animationContainer)
            foreach (var item in cached_main.dict_frame_data)
            {
                sprite_dict[item.Key] = new FrameData(cached_main.sprites[item.Key]);
                sprite_dict[item.Key].anim_frame_data = item.Value;
            }

        if (sprite_dict.Count > 0)
        {
            modified_units_sprites[actor.data.id] = sprite_dict;
            if (!patched)
            {
                patched = true;
                new Harmony("inmny.godtools.sprite_editor").Patch(
                    AccessTools.Method(typeof(UnitSpriteConstructor), nameof(UnitSpriteConstructor.getSpriteUnit)),
                    new HarmonyMethod(typeof(WindowCreatureSpriteEditor),
                                      nameof(Prefix_getSpriteUnit)));
            }

            actor._last_main_sprite = null;
            foreach (var item in sprite_dict) Main.LogInfo($"{item.Key}: {item.Value.sprite.GetInstanceID()}");
        }

        if (clean) actor = null;
    }

    [Hotfixable]
    private static bool Prefix_getSpriteUnit(ref Sprite __result, AnimationFrameData pFrameData, Sprite pMainSprite,
                                             Actor pActor,
                                             ColorAsset pColor, int pSkinSet, int pSkinColor,
                                             UnitTextureAtlasID pTextureAtlasID)
    {
        if (!modified_units_sprites.TryGetValue(pActor.data.id, out var sprite_dict)) return true;

        if (pSkinSet == -1) pSkinSet = 0;

        var color_id = 0L;
        var skin_color_id = 0L;
        var skin_id = 0L;
        var head_id = 0L;
        if (sprite_dict.TryGetValue(pMainSprite.name, out FrameData main_frame))
        {
            pMainSprite = main_frame.sprite;
            pFrameData = main_frame.anim_frame_data;
        }

        long body_id = UnitSpriteConstructor.getBodySpriteSmallID(pMainSprite);
        Sprite head_sprite = null;
        if (sprite_dict.TryGetValue("head", out FrameData head_frame))
        {
            ActorAnimationLoader.int_ids_heads.TryGetValue(head_frame.sprite, out var value);
            if (value == 0)
            {
                value = ActorAnimationLoader.int_ids_heads.Count + 1;
                ActorAnimationLoader.int_ids_heads.Add(head_frame.sprite, value);
            }

            head_id = value;
            pActor.cached_sprite_head = head_frame.sprite;
            head_sprite = head_frame.sprite;
        }
        else if (pActor.has_rendered_sprite_head)
        {
            ActorAnimationLoader.int_ids_heads.TryGetValue(pActor.cached_sprite_head, out var value);
            if (value == 0)
            {
                value = ActorAnimationLoader.int_ids_heads.Count + 1;
                ActorAnimationLoader.int_ids_heads.Add(pActor.cached_sprite_head, value);
            }

            head_sprite = pActor.cached_sprite_head;
            head_id = value;
        }

        if (pSkinColor != -1)
        {
            skin_id = pSkinSet         + 1;
            skin_color_id = pSkinColor + 1;
            if (pColor != null) color_id = pColor.index_id + 1;
        }
        else if (pColor != null)
        {
            color_id = pColor.index_id + 1;
        }

        var final_sprite_uid =
            color_id * 100000000L + head_id * 100000L + body_id * 100L + skin_id * 10L + skin_color_id;
        if (!UnitSpriteConstructor._sprites_units.TryGetValue(final_sprite_uid, out Sprite final_sprite))
        {
            final_sprite = UnitSpriteConstructor.createNewSpriteUnit(pFrameData, pMainSprite, head_sprite, pColor,
                                                                     pActor.asset, pSkinSet, pSkinColor,
                                                                     pTextureAtlasID);
            UnitSpriteConstructor._sprites_units[final_sprite_uid] = final_sprite;
        }

        __result = final_sprite;
        return false;
    }

    private class FrameData
    {
        public          AnimationFrameData anim_frame_data;
        public readonly Sprite             sprite;

        public FrameData(Sprite sprite)
        {
            this.sprite = sprite;
        }
    }
}