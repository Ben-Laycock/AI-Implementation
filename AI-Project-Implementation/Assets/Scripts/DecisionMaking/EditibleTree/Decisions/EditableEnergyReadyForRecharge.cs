using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableEnergyReadyForRecharge : EditableDecision
{
    public override void MakeDecision(AgentHandler agentScript)
    {
        if (agentScript.GetEnergyRegenTimer() >= agentScript.TimeToRegenEnergy)
        {
            RunChildDecision(agentScript, true);
        }
        else
        {
            RunChildDecision(agentScript, false);
        }
    }
}