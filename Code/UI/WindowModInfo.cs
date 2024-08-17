using NeoModLoader.General.UI.Window;

namespace GodTools.UI;

public class WindowModInfo : MultiTabWindow<WindowModInfo>
{
    public static string WINDOW_ID { get; private set; }

    protected override void Init()
    {
        WINDOW_ID = WindowID;
        InitMainPage();
        InitWorldPage();
        InitActorPage();
        InitBuildingPage();
        InitCityPage();
        InitKingdomPage();
        InitCulturePage();
        InitClanPage();
        InitTilePage();
        InitOtherPage();
    }

    private void InitOtherPage()
    {
    }

    private void InitTilePage()
    {
    }

    private void InitClanPage()
    {
    }

    private void InitCulturePage()
    {
    }

    private void InitKingdomPage()
    {
    }

    private void InitCityPage()
    {
    }

    private void InitBuildingPage()
    {
    }

    private void InitActorPage()
    {
    }

    private void InitWorldPage()
    {
    }

    private void InitMainPage()
    {
    }
}