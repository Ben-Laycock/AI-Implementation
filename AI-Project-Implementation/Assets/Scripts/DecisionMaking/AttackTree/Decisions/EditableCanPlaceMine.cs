using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableCanPlaceMine : EditableDecision
{
    public override void MakeDecision(AgentHandler agentScript)
    {
        if (agentScript.GetDeployMineTimer() >= agentScript.GetTimeRequiredToDelopMine() && GameConstants.Instance.NumberOfMines < GameConstants.Instance.MaxNumberOfMines)
        {
            RunChildDecision(agentScript, true);
        }
        else
        {
            RunChildDecision(agentScript, false);
        }
    }
}