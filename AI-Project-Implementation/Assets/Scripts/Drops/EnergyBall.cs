using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBall : MonoBehaviour
{
    
    [SerializeField] private float mEnergyToProvide = 1.0f;

    private Vector3 mGlideDirection = Vector3.zero;
    [SerializeField] private float mEnergyBallGlideTimer = 1000.0f;

    private Rigidbody mRigidbody;

    private void Awake()
    {
        mRigidbody = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(mEnergyBallGlideTimer < 1.0f)
        {
            mRigidbody.velocity = mGlideDirection;
            if(gameObject.transform.localScale.x < 0.3)
            {
                gameObject.transform.localScale = new Vector3(mEnergyBallGlideTimer, mEnergyBallGlideTimer, mEnergyBallGlideTimer);
            }
            mEnergyBallGlideTimer += Time.deltaTime * 0.2f;
        }
        else
        {
            mRigidbody.velocity = Vector3.zero;
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
        gameObject.SetActive(false);
        mGlideDirection = Vector3.zero;
        mEnergyBallGlideTimer = 1000.0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player")
        {
            //Give Player Energy
            PlayerHandler playerHandler = other.gameObject.GetComponent<PlayerHandler>();
            playerHandler.IncreaseScore((int)mEnergyToProvide);
            playerHandler.ChangePlayerEnergyBy(mEnergyToProvide);

            //Reset Energy Ball
            DeactivateEnergyBall();
        }
    }

}