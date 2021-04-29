using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableSwitchAlt : EditableAction
{
    public override void TakeAction(DMTestObjectScript agentScript)
    {
        agentScript.mAltActive = !agentScript.mAltActive;
    }
}