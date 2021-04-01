using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock
{
    private GameObject mFlockTarget = null;
    private List<Boids> mBoids = new List<Boids>();
    private SpatialGrid mGrid = null;
    private bool mUseGrid = false;

    public Flock(float argCellSize, bool argUseGrid)
    {
        mUseGrid = argUseGrid;
        mGrid = new SpatialGrid(argCellSize);
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

        if (argBoid.GetTarget() != mFlockTarget) argBoid.SetTarget(mFlockTarget);
    }

    public void RemoveAgent(Boids argBoid)
    {
        if (argBoid == null) return;
        if (mBoids.Contains(argBoid)) mBoids.Remove(argBoid);
    }

    public void SetFlockTarget(GameObject argTarget)
    {
        if (argTarget == null) return;
        mFlockTarget = argTarget;

        foreach (Boids boid in mBoids)
        {
            boid.SetTarget(argTarget);
        }
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
