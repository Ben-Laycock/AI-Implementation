using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableIsWeaponReadyToFire : EditableDecision
{
    public override void MakeDecision(AgentHandler agentScript)
    {

        float requiredTime = (agentScript.SelectedWeapon == WeaponSelected.MachineGun) ? agentScript.MachineGunFireRate : agentScript.RocketLauncherFireRate;

        if (agentScript.GetFireprojectileTimer() >= requiredTime)
        {
            RunChildDecision(agentScript, true);
        }
        else
        {
            RunChildDecision(agentScript, false);
        }
    }
}