using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableMoreThanTenEnergy : EditableDecision
{
    public override void MakeDecision(AgentHandler agentScript)
    {
        if (agentScript.GetEnergy() > 10)
        {
            RunChildDecision(agentScript, true);
        }
        else
        {
            RunChildDecision(agentScript, false);
        }
    }
}