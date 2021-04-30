using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableFleeFromPlayer : EditableAction
{
    public override void TakeAction(AgentHandler agentScript)
    {
        float distanceToPlayer = (GameConstants.Instance.PlayerObject.transform.position - agentScript.transform.position).magnitude;
        distanceToPlayer = 10 - Mathf.Clamp(distanceToPlayer, 0, 10);
        distanceToPlayer *= 0.1f;
        float fleeWeight = distanceToPlayer * 0.5f;
        agentScript.BoidController.SetTarget(GameConstants.Instance.PlayerObject.transform.position);
        agentScript.BoidController.FleePoint(GameConstants.Instance.PlayerObject.transform.position, fleeWeight);
        agentScript.BoidController.SetShouldFlock(true);
    }
}