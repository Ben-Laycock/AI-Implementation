using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBall : MonoBehaviour
{
    
    [SerializeField] private float mEnergyToProvide = 1.0f;

    private Vector3 mGlideDirection = Vector3.zero;
    [SerializeField] private float mEnergyBallGlideTimer = 1000.0f;

    private Rigidbody mRigidbody;

    private bool GlideTowardsPlayer = false;
    private bool CanGlideToPlayer = false;
    [SerializeField] private float mSpeedToApproachPlayer = 5.0f;
    [SerializeField] private GameObject EnergyPopVFX;

    private void Awake()
    {
        mRigidbody = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(mEnergyBallGlideTimer < 1.0f)
        {
            CanGlideToPlayer = false;
            mRigidbody.velocity = mGlideDirection;
            if(gameObject.transform.localScale.x < 0.3)
            {
                gameObject.transform.localScale = new Vector3(mEnergyBallGlideTimer, mEnergyBallGlideTimer, mEnergyBallGlideTimer);
            }
            mEnergyBallGlideTimer += Time.deltaTime * 0.2f;
        }
        else
        {
            CanGlideToPlayer = true;
            mRigidbody.velocity = Vector3.zero;
        }

        if(GlideTowardsPlayer && CanGlideToPlayer)
        {
            Vector3 dirToPlayer = GameConstants.Instance.PlayerObject.transform.position - transform.position;
            mRigidbody.velocity = dirToPlayer.normalized * mSpeedToApproachPlayer;

            mSpeedToApproachPlayer += Time.deltaTime * 3;

        }

        if ((GameConstants.Instance.PlayerObject.transform.position - transform.position).magnitude <= 2.0f)
        {
            AudioSystem.Instance.PlaySound("Pickup", 0.1f);

            GameObject EnergyPopVFXObject = PoolSystem.Instance.GetObjectFromPool(EnergyPopVFX, argActivateObject:true, argShouldExpandPool:true, argShouldCreateNonExistingPool:true);
            EnergyPopVFXObject.transform.position = transform.position;

            DeactivateVFX EnergyPopVFXObjectScript = EnergyPopVFXObject.GetComponent<DeactivateVFX>();
            EnergyPopVFXObjectScript.WaitForDeactivation();

            //Give Player Energy
            PlayerHandler playerHandler = GameConstants.Instance.PlayerObject.GetComponent<PlayerHandler>();
            playerHandler.IncreaseScore((int)mEnergyToProvide);
            playerHandler.ChangePlayerEnergyBy(mEnergyToProvide);

            //Reset Energy Ball
            DeactivateEnergyBall();
        }
    }

    public void SetupEnergyBall(float energy, Vector3 position)
    {
        mEnergyToProvide = energy;
        transform.position = position;
    }

    public void ActivateEnergyBall()
    {
        mEnergyBallGlideTimer = 0.0f;
        gameObject.transform.localScale = Vector3.zero;
        mGlideDirection = new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100));
        mGlideDirection.Normalize();

        gameObject.SetActive(true);
    }

    public void DeactivateEnergyBall()
    {
        mSpeedToApproachPlayer = 5.0f;
        mGlideDirection = Vector3.zero;
        mEnergyBallGlideTimer = 1000.0f;
        GlideTowardsPlayer = false;
        gameObject.SetActive(false); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "PlayerPickupRange")
        {

            GlideTowardsPlayer = true;
            
        }
    }

}