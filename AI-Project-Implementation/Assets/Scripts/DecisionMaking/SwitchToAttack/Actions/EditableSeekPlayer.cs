using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableSeekPlayer : EditableAction
{
    public override void TakeAction(AgentHandler agentScript)
    {
        float distanceToPlayer = (GameConstants.Instance.PlayerObject.transform.position - agentScript.transform.position).magnitude;
        distanceToPlayer = Mathf.Clamp(distanceToPlayer, 0, 100);
        distanceToPlayer *= 0.01f;
        float mSeekWeight = distanceToPlayer * 1f;

        agentScript.BoidController.SeekPoint(GameConstants.Instance.PlayerObject.transform.position, mSeekWeight);
        agentScript.BoidController.SetShouldFlock(true);
    }
}