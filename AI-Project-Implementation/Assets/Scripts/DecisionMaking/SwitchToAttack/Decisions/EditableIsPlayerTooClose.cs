using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableIsPlayerTooClose : EditableDecision
{
    public override void MakeDecision(AgentHandler agentScript)
    {
        if(agentScript.GetEnergyRegenTimer() >= agentScript.TimeToRegenEnergy)
        {
            agentScript.ChangeAgentEnergyBy(1);
            agentScript.SetEnergyRegenTimer(0.0f);
        }

        Vector3 directionToPlayer = GameConstants.Instance.PlayerObject.transform.position - agentScript.transform.position;
        if (directionToPlayer.magnitude <= agentScript.GetAttackRange())
        {
            RunChildDecision(agentScript, true);
        }
        else
        {
            RunChildDecision(agentScript, false);
        }
    }
}