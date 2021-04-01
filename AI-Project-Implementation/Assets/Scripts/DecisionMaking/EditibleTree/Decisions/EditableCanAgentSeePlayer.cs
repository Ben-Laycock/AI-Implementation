using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableCanAgentSeePlayer : EditableDecision
{
    public override void MakeDecision(AgentHandler agentScript)
    {
        Vector3 directionToPlayer = GameConstants.Instance.PlayerObject.transform.position - agentScript.transform.position;
        if (!Physics.Raycast(agentScript.transform.position, directionToPlayer.normalized, directionToPlayer.magnitude, LayerMask.GetMask("Default")))
        {
            RunChildDecision(agentScript, true);
        }
        else
        {
            RunChildDecision(agentScript, false);
        }
    }
}