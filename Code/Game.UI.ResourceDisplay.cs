using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.Game.UI;

public class ResourceDisplay : MonoBehaviour
{
    private Text gold_text;
    private Text wood_text;
    private Text stone_text;
    private Text pop_text;
    public void init()
    {
        HorizontalLayoutGroup hori_layout_group = gameObject.AddComponent<HorizontalLayoutGroup>();
        hori_layout_group.childForceExpandHeight = false;
        hori_layout_group.childForceExpandWidth = true;
        hori_layout_group.childAlignment = TextAnchor.MiddleCenter;
        hori_layout_group.childControlHeight = false;
        hori_layout_group.childControlWidth = true;
        
        GameObject gold_info = new("Gold", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(ContentSizeFitter));
        gold_info.transform.SetParent(transform);
        hori_layout_group = gold_info.GetComponent<HorizontalLayoutGroup>();
        hori_layout_group.childForceExpandHeight = false;
        hori_layout_group.childForceExpandWidth = false;
        hori_layout_group.childControlHeight = false;
        hori_layout_group.childControlWidth = false;
        hori_layout_group.childAlignment = TextAnchor.MiddleCenter;
        hori_layout_group.spacing = 12;
        ContentSizeFitter content_fitter = gold_info.GetComponent<ContentSizeFitter>();
        content_fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        content_fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        
        GameObject gold_icon = new("Icon", typeof(Image));
        gold_icon.transform.SetParent(gold_info.transform);
        gold_icon.GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("ui/icons/iconResGold");
        gold_icon.GetComponent<RectTransform>().sizeDelta = new(28, 28);
        
        GameObject gold_text_obj = new("Text", typeof(Text), typeof(ContentSizeFitter));
        gold_text_obj.transform.SetParent(gold_info.transform);
        content_fitter = gold_text_obj.GetComponent<ContentSizeFitter>();
        content_fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        gold_text = gold_text_obj.GetComponent<Text>();
        gold_text.font = LocalizedTextManager.currentFont;
        gold_text.alignment = TextAnchor.MiddleCenter;
        gold_text.text = "0";

        GameObject wood_info = Instantiate(gold_info, transform);
        wood_info.name = "Wood";
        wood_text = wood_info.transform.Find("Text").GetComponent<Text>();
        wood_info.transform.Find("Icon").GetComponent<Image>().sprite =
            SpriteTextureLoader.getSprite("ui/icons/iconResWood");
        
        GameObject stone_info = Instantiate(gold_info, transform);
        stone_info.name = "Stone";
        stone_text = stone_info.transform.Find("Text").GetComponent<Text>();
        stone_info.transform.Find("Icon").GetComponent<Image>().sprite =
            SpriteTextureLoader.getSprite("ui/icons/iconResStone");
        
        GameObject pop_info = Instantiate(gold_info, transform);
        pop_info.name = "Population";
        pop_text = pop_info.transform.Find("Text").GetComponent<Text>();
        pop_text.text = "0/0";
        pop_info.transform.Find("Icon").GetComponent<Image>().sprite =
            SpriteTextureLoader.getSprite("ui/icons/iconPopulation");

    }
    public void update()
    {
        gold_text.text = "0";
        wood_text.text = "0";
        stone_text.text = "0";
        pop_text.text = "0/0";
        if(!Game.instance.is_running) return;
        if (Game.instance.player == null || Game.instance.player.capital == null) return;
        City capital = Game.instance.player.capital;
        Dictionary<string, CityStorageSlot> resources = capital.data.storage.resources;
        CityStorageSlot slot;
        if (resources.TryGetValue(SR.gold, out slot))
        {
            gold_text.text = slot.amount.ToString();
        }

        if (resources.TryGetValue(SR.wood, out slot))
        {
            wood_text.text = slot.amount.ToString();
        }

        if (resources.TryGetValue(SR.stone, out slot))
        {
            stone_text.text = slot.amount.ToString();
        }

        pop_text.text = $"{capital.getPopulationTotal()}/{capital.getPopulationMaximum()}";
    }
}