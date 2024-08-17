using NeoModLoader.General.UI.Window;

namespace GodTools.UI;

public partial class WindowCreatureDataEditor : MultiTabWindow<WindowCreatureDataEditor>
{
    private Actor _selected_actor;

    protected override void Init()
    {
        InitializeVanilla();
#if 启源_启源核心
        InitializeCwCore();
#endif
    }

    public override void OnNormalEnable()
    {
        _selected_actor = Config.selectedUnit;
    }
}