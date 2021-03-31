using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Projectile : MonoBehaviour
{
    
    [SerializeField] private float mDamage = 1.0f;
    [SerializeField] private float mSpeed = 1.0f;

    [SerializeField] private GameObject mProjectileExplosionEffect;

    private float mProjectileLifeTimer = 0.0f;

    private Rigidbody mProjectileRigidbody;

    private void Awake()
    {
        PoolSystem.Instance.CreatePool(mProjectileExplosionEffect, 20, false);

        mProjectileRigidbody = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        
        if(gameObject.activeSelf)
        {
            mProjectileLifeTimer += Time.deltaTime;

            if(mProjectileLifeTimer >= 5.0f)
            {
                KillProjectile();
            }
        }

    }

    public void SetupProjectile(float damage, float speed, Vector3 position)
    {
        mProjectileLifeTimer = 0.0f;

        mDamage = damage;
        mSpeed = speed;

        transform.position = position;

        gameObject.SetActive(true);
    }
    
    public void LaunchProjectile(Vector3 directionToLaunch)
    {
        mProjectileRigidbody.velocity = directionToLaunch * mSpeed;
    }

    public void KillProjectile()
    {
        GameObject explosion = PoolSystem.Instance.GetObjectFromPool(mProjectileExplosionEffect, argActivateObject:true , argShouldExpandPool:true, argShouldCreateNonExistingPool:true);
        explosion.transform.position = transform.position;

        DeactivateVFX explsoionVFXScript = explosion.GetComponent<DeactivateVFX>();
        explsoionVFXScript.WaitForDeactivation();

        gameObject.SetActive(false);

        mProjectileRigidbody.velocity = Vector3.zero;

        transform.position = Vector3.zero;

        mDamage = 0.0f;
        mSpeed = 0.0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        KillProjectile();
    }

}