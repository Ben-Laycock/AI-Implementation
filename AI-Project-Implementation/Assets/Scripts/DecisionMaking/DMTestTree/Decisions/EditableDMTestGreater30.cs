using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableDMTestGreater30 : EditableDecision
{
    public override void MakeDecision(DMTestObjectScript agentScript)
    {
        if (agentScript.mAmount >= 30)
        {
            RunChildDecision(agentScript, true);
        }
        else
        {
            RunChildDecision(agentScript, false);
        }
    }
}