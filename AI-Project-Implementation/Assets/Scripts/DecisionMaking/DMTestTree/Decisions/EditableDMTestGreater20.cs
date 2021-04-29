using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableDMTestGreater20 : EditableDecision
{
    public override void MakeDecision(DMTestObjectScript agentScript)
    {
        if (agentScript.mAmount >= 20)
        {
            RunChildDecision(agentScript, true);
        }
        else
        {
            RunChildDecision(agentScript, false);
        }
    }
}