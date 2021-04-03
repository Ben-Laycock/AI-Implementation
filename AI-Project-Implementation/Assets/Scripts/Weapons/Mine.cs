using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour, IDamageable
{
    [SerializeField] private float mMineHealth = 1.0f;
    [SerializeField] private float mMineDamage = 5.0f;

    [SerializeField] private GameObject mExplosionVFX;

    [SerializeField] private bool mPlaySoundOnCollision = true;

    private void Update()
    {
        if(mMineHealth <= 0.0f)
        {
            KillMine();
        }
    }

    public void DecreaseHealth(float amount)
    {
        mMineHealth -= amount;
    }
    
    public void SpawnMine(Vector3 position)
    {
        transform.position = position;
    }
    
    public void KillMine()
    {
        GameConstants.Instance.NumberOfMines -= 1;

        if (mPlaySoundOnCollision)
        {
            int randomExplosion = Random.Range(0, 3);
            if (randomExplosion == 0)
                AudioSystem.Instance.PlaySound("Explosion", 0.08f);
            else if (randomExplosion == 1)
                AudioSystem.Instance.PlaySound("Explosion01", 0.08f);
            else
                AudioSystem.Instance.PlaySound("Explosion02", 0.08f);
        }

        GameObject explosion = PoolSystem.Instance.GetObjectFromPool(mExplosionVFX, argActivateObject: true, argShouldExpandPool: true, argShouldCreateNonExistingPool: true);
        explosion.transform.position = transform.position;

        DeactivateVFX explsoionVFXScript = explosion.GetComponent<DeactivateVFX>();
        explsoionVFXScript.WaitForDeactivation();

        gameObject.SetActive(false);
    }

    public void TakeDamage(float argAmount)
    {
        DecreaseHealth(argAmount);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player")
        {
            PlayerHandler mPlayerHandler = other.gameObject.GetComponent<PlayerHandler>();
            mPlayerHandler.ChangePlayerHealthBy(mMineDamage);
            KillMine();
        }
    }
}