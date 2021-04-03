using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    [Header("Agents")]
    [SerializeField] private GameObject mEnemyDrone;

    [Header("Player Objects")]
    [SerializeField] private GameObject mPlayerObject;
    [SerializeField] private GameObject mPlayerUICanvas;
    [SerializeField] private List<GameObject> mSpawnPoints = new List<GameObject>();
    [SerializeField] private Vector2 mSpawnDistanceIdeals = Vector2.zero;

    private PlayerHandler mPlayerHandler;
    private PlayerUIHandler mPlayerUIHandler;

    [Header("Wave Information")]
    [SerializeField] private int mCurrentWave = 1;

    [SerializeField] private int mNumberOfWaves = 5;
    [SerializeField] private int mNumberOfEnemies = 10;

    [SerializeField] private float mWaveLength = 60.0f;
    [SerializeField] private float mWaveTimer = 0.0f;

    bool mSpawnEnemies = true;

    private void Start()
    {
        mPlayerHandler = mPlayerObject.GetComponent<PlayerHandler>();
        mPlayerUIHandler = mPlayerUICanvas.GetComponent<PlayerUIHandler>();
    }

    private void Update()
    {
        mPlayerUIHandler.UpdateWaveText(mCurrentWave);

        GameObject idealSpawn = FindIdealSpawn();

        if(mSpawnEnemies)
        {
            int numberOfEnemiesToSpawn = mNumberOfEnemies + (mCurrentWave * 2);
            for(int i = 0; i < numberOfEnemiesToSpawn; i++)
            {
                if (idealSpawn != null)
                    SpawnEnemyDrone(idealSpawn.transform.position);
                else
                    SpawnEnemyDrone(new Vector3(0,0,0));
            }

            mSpawnEnemies = false;
        }

        if(mWaveTimer >= mWaveLength)
        {
            mPlayerHandler.IncreaseScore(50 * mCurrentWave);

            mCurrentWave++;
            mSpawnEnemies = true;
            mWaveTimer = 0.0f;

            if(mCurrentWave == mNumberOfWaves + 1)
            {
                //Game Over
            }
        }

        mWaveTimer += Time.deltaTime;
    }

    GameObject FindIdealSpawn()
    {
        float minSpawnDistanceSqr = mSpawnDistanceIdeals.x * mSpawnDistanceIdeals.x;
        float maxSpawnDistanceSqr = mSpawnDistanceIdeals.y * mSpawnDistanceIdeals.y;

        GameObject bestAvaliableSpawn = null;
        float closestSpawn = float.MaxValue;

        foreach (GameObject spawnPoint in mSpawnPoints)
        {
            float distanceToPlayerSqr = Vector3.SqrMagnitude(spawnPoint.transform.position - mPlayerObject.transform.position);
            if (distanceToPlayerSqr > minSpawnDistanceSqr && distanceToPlayerSqr < maxSpawnDistanceSqr)
            {
                return spawnPoint;
            }
            if (distanceToPlayerSqr > minSpawnDistanceSqr && distanceToPlayerSqr < closestSpawn)
            {
                closestSpawn = distanceToPlayerSqr;
                bestAvaliableSpawn = spawnPoint;
            }
        }
        return bestAvaliableSpawn;
    }

    public void SpawnEnemyDrone(Vector3 positionToSpawn)
    {
        GameObject enemyObject = PoolSystem.Instance.GetObjectFromPool(mEnemyDrone, argShouldExpandPool:true, argShouldCreateNonExistingPool:true);
        AgentHandler agentHandler = enemyObject.GetComponent<AgentHandler>();

        agentHandler.SetupAgentForSpawning(Mathf.Clamp(mCurrentWave, 0, 3), 20.0f, positionToSpawn);
        agentHandler.SpawnAgent();
    }

}