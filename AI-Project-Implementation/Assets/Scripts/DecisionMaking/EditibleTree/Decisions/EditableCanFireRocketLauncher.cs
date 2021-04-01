using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableCanFireRocketLauncher : EditableDecision
{
    public override void MakeDecision(AgentHandler agentScript)
    {
        if (agentScript.SelectedWeapon == WeaponSelected.RocketLauncher && agentScript.GetFireprojectileTimer() >= agentScript.RocketLauncherFireRate)
        {
            RunChildDecision(agentScript, true);
        }
        else
        {
            RunChildDecision(agentScript, false);
        }
    }
}