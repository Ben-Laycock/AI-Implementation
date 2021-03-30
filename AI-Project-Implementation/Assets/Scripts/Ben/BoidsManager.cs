using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell
{
    private List<Boids> mBoids = null;
    private List<Boids> mBoidsFromNeighbours = null;

    public GridCell()
    {
        mBoids = new List<Boids>();
        mBoidsFromNeighbours = new List<Boids>();
    }

    public void ClearData()
    {
        mBoids.Clear();
        mBoidsFromNeighbours.Clear();
    }

    public int GetNumberOfBoids()
    {
        return mBoids.Count;
    }

    public void Insert(Boids argBoid)
    {
        mBoids.Add(argBoid);
    }

    public void InsertFromNeighbour(Boids argBoid)
    {
        mBoidsFromNeighbours.Add(argBoid);
    }

    public void UpdateBoids()
    {
        List<Boids> allBoids = new List<Boids>(mBoids);
        allBoids.AddRange(mBoidsFromNeighbours);

        foreach (Boids agent in mBoids)
        {
            agent.UpdateBoid(allBoids);
        }

        mBoids.Clear();
        mBoidsFromNeighbours.Clear();
    }
}

public class Grid
{
    private float mGridCellSize = 1f;
    private Dictionary<Vector3Int, GridCell> mCells = null;

    public Grid(float argCellSize)
    {
        mGridCellSize = argCellSize;

        mCells = new Dictionary<Vector3Int, GridCell>();
    }

    public void InsertIntoGrid(Boids argBoid)
    {
        int gridCellX = Mathf.FloorToInt(argBoid.transform.position.x / mGridCellSize);
        int gridCellY = Mathf.FloorToInt(argBoid.transform.position.y / mGridCellSize);
        int gridCellZ = Mathf.FloorToInt(argBoid.transform.position.z / mGridCellSize);

        Vector3Int gridCell = new Vector3Int(gridCellX, gridCellY, gridCellZ);

        if (!mCells.ContainsKey(gridCell)) mCells.Add(gridCell, new GridCell());

        mCells[gridCell].Insert(argBoid);

        InsertIntoNeighbours(gridCell, argBoid);
    }

    void InsertIntoNeighbours(Vector3Int argCurrentCell, Boids argBoid)
    {
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                for (int z = -1; z < 2; z++)
                {
                    if (x == 0 && y == 0 && z == 0) continue;
                    int actualX = argCurrentCell.x + x;
                    int actualY = argCurrentCell.y + y;
                    int actualZ = argCurrentCell.z + z;
                    Vector3Int gridIndex = new Vector3Int(actualX, actualY, actualZ);

                    if (!mCells.ContainsKey(gridIndex)) mCells.Add((gridIndex), new GridCell());
                    mCells[gridIndex].InsertFromNeighbour(argBoid);
                }
            }
        }
    }

    public void UpdateGrid()
    {
        foreach (var KVP in mCells)
        {
            if (KVP.Value.GetNumberOfBoids() > 0) KVP.Value.UpdateBoids();
            else KVP.Value.ClearData();
        }
        mCells.Clear();
    }
}

public class Flock
{
    private Boids mLeader = null;
    private List<Boids> mBoids = new List<Boids>();
    private Grid mGrid = null;
    private bool mUseGrid = false;

    public Flock(float argCellSize, bool argUseGrid)
    {
        mUseGrid = argUseGrid;
        mGrid = new Grid(argCellSize);
    }

    public void UpdateFlock()
    {
        if (mUseGrid)
        {
            foreach (Boids boid in mBoids)
            {
                mGrid.InsertIntoGrid(boid);
            }

            mGrid.UpdateGrid();
        }
        else
        {
            foreach (Boids boid in mBoids)
            {
                boid.UpdateBoid(mBoids);
            }
        }
    }

    public void SetUseGrid(bool argState)
    {
        mUseGrid = argState;
    }

    public void AddAgent(Boids argBoid)
    {
        mBoids.Add(argBoid);

        if (argBoid.GetLeader() != mLeader) argBoid.SetLeader(mLeader);
    }

    public void RemoveAgent(Boids argBoid)
    {
        if (mBoids.Contains(argBoid)) mBoids.Remove(argBoid);
        if (mLeader != argBoid) return;

        mLeader = null;
        if (mBoids.Count > 0)
        {
            SetFlockLeader(mBoids[0]);
        }
    }

    public void SetFlockLeader(Boids argBoid)
    {
        if (argBoid == null) return;

        if (mLeader != null)
        {
            mLeader.SetLeader(null);
            mBoids.Add(mLeader);
            mLeader = null;
        }

        if (mBoids.Contains(argBoid)) mBoids.Remove(argBoid);
        argBoid.SetLeader(null);
        mLeader = argBoid;

        foreach (Boids boid in mBoids)
        {
            boid.SetLeader(argBoid);
        }
    }

    public Boids GetFlockLeader()
    {
        return mLeader;
    }
}

public class BoidsManager : MonoBehaviour
{
    [SerializeField] private GameObject mBoidPrefab = null;
    [SerializeField] private GameObject mAgentContainer = null;
    [SerializeField] private List<Flock> mFlocks = new List<Flock>();
    [SerializeField] private int mNumOfBoids = 10;
    [SerializeField] private int mNumCollisionTestDirections = 100;
    public static Vector3[] mCollisionTestDirections;

    [SerializeField] private float mDefaultCellSize = 1f;
    [SerializeField] private bool mUseGridByDefault = false;

    // Start is called before the first frame update
    void Start()
    {
        // Creates a sphere of points that will be used by the boids to avoid collisions
        SetupCollisionTestDirections();
    }

    void SetupCollisionTestDirections()
    {
        mCollisionTestDirections = new Vector3[mNumCollisionTestDirections];

        float angle = Mathf.PI * (3f - Mathf.Sqrt(5f));
        for (int i = 0; i < mNumCollisionTestDirections; i++)
        {
            float y = 1f - (i / (float)(mNumCollisionTestDirections - 1f)) * 2f;
            float radius = Mathf.Sqrt(1 - y * y);

            float theta = angle * i;

            float x = Mathf.Cos(theta) * radius;
            float z = Mathf.Sin(theta) * radius;

            mCollisionTestDirections[i] = new Vector3(x, z, y);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!mBoidPrefab || !mAgentContainer) return;

            GameObject newObject = new GameObject("Flock " + mFlocks.Count);
            newObject.transform.parent = mAgentContainer.transform;

            Flock newFlock = new Flock(mDefaultCellSize, mUseGridByDefault);
            mFlocks.Add(newFlock);

            for (int i = 0; i < mNumOfBoids; i++)
            {
                GameObject newAgent = Instantiate(mBoidPrefab, new Vector3(Random.Range(18, 22), Random.Range(18, 22), Random.Range(18, 22)), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
                newAgent.name = "Boid " + i;
                newAgent.transform.parent = newObject.transform;
                Boids agentScript = newAgent.GetComponent<Boids>();
                agentScript.SetFlock(newFlock);

                if (i == 0)
                {
                    newAgent.name = "Flock Leader";
                    newFlock.SetFlockLeader(agentScript);
                }
                else
                {
                    newFlock.AddAgent(agentScript);
                    agentScript.SetLeader(newFlock.GetFlockLeader());
                }
            }
        }

        foreach (Flock f in mFlocks)
        {
            f.UpdateFlock();
        }
    }
}
