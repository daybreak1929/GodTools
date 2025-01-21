namespace GodTools.Features.WorldScript;

public enum TriggerConditionType
{
    Time,
    Always,
    UpdateStats
}
public class TriggerCondition
{
    public TriggerConditionType Type;
    public float Value;
}