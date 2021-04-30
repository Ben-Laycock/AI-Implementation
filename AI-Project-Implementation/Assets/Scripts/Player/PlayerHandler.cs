using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour, IDamageable
{
    [Header("Player Values")]
    [SerializeField] private float mHealth = 100.0f;
    [SerializeField] private float mEnergy = 100.0f;

    [SerializeField] private float mMaxHealth = 100.0f;
    [SerializeField] private float mMaxEnergy = 100.0f;

    [SerializeField] private int mScore = 0;
    [SerializeField] private int mNumberOfRelicsCollected = 0;
    [SerializeField] private int mMaxRelicNumber = 5;

    [Header("Game Objects for Reference")]
    [SerializeField] private GameObject mPlayerUICanvas;
    [SerializeField] private GameObject mMainSystem;

    [SerializeField] private GameOverPortal mGameOverPortal;

    //Other Player Scripts
    private PlayerMovement mPlayerMovementScript;
    private PlayerParticleHandler mPlayerParticleHandlerScript;
    private PlayerUIHandler mPlayerUIHandler;
    private MainSystem mMainSystemScript;

    public MainSystem GetMainSystem()
    {
        return mMainSystemScript;
    }

    private void Start()
    {
        
        GameConstants.Instance.PlayerObject = gameObject;

        mPlayerMovementScript = GetComponent<PlayerMovement>();
        mPlayerParticleHandlerScript = GetComponent<PlayerParticleHandler>();

        mPlayerUIHandler = mPlayerUICanvas.GetComponent<PlayerUIHandler>();

        mMainSystemScript = mMainSystem.GetComponent<MainSystem>();

    }

    private void Update()
    {

        mPlayerParticleHandlerScript.ChangeBoosterParticles(mPlayerMovementScript.GetPlayerMovementSpeed(), mPlayerMovementScript.GetPlayerBoostMultiplier() * mPlayerMovementScript.GetPlayerNormalSpeed());

        if(mHealth <= 0.0f)
        {
            mMainSystemScript.SetFinalScore(mScore);
            mMainSystemScript.SetGameOver(true);
        }
    }

    public void IncreaseScore(int amount)
    {
        mScore += amount;
        mPlayerUIHandler.UpdateScoreText(mScore);
    }

    public void IncreaseCollectedRelicCount(int amount)
    {
        mNumberOfRelicsCollected += amount;
        mPlayerUIHandler.UpdateObjective(mNumberOfRelicsCollected, mMaxRelicNumber);
        if(mNumberOfRelicsCollected >= mMaxRelicNumber)
        {
            mGameOverPortal.ActivatePortal();
        }
    }

    public void ChangePlayerHealthBy(float amount)
    {
        mHealth += amount;

        mHealth = Mathf.Clamp(mHealth, 0.0f, mMaxHealth);

        mPlayerUIHandler.UpdateHealthBar(mHealth, mMaxHealth);
    }

    public void ChangePlayerEnergyBy(float amount)
    {
        mEnergy += amount;

        mEnergy = Mathf.Clamp(mEnergy, 0.0f, mMaxEnergy);

        mPlayerUIHandler.UpdateEnergyBar(mEnergy, mMaxEnergy);
    }


    //Get Functions
    public float GetEnergy()
    {
        return mEnergy;
    }

    public float GetHealth()
    {
        return mHealth;
    }

    public int GetScore()
    {
        return mScore;
    }

    public void TakeDamage(float argAmount)
    {
        ChangePlayerHealthBy(-argAmount);
    }

    public PlayerUIHandler GetPlayerUIHandler()
    {
        return mPlayerUIHandler;
    }
}