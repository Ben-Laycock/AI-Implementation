using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsManager : MonoBehaviour
{
    [SerializeField] private GameObject mFlockTarget = null;
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

        SetupSpatialGrid();
        SetupDefaultFlock();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Boids boid in mBoids)
        {
            mGrid.InsertIntoGrid(boid);
        }

        mGrid.UpdateGrid();
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

    void SetupSpatialGrid()
    {
        mGrid = new SpatialGrid(mSpatialGridCellSize);
    }

    void SetupDefaultFlock()
    {
        for (int i = 0; i < mNumOfBoids; i++)
        {
            GameObject newAgent = Instantiate(mBoidPrefab, new Vector3(Random.Range(18, 22), Random.Range(18, 22), Random.Range(18, 22)), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
            newAgent.name = "Boid " + i;
            newAgent.transform.parent = mAgentContainer.transform;
            Boids agentScript = newAgent.GetComponent<Boids>();
            agentScript.SetManager(this);
            agentScript.SetShouldFlock(true);

            AddBoid(agentScript);
            agentScript.SetTarget(mFlockTarget);
        }
    }

    public void AddBoid(Boids argBoid)
    {
        if (argBoid == null) return;

        if (!mBoids.Contains(argBoid)) mBoids.Add(argBoid);
        if (argBoid.GetTarget() != mFlockTarget) argBoid.SetTarget(mFlockTarget);
    }

    public void RemoveBoid(Boids argBoid)
    {
        if (argBoid == null) return;
        if (mBoids.Contains(argBoid)) mBoids.Remove(argBoid);
    }

    public GameObject GetFlockTarget()
    {
        return mFlockTarget;
    }

    public SpatialGrid GetSpatialGrid()
    {
        return mGrid;
    }
}
