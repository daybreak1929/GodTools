using System.Collections.Generic;

namespace GodTools.Features.WorldScript;

public class ScriptInstance
{
    public string Name;
    public bool Enabled;
    public TriggerCondition TriggerCondition;
    public List<TargetFilter> Filters;
    public List<ScriptAction> Actions;
}