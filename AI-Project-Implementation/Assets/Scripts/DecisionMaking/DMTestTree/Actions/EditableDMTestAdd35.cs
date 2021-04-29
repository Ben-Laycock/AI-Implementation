using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableDMTestAdd35 : EditableAction
{
    public override void TakeAction(DMTestObjectScript agentScript)
    {
        agentScript.ChangeValueBy(35);
    }
}