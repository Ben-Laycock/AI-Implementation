using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingData : MonoBehaviour
{
    private static PathFindingData sInstance = null;

    // Get Singleton Instance
    public static PathFindingData Instance
    {
        get { return sInstance; }
    }

    private void Awake()
    {
        if (null != sInstance && this != sInstance) Destroy(this.gameObject);

        sInstance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        mTimeOfLastDataWipe = Time.time;
    }

    [SerializeField] private float mTimeBetweenDataWipe = 20f;
    [SerializeField] private float mPathDataCellSize = 10f;
    [SerializeField] private Dictionary<GameObject, Dictionary<Vector3Int, List<Vector3Int>>> mPathData = new Dictionary<GameObject, Dictionary<Vector3Int, List<Vector3Int>>>();
    private float mTimeOfLastDataWipe = 0f;

    public List<Vector3Int> GetPath(GameObject argTarget, Vector3 argPosition)
    {
        // Too much time has passed, data could be innacurate
        // wipe data
        if (Time.time > mTimeOfLastDataWipe + mTimeBetweenDataWipe)
        {
            ClearData();
            mTimeOfLastDataWipe = Time.time;
        }

        int gridCellX = Mathf.FloorToInt(argPosition.x / mPathDataCellSize);
        int gridCellY = Mathf.FloorToInt(argPosition.y / mPathDataCellSize);
        int gridCellZ = Mathf.FloorToInt(argPosition.z / mPathDataCellSize);
        Vector3Int gridIndex = new Vector3Int(gridCellX, gridCellY, gridCellZ);

        // Dictionary doesn't contain target object or Second dictionary doesnt contain grid idex
        if (!mPathData.ContainsKey(argTarget) || !mPathData[argTarget].ContainsKey(gridIndex))
        {
            Vector3Int startPos = new Vector3Int((int)argPosition.x, (int)argPosition.y, (int)argPosition.z);
            
            Vector3 targetPosition = argTarget.transform.position;
            Vector3Int endPos = new Vector3Int((int)targetPosition.x, (int)targetPosition.y, (int)targetPosition.z);

            GeneratePath(argTarget, gridIndex, startPos, endPos);
        }

        return mPathData[argTarget][gridIndex];
    }

    public void GeneratePath(GameObject argTarget, Vector3Int argGridIndex, Vector3Int argStartPos, Vector3Int argEndPos)
    {
        Debug.Log("Generated new path");
        List<Vector3Int> path = AStar.SearchForPath(argStartPos, argEndPos, ref VoxelManager.Instance.GetMapData());

        if (!mPathData.ContainsKey(argTarget)) mPathData.Add(argTarget, new Dictionary<Vector3Int, List<Vector3Int>>());
        if (!mPathData[argTarget].ContainsKey(argGridIndex)) mPathData[argTarget].Add(argGridIndex, path);
    }

    void ClearData()
    {
        mPathData.Clear();
    }
}
