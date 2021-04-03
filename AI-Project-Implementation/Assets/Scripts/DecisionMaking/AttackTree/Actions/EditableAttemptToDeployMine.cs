using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableAttemptToDeployMine : EditableAction
{
    public override void TakeAction(AgentHandler agentScript)
    {
        GameConstants.Instance.NumberOfMines++;

        GameObject mineToDeploy = PoolSystem.Instance.GetObjectFromPool(agentScript.MinePrefab, argActivateObject:true, argShouldExpandPool: true, argShouldCreateNonExistingPool: true);
        Mine mineScript = mineToDeploy.GetComponent<Mine>();

        mineScript.SpawnMine(agentScript.transform.position);

        agentScript.SetDeployMineTimer(0.0f);
    }
}