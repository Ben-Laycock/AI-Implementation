using System.Collections.Generic;
using UnityEngine;

public class PoolSystem : MonoBehaviour
{
    private static PoolSystem sInstance = null;

    // Get Singleton Instance
    public static PoolSystem Instance
    {
        get
        {
            return sInstance;
        }
    }


    private void Awake()
    {
        if (null != sInstance && this != sInstance)
            Destroy(this.gameObject);

        sInstance = this;
        DontDestroyOnLoad(this.gameObject);
    }


    [SerializeField] private Dictionary<GameObject, List<GameObject>> mPools = new Dictionary<GameObject, List<GameObject>>();
    [SerializeField] private int mDefaultNumberOfObjectsToCreate = 10;


    /*
     * Function to create a pool of GameObjects
     * 
     * argKey = Object that the pool should contain
     * argAmount = Amount of times the argKey object should be created in the pool
     */
    public void CreatePool
        (GameObject argKey,
        int argAmount,
        bool argExpandExistingPool = true)
    {                  
        // Return due to invalid pool key
        if (null == argKey)
        {
            Debug.LogWarning("Pool System: Given invalid key for pool creation.");
            return;
        }

        // Check for amount of 0 or less
        if (0 >= argAmount)
        {
            if (0 == argAmount)
                argAmount = mDefaultNumberOfObjectsToCreate;
            else
                argAmount = Mathf.Abs(argAmount);
        }

        // Check if a pool with the given key already exists
        if (mPools.ContainsKey(argKey))
        {
            if (argExpandExistingPool)
            {
                Debug.LogWarning("Pool System: Given key already exists, expanding pool.");

                // Get pool parent object
                GameObject poolParentObject = GameObject.Find("Pool_" + argKey.name);

                // Expand the pool by given number of objects
                for (int i = 0; i < argAmount; i++)
                {
                    GameObject newObject = Instantiate(argKey, poolParentObject.transform);
                    newObject.name = "PoolObject_" + argKey.name;
                    newObject.SetActive(false);
                    mPools[argKey].Add(newObject);
                }
            }
            else
            {
                Debug.LogWarning("Pool System: Given key already exists, skipping creation.");
                return;
            }
        }
        else
        {
            // Create new pool of objects
            List<GameObject> newList = new List<GameObject>();
            mPools.Add(argKey, newList);

            // Create parent object for new pool
            GameObject newPoolParentObject = new GameObject("Pool_" + argKey.name);
            newPoolParentObject.transform.parent = transform;

            // Populate new pool with specified amount of objects
            for (int i = 0; i < argAmount; i++)
            {
                GameObject newPoolObject = Instantiate(argKey, newPoolParentObject.transform);
                newPoolObject.name = "PoolObject_" + argKey.name;
                newPoolObject.SetActive(false);
                newList.Add(newPoolObject);
            }
        }
    }


    /*
     * Function to retrieve an object from a given pool
     * 
     * argKey = Object to search for in mPools
     * argActivateObject = Should the object be set to active when retrieved from the pool (False by default)
     * argIgnoreActiveCheck = If true, returns an object from the given pool even if the object is already active (False by default)
     * argShouldExpandPool = Creates new object (Same as the given key) if a valid object is not found
     */
    public GameObject GetObjectFromPool
        (GameObject argKey,
        bool argActivateObject = false,
        bool argIgnoreActiveCheck = false,
        bool argShouldExpandPool = false,
        bool argShouldCreateNonExistingPool = false)
    {
        // Check if a pool exists for the given key
        if (null == argKey)
        {
            Debug.LogWarning("Pool System: Given null key, returning null.");
            return null;
        }

        // Create new pool for invalid object key
        if (!mPools.ContainsKey(argKey))
        {
            if (!argShouldCreateNonExistingPool)
            { 
                Debug.LogWarning("Pool System: Given invalid key, returning null.");
                return null;
            }
            else
            {
                Debug.LogWarning("Pool System: Given invalid key, creating pool.");
                CreatePool(argKey, mDefaultNumberOfObjectsToCreate);
                // Pool has just been created so return object at the back of the pool
                return mPools[argKey][mPools[argKey].Count];
            }
        }

        // Check to find an inactive object
        for (int i = 0; i < mPools[argKey].Count; i++)
        {
            if (!mPools[argKey][i].activeSelf)
            {
                GameObject inactiveObject = mPools[argKey][i];
                mPools[argKey].RemoveAt(i);
                mPools[argKey].Add(inactiveObject);

                if (argActivateObject)
                    inactiveObject.SetActive(true);

                return inactiveObject;
            }
        }

        // Ignore active check
        if (argIgnoreActiveCheck)
        {
            GameObject objectToReturn = mPools[argKey][0];
            mPools[argKey].RemoveAt(0);
            mPools[argKey].Add(objectToReturn);

            if (argActivateObject)
                objectToReturn.SetActive(true);
            else
                objectToReturn.SetActive(false);

            return objectToReturn;
        }

        // Expand existing pool
        if (argShouldExpandPool)
        {
            GameObject poolParentObject = GameObject.Find("Pool_" + argKey.name);
            GameObject newObject = Instantiate(argKey, poolParentObject.transform);
            newObject.name = "PoolObject_" + argKey.name;
            mPools[argKey].Add(newObject);

            if (argActivateObject)
                newObject.SetActive(true);
            else
                newObject.SetActive(false);

            return newObject;
        }

        return null;
    }


    /*
     * Resets all pools by disabling every object ready for their next use
     */
    void ResetPools()
    {
        foreach (var KVP in mPools)
        {
            foreach (GameObject poolObject in mPools[KVP.Key])
            {
                poolObject.SetActive(false);
            }
        }
    }
}
