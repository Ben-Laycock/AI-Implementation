using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialGridCell
{
    private List<Boids> mBoids = null;
    private List<Boids> mBoidsFromNeighbours = null;

    public SpatialGridCell()
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

public class SpatialGrid
{
    private float mGridCellSize = 1f;
    private Dictionary<Vector3Int, SpatialGridCell> mCells = null;

    public SpatialGrid(float argCellSize)
    {
        mGridCellSize = argCellSize;

        mCells = new Dictionary<Vector3Int, SpatialGridCell>();
    }

    public void InsertIntoGrid(Boids argBoid)
    {
        int gridCellX = Mathf.FloorToInt(argBoid.transform.position.x / mGridCellSize);
        int gridCellY = Mathf.FloorToInt(argBoid.transform.position.y / mGridCellSize);
        int gridCellZ = Mathf.FloorToInt(argBoid.transform.position.z / mGridCellSize);

        Vector3Int gridCell = new Vector3Int(gridCellX, gridCellY, gridCellZ);

        if (!mCells.ContainsKey(gridCell)) mCells.Add(gridCell, new SpatialGridCell());

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

                    if (!mCells.ContainsKey(gridIndex)) mCells.Add((gridIndex), new SpatialGridCell());
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