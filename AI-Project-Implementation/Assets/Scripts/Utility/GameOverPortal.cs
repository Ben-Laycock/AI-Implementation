using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class GameOverPortal : MonoBehaviour
{

    [Header("Other Systems")]
    [SerializeField] private MainSystem mMainSystem;

    [Header("Portal Visual Effect")]
    [SerializeField] private VisualEffect mVFX = null;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player")
        {
            mMainSystem.SetFinalScore(other.gameObject.GetComponent<PlayerHandler>().GetScore());
            mMainSystem.SetGameOver(true);
        }
    }

    public void ActivatePortal()
    {
        mVFX.Play();
    }

}