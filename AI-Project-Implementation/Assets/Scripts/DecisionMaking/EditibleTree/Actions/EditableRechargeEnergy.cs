using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableRechargeEnergy : EditableAction
{
    public override void TakeAction(AgentHandler agentScript)
    {
        agentScript.ChangeAgentEnergyBy(1);
        agentScript.SetEnergyRegenTimer(0.0f);
    }
}