using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableDMTestGreater50 : EditableDecision
{
    public override void MakeDecision(DMTestObjectScript agentScript)
    {
        if (agentScript.mAmount >= 50)
        {
            RunChildDecision(agentScript, true);
        }
        else
        {
            RunChildDecision(agentScript, false);
        }
    }
}