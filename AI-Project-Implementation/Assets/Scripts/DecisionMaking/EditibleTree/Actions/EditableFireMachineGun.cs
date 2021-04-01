using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableFireMachineGun : EditableAction
{
    public override void TakeAction(AgentHandler agentScript)
    {
        GameObject projectileToFire = PoolSystem.Instance.GetObjectFromPool(agentScript.BulletPrefab, argShouldExpandPool: true, argShouldCreateNonExistingPool: true);
        Projectile projectileScript = projectileToFire.GetComponent<Projectile>();

        projectileScript.SetupProjectile(1.0f, 15.0f, agentScript.transform.position);
        projectileScript.LaunchProjectile((GameConstants.Instance.PlayerObject.transform.position - agentScript.transform.position).normalized);

        agentScript.ChangeAgentEnergyBy(-agentScript.GetMachineGunEnergyCost());

        agentScript.SetFireProjectileTimer(0.0f);
    }
}