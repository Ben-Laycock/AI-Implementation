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

    [Header("Projectile Information")]
    [SerializeField] private float mBulletSpeed = 1.0f;
    [SerializeField] private float mBulletDamage = 1.0f;
    [SerializeField] private float mRocketSpeed = 1.0f;
    [SerializeField] private float mRocketDamage = 5.0f;

    [Header("Projectile Types")]
    [SerializeField] private GameObject mBulletPrefab;
    [SerializeField] private GameObject mRocketPrefab;

    private float mMachineGunFireTimer = 0.0f;
    private float mRocketLauncherFireTimer = 0.0f;

    private void Start()
    {

        mPlayerHandler = gameObject.GetComponent<PlayerHandler>();

        PoolSystem.Instance.CreatePool(mBulletPrefab, 20, false);
        PoolSystem.Instance.CreatePool(mRocketPrefab, 10, false);

    }

    private void Update()
    {
        if(Input.GetMouseButton(0))
        {
            mMachineGunFireTimer += Time.deltaTime;
            mRocketLauncherFireTimer += Time.deltaTime;

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
        switch (mCurrentWeaponType)
        {
            case WeaponType.MachineGun:
                GameObject BulletToFire = PoolSystem.Instance.GetObjectFromPool(mBulletPrefab, argShouldExpandPool:true, argShouldCreateNonExistingPool:true);
                Projectile bulletProjectileScript = BulletToFire.GetComponent<Projectile>();

                bulletProjectileScript.SetupProjectile(damageToApply, speed, positionToFireFrom);
                bulletProjectileScript.LaunchProjectile(transform.forward);
                break;

            case WeaponType.RocketLauncher:
                GameObject RocketToFire = PoolSystem.Instance.GetObjectFromPool(mRocketPrefab, argShouldExpandPool: true, argShouldCreateNonExistingPool: true);
                Projectile rocketProjectileScript = RocketToFire.GetComponent<Projectile>();

                rocketProjectileScript.SetupProjectile(damageToApply, speed, positionToFireFrom);
                rocketProjectileScript.LaunchProjectile(transform.forward);
                break;

            default:
                Debug.Log("Incorrect Projectile Type!");
                break;
        }
    }

}