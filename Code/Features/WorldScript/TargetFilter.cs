namespace GodTools.Features.WorldScript;

public enum FilterGroup
{
    MustTrue,
    AnyTrue,
    MustFalse,
    AnyFalse
}
public class TargetFilter
{
    public string FilterID;
    public string Value;
    public FilterGroup Group;
}