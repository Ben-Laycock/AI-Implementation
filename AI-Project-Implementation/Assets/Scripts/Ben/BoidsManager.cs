using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsManager : MonoBehaviour
{
    [SerializeField] private GameObject mBoidPrefab = null;
    [SerializeField] private GameObject mAgentContainer = null;
    private List<Boids> mBoids = null;
    private SpatialGrid mGrid = null;

    [Space][Header("Defaults")]
    [SerializeField] private int mNumOfBoids = 10;
    [SerializeField] private float mSpatialGridCellSize = 1f;

    [SerializeField] private int mNumCollisionTestDirections = 100;
    public static Vector3[] mCollisionTestDirections;

    // Start is called before the first frame update
    void Start()
    {
        // Creates a sphere of points that will be used by the boids to avoid collisions
        CalculateCollisionTestDirections();

        if (mBoids == null) mBoids = new List<Boids>();
    }

    void CalculateCollisionTestDirections()
    {
        mCollisionTestDirections = new Vector3[mNumCollisionTestDirections];

        float angle = Mathf.PI * (3f - Mathf.Sqrt(5f));
        for (int i = 0; i < mNumCollisionTestDirections; i++)
        {
            float z = 1f - (i / (float)(mNumCollisionTestDirections - 1f)) * 2f;

            float radius = Mathf.Sqrt(1 - z * z);
            float theta = angle * i;

            float x = Mathf.Cos(theta) * radius;
            float y = Mathf.Sin(theta) * radius;

            mCollisionTestDirections[i] = new Vector3(x, y, z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    if (!mBoidPrefab || !mAgentContainer) return;

        //    int flockID = mFlocks.Count;
        //    GameObject newObject = new GameObject("Flock " + flockID);
        //    newObject.transform.parent = mAgentContainer.transform;

        //    Flock newFlock = new Flock(mDefaultCellSize, mUseGridByDefault);
        //    mFlocks.Add(newFlock);

        //    GameObject newFlockTarget = new GameObject("Flock " + flockID + " Target");
        //    newFlockTarget.transform.parent = newObject.transform;
        //    newFlock.SetFlockTarget(newFlockTarget);

        //    for (int i = 0; i < mNumOfBoids; i++)
        //    {
        //        GameObject newAgent = Instantiate(mBoidPrefab, new Vector3(Random.Range(18, 22), Random.Range(18, 22), Random.Range(18, 22)), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        //        newAgent.name = "Boid " + i;
        //        newAgent.transform.parent = newObject.transform;
        //        Boids agentScript = newAgent.GetComponent<Boids>();
        //        agentScript.SetFlock(newFlock);

        //        newFlock.AddAgent(agentScript);
        //        agentScript.SetTarget(newFlockTarget);
        //    }
        //}

        //foreach (Flock f in mFlocks)
        //{
        //    f.UpdateFlock();
        //}
    }
}
