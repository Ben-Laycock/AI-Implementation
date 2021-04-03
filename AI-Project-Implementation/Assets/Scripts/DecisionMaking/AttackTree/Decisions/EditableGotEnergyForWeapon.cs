using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableGotEnergyForWeapon : EditableDecision
{
    public override void MakeDecision(AgentHandler agentScript)
    {
        
        float requiredEnergy = (agentScript.SelectedWeapon == WeaponSelected.MachineGun) ? agentScript.GetMachineGunEnergyCost() : agentScript.GetRocketLauncherEnergyCost();

        if (agentScript.GetEnergy() >= requiredEnergy)
        {
            RunChildDecision(agentScript, true);
        }
        else
        {
            RunChildDecision(agentScript, false);
        }
    }
}