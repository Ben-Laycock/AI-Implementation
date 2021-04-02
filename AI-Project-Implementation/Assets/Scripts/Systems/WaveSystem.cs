using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    [Header("Agents")]
    [SerializeField] private GameObject mEnemyDrone;

    [Header("Wave Information")]
    [SerializeField] private int mCurrentWave = 1;

    [SerializeField] private int mNumberOfWaves = 5;
    [SerializeField] private int mNumberOfEnemies = 10;

    [SerializeField] private float mWaveLength = 60.0f;
    [SerializeField] private float mWaveTimer = 0.0f;

    bool mSpawnEnemies = true;

    private void Update()
    {
        if(mSpawnEnemies)
        {
            int numberOfEnemiesToSpawn = mNumberOfEnemies + (mCurrentWave * 2);
            for(int i = 0; i < numberOfEnemiesToSpawn; i++)
            {
                SpawnEnemyDrone(new Vector3(0,0,0));
            }

            mSpawnEnemies = false;
        }

        if(mWaveTimer >= mWaveLength)
        {
            mCurrentWave++;
            mSpawnEnemies = true;
            mWaveTimer = 0.0f;
        }

        mWaveTimer += Time.deltaTime;
    }

    public void SpawnEnemyDrone(Vector3 positionToSpawn)
    {
        GameObject enemyObject = PoolSystem.Instance.GetObjectFromPool(mEnemyDrone, argShouldExpandPool:true, argShouldCreateNonExistingPool:true);
        AgentHandler agentHandler = enemyObject.GetComponent<AgentHandler>();

        agentHandler.SetupAgentForSpawning(Mathf.Clamp(mCurrentWave, 0, 3), 20.0f, positionToSpawn);
        agentHandler.SpawnAgent();
    }

}