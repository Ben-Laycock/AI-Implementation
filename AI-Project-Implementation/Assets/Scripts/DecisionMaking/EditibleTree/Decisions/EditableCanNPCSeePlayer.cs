using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableCanNPCSeePlayer : EditableDecision
{
    public override void MakeDecision(AgentInformation agentInformation)
    {
        if (agentInformation.mAgentHealth < 10)
        {
            RunChildDecision(agentInformation, true);
        }
        else
        {
            RunChildDecision(agentInformation, false);
        }
    }
}