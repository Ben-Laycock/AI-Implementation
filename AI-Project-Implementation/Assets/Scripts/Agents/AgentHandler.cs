using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponSelected
{
    None = 0,
    MachineGun = 1,
    RocketLauncher = 2
}

public class AgentHandler : MonoBehaviour, IDamageable
{
    [Header("Decision Trees To Use")]
    [SerializeField] private EditableTree mBasicAgentTree;

    private FiniteStateMachine mBasicAgentFSM;

    //AI Algorithms
    private Boids mBoidController;
    public Boids BoidController
    {
        get { return mBoidController; }
    }

    [Header("Agent Values")]
    [SerializeField] private float mHealth = 5.0f;
    [SerializeField] private float mEnergy = 20.0f;

    [SerializeField] private float mMaxHealth = 5.0f;
    [SerializeField] private float mMaxEnergy = 5.0f;

    [SerializeField] private float mEnergyRegenTimer = 0.0f;
    [SerializeField] private float mTimeToRegenEnergy = 1.0f;
    public float TimeToRegenEnergy
    {
        get { return mTimeToRegenEnergy; }
        set { mTimeToRegenEnergy = value; }
    }

    [Header("Agent Attack Values")]
    [SerializeField] private float mFireProjectileTimer = 0.0f;

    [SerializeField] private float mMachineGunFireRate = 0.1f;
    public float MachineGunFireRate
    {
        get { return mMachineGunFireRate; }
    }

    [SerializeField] private float mRocketLauncherFireRate = 1.0f;
    public float RocketLauncherFireRate
    {
        get { return mRocketLauncherFireRate; }
    }

    [SerializeField] private float mMachineGunEnergyCost = 1.0f;
    [SerializeField] private float mRocketLauncherEnergyCost = 5.0f;

    [SerializeField] private float mAttackRange = 20.0f;
    [SerializeField] private float mTargetTooClose = 10.0f;

    [SerializeField] private WeaponSelected mCurrentSelectedWeapon = WeaponSelected.MachineGun;
    public WeaponSelected SelectedWeapon
    {
        get { return mCurrentSelectedWeapon; }
        set { mCurrentSelectedWeapon = value; }
    }

    [Header("Prefab Objects")]
    [SerializeField] private GameObject mEnergyBall;
    [SerializeField] private GameObject mBullet;

    public GameObject BulletPrefab
    {
        get { return mBullet; }
    }

    [SerializeField] private GameObject mRocket;
    public GameObject RocketPrefab
    {
        get { return mRocket; }
    }

    //Agent Setup
    private void Start()
    {
        mBoidController = this.gameObject.GetComponent<Boids>();
        SetupFSM();
    }

    //Run AI Agent Functionality
    private void Update()
    {
        if(mHealth <= 0.0f)
        {
            KillAgent();
        }
        else
        {
            mEnergyRegenTimer += Time.deltaTime;
            mFireProjectileTimer += Time.deltaTime;

            mBasicAgentFSM.currentState.Reason(GameConstants.Instance.PlayerObject, this);
            mBasicAgentFSM.currentState.Act(GameConstants.Instance.PlayerObject, this);
        }
    }

    public void SetupAgentForSpawning(float health, float energy, Vector3 position)
    {
        if(mBoidController == null)
            mBoidController = this.gameObject.GetComponent<Boids>();

        mBoidController.SetManager(GameConstants.Instance.BoidsManager);
        GameConstants.Instance.BoidsManager.AddBoid(mBoidController);

        mHealth = health;
        mEnergy = energy;
        
        transform.position = position;
    }

    public void SpawnAgent()
    {
        gameObject.SetActive(true);
    }

    public void KillAgent()
    {
        AudioSystem.Instance.PlaySound("Explosion", 0.08f);

        mBoidController.ResetBoid();
        mBoidController.RemoveBoidFromManager();

        for(int i = 0; i < 5; i++)
        {
            GameObject newEnergyBall = PoolSystem.Instance.GetObjectFromPool(mEnergyBall, argShouldExpandPool:true, argShouldCreateNonExistingPool:true);
            EnergyBall energyBallScript = newEnergyBall.GetComponent<EnergyBall>();
            energyBallScript.SetupEnergyBall(Random.Range(3, 7), transform.position);
            energyBallScript.ActivateEnergyBall();
        }

        gameObject.SetActive(false);
    }

    //Finite State Machine Setup
    public void SetupFSM()
    {
        FollowPathState followState = new FollowPathState();
        followState.SetAgentHandler(this);
        followState.AddTransitionToState(fsmTransition.ToChase, fsmStateID.ChasePlayer);
        followState.AddTransitionToState(fsmTransition.ToAttack, fsmStateID.AttackPlayer);

        AttackState attackState = new AttackState();
        attackState.AddTransitionToState(fsmTransition.ToLostPlayer, fsmStateID.FollowPath);
        attackState.AddTransitionToState(fsmTransition.ToChase, fsmStateID.ChasePlayer);

        ChasePlayerState chaseState = new ChasePlayerState();
        chaseState.AddTransitionToState(fsmTransition.ToLostPlayer, fsmStateID.FollowPath);
        chaseState.AddTransitionToState(fsmTransition.ToAttack, fsmStateID.AttackPlayer);

        mBasicAgentFSM = new FiniteStateMachine();
        mBasicAgentFSM.AddState(attackState);
        mBasicAgentFSM.AddState(followState);
        mBasicAgentFSM.AddState(chaseState);
    }

    public void FSMTransitionPassthrough(fsmTransition transition) { mBasicAgentFSM.PerformTransition(transition); }

    //Agent Data get functions
    public float GetHealth()
    {
        return mHealth;
    }

    public float GetEnergy()
    {
        return mEnergy;
    }

    public float GetFireprojectileTimer()
    {
        return mFireProjectileTimer;
    }

    public float GetAttackRange()
    {
        return mAttackRange;
    }

    public float GetTargetTooCloseRange()
    {
        return mTargetTooClose;
    }

    public float GetMachineGunEnergyCost()
    {
        return mMachineGunEnergyCost;
    }

    public float GetRocketLauncherEnergyCost()
    {
        return mRocketLauncherEnergyCost;
    }

    public float GetEnergyRegenTimer()
    {
        return mEnergyRegenTimer;
    }

    public EditableTree GetBasicAgentDecisionTree()
    {
        return mBasicAgentTree;
    }

    //Change functions for Agent Values
    public void ChangeAgentHealthBy(float amount)
    {
        mHealth += amount;

        mHealth = Mathf.Clamp(mHealth, 0.0f, mMaxHealth);
    }

    public void ChangeAgentEnergyBy(float amount)
    {
        mEnergy += amount;

        mEnergy = Mathf.Clamp(mEnergy, 0.0f, mMaxEnergy);
    }

    public void SetFireProjectileTimer(float value)
    {
        mFireProjectileTimer = value;
    }

    public void SetEnergyRegenTimer(float value)
    {
        mEnergyRegenTimer = value;
    }

    //Interface Implementations
    public void TakeDamage(float argAmount)
    {
        ChangeAgentHealthBy(-argAmount);
    }

}