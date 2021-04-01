using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableSelectMachineGun : EditableAction
{
    public override void TakeAction(AgentHandler agentScript)
    {
        agentScript.SelectedWeapon = WeaponSelected.MachineGun;
    }
}