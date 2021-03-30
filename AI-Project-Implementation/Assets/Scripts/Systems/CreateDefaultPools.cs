using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateDefaultPools : MonoBehaviour
{
    [System.Serializable]
    public struct PoolToCreate
    {
        public string mPoolName;
        public GameObject mPoolPrefab;
        public int mAmountToCreate;
    }

    public PoolToCreate[] mPools;

    // Start is called before the first frame update
    void Start()
    {
        if (mPools == null) return;
        foreach (PoolToCreate pool in mPools)
        {
            PoolSystem.Instance.CreatePool(pool.mPoolPrefab, pool.mAmountToCreate, false);
        }
    }
}
