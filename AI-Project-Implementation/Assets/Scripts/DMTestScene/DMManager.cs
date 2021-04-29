using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMManager : MonoBehaviour
{
    [SerializeField] private GameObject mTestObjectPrefab;    
    [SerializeField] private int mNumberToSpawn = 1;
    [SerializeField] private int mNumberToSpawnInWave = 50;

    private void Start()
    {
        for(int i = 0; i < mNumberToSpawn; i++)
        {
            GameObject mSpawnedObject = Instantiate(mTestObjectPrefab);
            mSpawnedObject.transform.position = Vector3.zero;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            for (int i = 0; i < mNumberToSpawnInWave; i++)
            {
                GameObject mSpawnedObject = Instantiate(mTestObjectPrefab);
                mSpawnedObject.transform.position = Vector3.zero;
            }
        }
    }

}