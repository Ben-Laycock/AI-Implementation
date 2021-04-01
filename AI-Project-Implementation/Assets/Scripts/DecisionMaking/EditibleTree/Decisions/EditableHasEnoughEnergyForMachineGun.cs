using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableHasEnoughEnergyForMachineGun : EditableDecision
{
    public override void MakeDecision(AgentHandler agentScript)
    {
        if (agentScript.GetEnergy() >= agentScript.GetMachineGunEnergyCost())
        {
            RunChildDecision(agentScript, true);
        }
        else
        {
            RunChildDecision(agentScript, false);
        }
    }
}