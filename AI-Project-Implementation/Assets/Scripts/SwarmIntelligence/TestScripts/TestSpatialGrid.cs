using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpatialGridCell
{
    private List<TestBoid> mBoids = null;
    private List<TestBoid> mBoidsFromNeighbours = null;

    public TestSpatialGridCell()
    {
        mBoids = new List<TestBoid>();
        mBoidsFromNeighbours = new List<TestBoid>();
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

    public void Insert(TestBoid argBoid)
    {
        mBoids.Add(argBoid);
    }

    public void InsertFromNeighbour(TestBoid argBoid)
    {
        mBoidsFromNeighbours.Add(argBoid);
    }

    public void UpdateBoids()
    {
        List<TestBoid> allBoids = new List<TestBoid>(mBoids);
        allBoids.AddRange(mBoidsFromNeighbours);

        foreach (TestBoid agent in mBoids)
        {
            agent.UpdateBoid(allBoids);
        }

        mBoids.Clear();
        mBoidsFromNeighbours.Clear();
    }
}

public class TestSpatialGrid : MonoBehaviour
{
    private float mGridCellSize = 1f;
    private Dictionary<Vector3Int, TestSpatialGridCell> mCells = null;

    public TestSpatialGrid(float argCellSize)
    {
        mGridCellSize = argCellSize;

        mCells = new Dictionary<Vector3Int, TestSpatialGridCell>();
    }

    public void InsertIntoGrid(TestBoid argBoid)
    {
        int gridCellX = Mathf.FloorToInt(argBoid.transform.position.x / mGridCellSize);
        int gridCellY = Mathf.FloorToInt(argBoid.transform.position.y / mGridCellSize);
        int gridCellZ = Mathf.FloorToInt(argBoid.transform.position.z / mGridCellSize);

        Vector3Int gridCell = new Vector3Int(gridCellX, gridCellY, gridCellZ);

        if (!mCells.ContainsKey(gridCell)) mCells.Add(gridCell, new TestSpatialGridCell());

        mCells[gridCell].Insert(argBoid);

        InsertIntoNeighbours(gridCell, argBoid);
    }

    void InsertIntoNeighbours(Vector3Int argCurrentCell, TestBoid argBoid)
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

                    if (!mCells.ContainsKey(gridIndex)) mCells.Add((gridIndex), new TestSpatialGridCell());
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

    public void SetCellSize(int argCellSize)
    {
        mGridCellSize = argCellSize;
    }
}
