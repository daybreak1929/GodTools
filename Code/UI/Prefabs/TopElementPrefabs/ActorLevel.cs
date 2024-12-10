using NeoModLoader.General.UI.Prefabs;
using UnityEngine;

namespace GodTools.UI.Prefabs.TopElementPrefabs;

public class ActorLevel : APrefab<ActorLevel>
{
    public RawText           Order   { get; private set; }
    public PrefabUnitElement Element { get; private set; }

    protected override void Init()
    {
        if (Initialized) return;
        base.Init();
        Element = GetComponent<PrefabUnitElement>();
        Order = transform.Find(nameof(Order)).GetComponent<RawText>();
    }

    public void Setup(Actor actor, int order)
    {
        Init();
        Order.Setup(order.ToString());
        Element.show(actor);
    }

    private static void _init()
    {
        GameObject obj = Instantiate(Resources.Load<WindowFavorites>("windows/favorites").element_prefab.gameObject,
            Main.prefabs);
        obj.name = nameof(ActorLevel);

        RawText order = Instantiate(RawText.Prefab, obj.transform);
        order.name = nameof(Order);
        order.transform.localPosition = new Vector3(-82, 13f);
        order.transform.localScale = Vector3.one;
        order.SetSize(new Vector2(30, 10));

        Prefab = obj.AddComponent<ActorLevel>();
    }
}