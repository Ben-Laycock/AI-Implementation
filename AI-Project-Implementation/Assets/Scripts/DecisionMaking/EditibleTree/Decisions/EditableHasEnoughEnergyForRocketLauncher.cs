using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableHasEnoughEnergyForRocketLauncher : EditableDecision
{
    public override void MakeDecision(AgentHandler agentScript)
    {
        if (agentScript.GetEnergy() >= agentScript.GetRocketLauncherEnergyCost())
        {
            RunChildDecision(agentScript, true);
        }
        else
        {
            RunChildDecision(agentScript, false);
        }
    }
}