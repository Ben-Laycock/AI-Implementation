using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBall : MonoBehaviour
{
    
    [SerializeField] private float mEnergyToProvide = 1.0f;

    public void SetupEnergyBall(float energy, Vector3 position)
    {
        mEnergyToProvide = energy;
        transform.position = position;
    }

    public void ActivateEnergyBall()
    {
        gameObject.SetActive(true);
    }

    public void DeactivateEnergyBall()
    {
        gameObject.SetActive(false);
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