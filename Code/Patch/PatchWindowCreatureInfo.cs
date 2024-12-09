using System;
using DG.Tweening;
using GodTools.UI;
using HarmonyLib;
using NeoModLoader.api.attributes;
using NeoModLoader.General;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace GodTools.Patch;

internal static class PatchWindowCreatureInfo
{
    private static bool initialized;

    private static float _last_y = 84;
    private static Image gender_image;
    private static Image saved_image;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(WindowCreatureInfo), nameof(WindowCreatureInfo.OnEnable))]
    public static void OnEnable_postfix(WindowCreatureInfo __instance)
    {
        Actor actor = __instance.actor;
        if (!initialized)
        {
            initialized = true;
            var gender = new GameObject("Gender", typeof(Image), typeof(Button));
            gender.transform.SetParent(__instance.avatarElement.transform);
            var gender_rect = gender.GetComponent<RectTransform>();
            gender_rect.localPosition = new Vector3(17.5f, -16.8f);
            gender_rect.localScale = Vector3.one;
            gender_rect.sizeDelta = new Vector2(16, 16);
            gender_image = gender.GetComponent<Image>();
            gender.GetComponent<Button>().onClick.AddListener(() =>
            {
                Actor actor = __instance.actor;
                if (actor.data.gender != ActorGender.Male)
                    actor.data.gender = ActorGender.Male;
                else
                    actor.data.gender = ActorGender.Female;

                actor.clearSprites();
                __instance.avatarElement.show(actor);
                CheckGender(actor);
            });

            //AddEntryButtonForWindow(__instance, nameof(WindowCreatureSpriteEditor), "ui/icons/iconAttractive");
            AddEntryButtonForWindow(__instance, nameof(WindowCreatureDataEditor), "ui/icons/iconOptions");

            Image mood_image = __instance.moodSprite;
            var mood_modify_button = mood_image.gameObject.AddComponent<Button>();
            var mood_tip_button = mood_image.gameObject.AddComponent<TipButton>();
            mood_tip_button.hoverAction = () =>
            {
                Tooltip.show(mood_image.gameObject, "mood", new TooltipData
                {
                    actor = __instance.actor
                });
                mood_image.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                mood_image.transform.DOKill();
                mood_image.transform.DOScale(1f, 0.1f)
                    .SetEase(Ease.InBack);
            };
            mood_modify_button.onClick.AddListener(() =>
            {
                var mood_list = AssetManager.moods.list;
                var curr_mood_idx = mood_list.FindIndex(x => x == AssetManager.moods.get(__instance.actor.data.mood));
                MoodAsset new_mood =
                    mood_list[Math.Min(mood_list.Count - 1, Math.Max(0, (curr_mood_idx + 1) % (mood_list.Count - 1)))];
                __instance.actor.changeMood(new_mood.id);
                mood_image.sprite = new_mood.getSprite();
            });


            GameObject save_actor =
                Object.Instantiate(__instance.buttonTraitEditor, __instance.transform.Find("Background"));
            save_actor.transform.localPosition = new Vector3(116.8f, -112f);
            save_actor.transform.localScale = new Vector3(1,         1);
            saved_image = save_actor.transform.Find("Button Trait/Icon").GetComponent<Image>();
            saved_image.sprite =
                Resources.Load<Sprite>("gt_windows/save_actor");
            var button = save_actor.transform.Find("Button Trait").GetComponent<Button>();
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener([Hotfixable]() =>
            {
                WindowCreatureSavedList save_list = WindowCreatureSavedList.Instance;
                if (save_list.ActorSaved(__instance.actor))
                    save_list.UnsaveActor(__instance.actor);
                else
                    save_list.SaveActor(__instance.actor);

                CheckSave(__instance.actor, saved_image);
            });
            button.GetComponent<TipButton>().textOnClick = "save_actor";
#if 一米_中文名
            __instance.nameInput.addListener(new_name => { });
#endif
        }

        WindowStatusEffectEditor.init(__instance);
        if (actor.asset.use_items)
        {
            WindowItemEditor.init(__instance);
            WindowItemEditor.entry_button.SetActive(true);
        }
        else if (WindowItemEditor.entry_button != null)
        {
            WindowItemEditor.entry_button.SetActive(false);
        }

        CheckGender(actor);
        CheckSave(actor, saved_image);
    }

    private static void CheckSave(Actor actor, Image image)
    {
        WindowCreatureSavedList save_list = WindowCreatureSavedList.Instance;
        if (save_list.ActorSaved(actor))
            image.color = Color.white;
        else
            image.color = Color.gray;
    }

    private static void CheckGender(Actor actor)
    {
        if (actor.data.gender == ActorGender.Unknown)
        {
            gender_image.enabled = false;
        }
        else
        {
            gender_image.enabled = true;
            gender_image.sprite =
                SpriteTextureLoader.getSprite(actor.data.gender == ActorGender.Male
                    ? "gt_windows/male"
                    : "gt_windows/female");
        }
    }

    private static void AddEntryButtonForWindow(WindowCreatureInfo target_window, string entry_window_id, string icon)
    {
        PowerButtonCreator.CreateWindowButton(entry_window_id, entry_window_id,
            SpriteTextureLoader.getSprite(icon),
            target_window.transform.Find("Background"), new Vector2(156, _last_y));
        _last_y -= 40;
    }

    private static void AddSideButton(WindowCreatureInfo target_window, string entry_window_id, string icon)
    {
        PowerButtonCreator.CreateWindowButton(entry_window_id, entry_window_id,
            SpriteTextureLoader.getSprite(icon),
            target_window.transform.Find("Background"), new Vector2(156, _last_y));
        _last_y -= 40;
    }
}