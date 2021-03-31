using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableTradeEnergyForHealth : EditableAction
{
    public override void TakeAction(AgentInformation agentInformation)
    {
        Debug.Log("Trading Energy For Health!");
        agentInformation.mAgentEnergy -= 10;
        agentInformation.mAgentHealth += 10;
    }
}