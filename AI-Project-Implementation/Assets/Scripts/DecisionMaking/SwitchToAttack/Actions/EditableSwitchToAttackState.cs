using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableSwitchToAttackState : EditableAction
{
    public override void TakeAction(AgentHandler agentScript)
    {
        agentScript.FSMTransitionPassthrough(fsmTransition.ToAttack);
    }
}