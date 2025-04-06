using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.UI;

namespace GodTools.UI.Prefabs.TopElementPrefabs;

public class TopElementActor : APrefab<TopElementActor>
{
    public RawText           Order   => _order;
    public PrefabUnitElement Element => _element;
    public RawText Disp => _disp;
    [SerializeField]
    private RawText _disp;
    [SerializeField]
    private RawText _order;
    [SerializeField]
    private PrefabUnitElement _element;
    
    public void Setup(Actor actor, int order, string disp)
    {
        Order.Setup(order.ToString());
        Disp.Setup(disp, alignment: TextAnchor.UpperRight);
        Element.show(actor);
    }

    private static void _init()
    {
        GameObject obj = Instantiate(Resources.Load<ListWindow>("windows/list_favorite_units")._list_element_prefab,
            Main.prefabs);
        obj.name = nameof(TopElementActor);

        RawText order = Instantiate(RawText.Prefab, obj.transform);
        order.name = nameof(Order);
        order.AddComponent<LayoutElement>().ignoreLayout = true;
        order.SetSize(new Vector2(30, 10));
        order.transform.localPosition = new Vector3(-90, 13f);
        order.transform.localScale = Vector3.one;
        
        RawText disp = Instantiate(RawText.Prefab, obj.transform);
        disp.name = nameof(Disp);
        disp.AddComponent<LayoutElement>().ignoreLayout = true;
        disp.SetSize(new Vector2(100, 10));
        disp.transform.localPosition = new Vector3(18, 8f);
        disp.transform.localScale = Vector3.one;
        

        Prefab = obj.AddComponent<TopElementActor>();
        Prefab._order = order;
        Prefab._disp = disp;
        Prefab._element = obj.GetComponent<PrefabUnitElement>();
    }
}