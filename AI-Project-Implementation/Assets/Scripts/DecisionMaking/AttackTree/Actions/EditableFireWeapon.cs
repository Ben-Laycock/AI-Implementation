using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableFireWeapon : EditableAction
{
    public override void TakeAction(AgentHandler agentScript)
    {
        GameObject projectileToFire;
        Projectile projectileScript;

        switch (agentScript.SelectedWeapon)
        {
            case WeaponSelected.MachineGun:
                projectileToFire = PoolSystem.Instance.GetObjectFromPool(agentScript.BulletPrefab, argShouldExpandPool: true, argShouldCreateNonExistingPool: true);
                projectileScript = projectileToFire.GetComponent<Projectile>();

                projectileScript.SetupProjectile(1.0f, 15.0f, agentScript.transform.position);
                projectileScript.LaunchProjectile((GameConstants.Instance.PlayerObject.transform.position - agentScript.transform.position).normalized, null, false);

                agentScript.ChangeAgentEnergyBy(-agentScript.GetMachineGunEnergyCost());
                break;

            case WeaponSelected.RocketLauncher:
                projectileToFire = PoolSystem.Instance.GetObjectFromPool(agentScript.BulletPrefab, argShouldExpandPool: true, argShouldCreateNonExistingPool: true);
                projectileScript = projectileToFire.GetComponent<Projectile>();

                projectileScript.SetupProjectile(1.0f, 15.0f, agentScript.transform.position);
                projectileScript.LaunchProjectile((GameConstants.Instance.PlayerObject.transform.position - agentScript.transform.position).normalized, GameConstants.Instance.PlayerObject, true);

                agentScript.ChangeAgentEnergyBy(-agentScript.GetRocketLauncherEnergyCost());
                break;

            default:
                break;
        }

        agentScript.SetFireProjectileTimer(0.0f);
    }
}