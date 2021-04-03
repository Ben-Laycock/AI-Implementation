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

    private GameObject mProjectileTarget = null;
    private bool mShouldTarget = false;

    [SerializeField] private TrailRenderer mProjectileTrail;

    private void Awake()
    {
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

            if(mShouldTarget && mProjectileTarget != null && mProjectileTarget.activeSelf)
            {
                mProjectileRigidbody.velocity = (mProjectileTarget.transform.position - transform.position).normalized * mSpeed;
            }
        }

    }

    public void SetupProjectile(float damage, float speed, Vector3 position)
    {
        mProjectileLifeTimer = 0.0f;

        mDamage = damage;
        mSpeed = speed;

        transform.position = position;

        if(mProjectileTrail != null)
        {
            mProjectileTrail.Clear();
        }

        gameObject.SetActive(true);
    }
    
    public void LaunchProjectile(Vector3 directionToLaunch, GameObject targetToFollow, bool shouldTarget)
    {
        mProjectileRigidbody.velocity = directionToLaunch * mSpeed;

        mShouldTarget = shouldTarget;

        if (shouldTarget)
        {
            mProjectileTarget = targetToFollow;
        }       
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
        mProjectileTarget = null;

        mDamage = 0.0f;
        mSpeed = 0.0f;
    }

    public float GetDamage()
    {
        return mDamage;
    }

    private void OnTriggerEnter(Collider collision)
    {
        IDamageable objectInterface = collision.gameObject.GetComponent<IDamageable>();

        if(objectInterface != null)
        {
            objectInterface.TakeDamage(mDamage);
        }
        
        KillProjectile();

    }

}