using System.Linq;
using GodTools.Features;
using GodTools.Libraries;
using GodTools.UI.Prefabs;
using NeoModLoader.api;
using NeoModLoader.General;
using NeoModLoader.General.UI.Window;
using NeoModLoader.General.UI.Window.Layout;
using NeoModLoader.General.UI.Window.Utils.Extensions;
using UnityEngine;

namespace GodTools.UI;

public class WindowConsistentCreaturePowerTop : SingleAutoLayoutWindow<WindowConsistentCreaturePowerTop>
{
    private ObjectPoolGenericMono<ConsistentCreaturePowerElement> _pool;
    private PowerButton _hidden_button;
    protected override void Init()
    {
        ContentTransform.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
        _pool = new ObjectPoolGenericMono<ConsistentCreaturePowerElement>(
            ConsistentCreaturePowerElement.Prefab, ContentTransform);
        _hidden_button = PowerButtonCreator.CreateGodPowerButton(GodPowers.place_saved_actor_in_top.id,
            SpriteTextureLoader.getSprite("gt_windows/save_actor_list"), Main.prefabs);
    }

    public override void OnNormalEnable()
    {
        base.OnNormalEnable();
        _pool.clear();
        var list = ConsistentCreaturePowerTop.GetSortedCreaturePowerData();
        
        for (int i = list.Count - 1; i >= 0; i--)
        {
            var element = _pool.getNext();
            element.Setup(list[i], list.Count - 1 - i, () =>
            {
                ScrollWindowComponent.clickCloseButton();
                _hidden_button.clickActivePower();
            });
        }
    }
}