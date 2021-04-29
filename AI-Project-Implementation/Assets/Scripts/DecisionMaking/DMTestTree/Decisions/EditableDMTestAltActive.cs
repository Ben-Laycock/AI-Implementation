using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableDMTestAltActive : EditableDecision
{
    public override void MakeDecision(DMTestObjectScript agentScript)
    {
        if (agentScript.mAltActive)
        {
            RunChildDecision(agentScript, true);
        }
        else
        {
            RunChildDecision(agentScript, false);
        }
    }
}