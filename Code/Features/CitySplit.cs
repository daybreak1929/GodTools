namespace GodTools.Features;

public static class CitySplit
{
    private static State state = State.SelectMajorPart;

    public static bool ResetCitySplitState(string power_id)
    {
        state = State.SelectMajorPart;
        return true;
    }

    private enum State
    {
        SelectMajorPart,
        SelectPointStart,
        SelectPointEnd
    }
}