using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneWayFinder : MonoBehaviour
{
    [SerializeField] private LineRenderer mLineDisplay = null;
    [SerializeField] private float mCollisionLineRadiusCheck = 1f;
    [SerializeField] private List<Vector3Int> mPathPoints = null;
    [SerializeField] private List<Vector3> mCondensedPath = null;
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
        if (mCondensedPath == null) mCondensedPath = new List<Vector3>();
        mCondensedPath.Clear();
        mCondensedPath = PathHelpers.CondensePathPoints(mPathPoints, 5f, mCollisionLineRadiusCheck, mPathCollisionMask);
    }

    void DrawPath()
    {
        if (mPathPoints.Count <= 0 || mLineDisplay == null) return;

        List<Vector3> renderablePath = new List<Vector3>();
        int furthestVisibleIndex = PathHelpers.FindFurthestVisiblePointIndex(mCondensedPath, transform.position, mPathCollisionMask);

        renderablePath.Add(transform.position);
        for (int i = furthestVisibleIndex; i < mCondensedPath.Count; i++)
        {
            renderablePath.Add(mCondensedPath[i]);
        }
        mLineDisplay.positionCount = renderablePath.Count;
        mLineDisplay.SetPositions(renderablePath.ToArray());
    }
}
