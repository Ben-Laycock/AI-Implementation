using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableIsBoidInAttackZone : EditableDecision
{
    public override void MakeDecision(AgentHandler agentScript)
    {
        Vector3 directionToPlayer = GameConstants.Instance.PlayerObject.transform.position - agentScript.transform.position;
        if (directionToPlayer.magnitude >= agentScript.GetTargetTooCloseRange() && directionToPlayer.magnitude <= agentScript.GetAttackRange())
        {
            RunChildDecision(agentScript, true);
        }
        else
        {
            RunChildDecision(agentScript, false);
        }
    }
}