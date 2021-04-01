using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableFireRocketLauncher : EditableAction
{
    public override void TakeAction(AgentHandler agentScript)
    {
        GameObject projectileToFire = PoolSystem.Instance.GetObjectFromPool(agentScript.RocketPrefab, argShouldExpandPool: true, argShouldCreateNonExistingPool: true);
        Projectile projectileScript = projectileToFire.GetComponent<Projectile>();

        projectileScript.SetupProjectile(5.0f, 12.0f, agentScript.transform.position);
        projectileScript.LaunchProjectile((GameConstants.Instance.PlayerObject.transform.position - agentScript.transform.position).normalized);

        agentScript.ChangeAgentEnergyBy(-agentScript.GetRocketLauncherEnergyCost());

        agentScript.SetFireProjectileTimer(0.0f);
    }
}