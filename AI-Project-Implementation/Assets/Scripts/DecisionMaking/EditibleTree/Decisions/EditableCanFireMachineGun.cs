using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableCanFireMachineGun : EditableDecision
{
    public override void MakeDecision(AgentHandler agentScript)
    {
        if (agentScript.SelectedWeapon == WeaponSelected.MachineGun && agentScript.GetFireprojectileTimer() >= agentScript.MachineGunFireRate)
        {
            RunChildDecision(agentScript, true);
        }
        else
        {
            RunChildDecision(agentScript, false);
        }
    }
}