using GodTools.UI;
using HarmonyLib;
using NeoModLoader.General;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.HarmonySpace;

internal static class H_WindowPatch
{
    private static bool initialized;

    private static float _last_y = 84;
    private static Image gender_image;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(WindowCreatureInfo), "OnEnable")]
    public static void item_editor_init(WindowCreatureInfo __instance)
    {
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

            AddEntryButtonForWindow(__instance, nameof(WindowCreatureSpriteEditor), "ui/icons/iconAttractive");
            AddEntryButtonForWindow(__instance, nameof(WindowCreatureDataEditor), "ui/icons/iconOptions");
#if 一米_中文名
            __instance.nameInput.addListener(new_name => { });
#endif
        }

        WindowStatusEffectEditor.init(__instance);
        Actor actor = __instance.actor;
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
}