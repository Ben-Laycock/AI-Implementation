using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneWayFinder : MonoBehaviour
{
    [SerializeField] private LineRenderer mLineDisplay = null;
    [SerializeField] private float mCollisionLineRadiusCheck = 1f;
    [SerializeField] private List<Vector3Int> mPathPoints = null;
    [SerializeField] private LayerMask mPathCollisionMask = new LayerMask();
    private bool mShouldRenderPath = false;


    // Start is called before the first frame update
    void Start()
    {
        if (mLineDisplay == null) mLineDisplay = this.gameObject.GetComponent<LineRenderer>();
        mLineDisplay.positionCount = 0;
        mLineDisplay.numCornerVertices = 10;
        mLineDisplay.numCapVertices = 10;
    }

    // Update is called once per frame
    void Update()
    {
        if (mShouldRenderPath) DrawPath();
    }

    public void ClearPathRender()
    {
        mLineDisplay.positionCount = 0;
    }

    public void SetShouldRenderPath(bool argState)
    {
        mShouldRenderPath = argState;
    }

    public void FindPathToPoint(Vector3Int argFrom, Vector3Int argTo)
    {
        mPathPoints = AStar.SearchForPath(argFrom, argTo, ref VoxelManager.Instance.GetMapData());
    }

    void DrawPath()
    {
        if (mPathPoints.Count <= 0 || mLineDisplay == null) return;
        List<Vector3> path = CondensePathPoints();
        mLineDisplay.positionCount = path.Count;
        mLineDisplay.SetPositions(path.ToArray());
    }

    List<Vector3> CondensePathPoints()
    {
        List<Vector3> condensedPath = new List<Vector3>();

        condensedPath.Add(transform.position);

        int furthestVisiblePointIndex = FindFurthestVisiblePointIndex();
        condensedPath.Add(mPathPoints[furthestVisiblePointIndex]);
        if (furthestVisiblePointIndex == mPathPoints.Count - 1) return condensedPath;

        int currentIndex = furthestVisiblePointIndex + 1;
        while(currentIndex < mPathPoints.Count)
        {
            Vector3Int nextPoint = mPathPoints[currentIndex];
            Vector3Int prevPoint = mPathPoints[currentIndex - 1];
            if (PathBlocked(condensedPath[condensedPath.Count - 1], nextPoint)) condensedPath.Add(prevPoint);
            else currentIndex++;

            if (currentIndex == mPathPoints.Count - 1)
                condensedPath.Add(nextPoint);
        }

        return condensedPath;
    }

    int FindFurthestVisiblePointIndex()
    {
        int furthestVisibleIndex = -1;
        for (int i = 0; i < mPathPoints.Count; i++)
        {
            if (Physics.Raycast(new Ray(transform.position, (mPathPoints[i] - transform.position).normalized), (mPathPoints[i] - transform.position).magnitude, mPathCollisionMask)) continue;
            furthestVisibleIndex = i;
        }
        return furthestVisibleIndex;
    }

    bool PathBlocked(Vector3 argFrom, Vector3 argTo)
    {
        return Physics.SphereCast(new Ray(argFrom, (argTo - argFrom).normalized), mCollisionLineRadiusCheck, (argTo - argFrom).magnitude, mPathCollisionMask);
    }
}
