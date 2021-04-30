using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    MachineGun = 0,
    RocketLauncher = 1
}

public class PlayerWeaponHandler : MonoBehaviour
{
    private PlayerHandler mPlayerHandler;

    [Header("Player Fire Points")]
    [SerializeField] private GameObject mRightHandFirePosition;
    [SerializeField] private GameObject mLeftHandFirePosition;
    private bool mFireFromRight = true;

    [Header("Weapon Information")]
    [SerializeField] private WeaponType mCurrentWeaponType = WeaponType.MachineGun;
    [SerializeField] private float mMachineGunFireRate = 0.1f;
    [SerializeField] private float mRocketLauncherFireRate = 1.0f;
    [SerializeField] private LayerMask mEnemyShipLayer;

    [Header("Projectile Information")]
    [SerializeField] private float mBulletSpeed = 1.0f;
    [SerializeField] private float mBulletDamage = 1.0f;
    [SerializeField] private float mRocketSpeed = 1.0f;
    [SerializeField] private float mRocketDamage = 5.0f;

    [Header("Projectile Types")]
    [SerializeField] private GameObject mBulletPrefab;
    [SerializeField] private GameObject mRocketPrefab;

    private string[] mShootingSounds = new string[5] { "Shoot", "Shoot01", "Shoot02", "Shoot03", "Shoot04" };

    private float mMachineGunFireTimer = 0.0f;
    private float mRocketLauncherFireTimer = 0.0f;

    private void Start()
    {

        mPlayerHandler = gameObject.GetComponent<PlayerHandler>();

    }

    private void Update()
    {
        mMachineGunFireTimer += Time.deltaTime;
        mRocketLauncherFireTimer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if(mCurrentWeaponType == WeaponType.MachineGun)
                mCurrentWeaponType = WeaponType.RocketLauncher;
            else
                mCurrentWeaponType = WeaponType.MachineGun;

            mPlayerHandler.GetPlayerUIHandler().SwitchSelectedWeapon();
        }

        if(Input.GetMouseButton(0) && mPlayerHandler.GetMainSystem().GetCurrentGameState() == GameState.Running)
        {
            switch (mCurrentWeaponType)
            {
                case WeaponType.MachineGun:
                    if(mMachineGunFireTimer >= mMachineGunFireRate && mPlayerHandler.GetEnergy() >= 1)
                    {
                        mMachineGunFireTimer = 0.0f;
                        mPlayerHandler.ChangePlayerEnergyBy(-1);
                        FireProjectile(mBulletDamage, mBulletSpeed, WeaponType.MachineGun, (mFireFromRight) ? mRightHandFirePosition.transform.position : mLeftHandFirePosition.transform.position);
                        mFireFromRight = !mFireFromRight;
                    }
                    break;

                case WeaponType.RocketLauncher:
                    if (mRocketLauncherFireTimer >= mRocketLauncherFireRate && mPlayerHandler.GetEnergy() >= 5)
                    {
                        mRocketLauncherFireTimer = 0.0f;
                        mPlayerHandler.ChangePlayerEnergyBy(-5);
                        FireProjectile(mRocketDamage, mRocketSpeed, WeaponType.RocketLauncher, (mFireFromRight) ? mRightHandFirePosition.transform.position : mLeftHandFirePosition.transform.position);
                        mFireFromRight = !mFireFromRight;
                    }
                    break;

                default:
                    Debug.Log("Incorrect Projectile Type!");
                    break;
            }
        }
    }

    public void FireProjectile(float damageToApply, float speed, WeaponType type, Vector3 positionToFireFrom)
    {
        int randomSoundIndex = Random.Range(0, 5);
        switch (mCurrentWeaponType)
        {
            case WeaponType.MachineGun:
                GameObject BulletToFire = PoolSystem.Instance.GetObjectFromPool(mBulletPrefab, argShouldExpandPool:true, argShouldCreateNonExistingPool:true);
                Projectile bulletProjectileScript = BulletToFire.GetComponent<Projectile>();

                bulletProjectileScript.SetupProjectile(damageToApply, speed, positionToFireFrom);
                bulletProjectileScript.LaunchProjectile(transform.forward, null, false);

                AudioSystem.Instance.PlaySound(mShootingSounds[randomSoundIndex], 0.1f);
                break;

            case WeaponType.RocketLauncher:
                GameObject RocketToFire = PoolSystem.Instance.GetObjectFromPool(mRocketPrefab, argShouldExpandPool: true, argShouldCreateNonExistingPool: true);
                Projectile rocketProjectileScript = RocketToFire.GetComponent<Projectile>();

                rocketProjectileScript.SetupProjectile(damageToApply, speed, positionToFireFrom);

                RaycastHit hit;

                if (Physics.SphereCast(new Ray(transform.position, transform.forward), 5.0f, out hit, 50.0f, mEnemyShipLayer))
                {
                    rocketProjectileScript.LaunchProjectile(transform.forward, hit.collider.gameObject, true);
                }
                else
                {
                    rocketProjectileScript.LaunchProjectile(transform.forward, null, false);
                }

                AudioSystem.Instance.PlaySound(mShootingSounds[randomSoundIndex], 0.1f);
                break;

            default:
                Debug.Log("Incorrect Projectile Type!");
                break;
        }
    }

}