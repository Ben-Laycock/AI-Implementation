using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelManager : MonoBehaviour
{
    private static VoxelManager sInstance = null;

    // Get Singleton Instance
    public static VoxelManager Instance
    {
        get
        {
            return sInstance;
        }
    }


    private byte[,,] mMapData = null;
    [SerializeField] private Vector3Int mMapSize = Vector3Int.zero;
    public Vector3Int MapSize
    {
        get { return mMapSize; }
    }

    public byte GetMapDataAt(int x, int y, int z)
    {
        // Returns 0 if requested data is out of range
        if (x >= mMapData.GetLength(0) || x < 0
            || y >= mMapData.GetLength(1) || y < 0
            || z >= mMapData.GetLength(2) || z < 0) return (byte)0;

        return mMapData[x, y, z];
    }

    bool WithinBounds(int x, int y, int z)
    {
        if (x >= mMapData.GetLength(0) || 
            y >= mMapData.GetLength(1) || 
            z >= mMapData.GetLength(2) || 
            x < 0 || 
            y < 0 ||
            z < 0) 
            return false;

        return true;
    }

    private void Awake()
    {
        if (null != sInstance && this != sInstance)
            Destroy(this.gameObject);

        sInstance = this;
        DontDestroyOnLoad(this.gameObject);

        if (null == mMapData)
        {
            Vector3 newMapSize = mMapSize * mChunkSize;
            mMapData = new byte[(int)newMapSize.x, (int)newMapSize.y, (int)newMapSize.z];
        }

        GenerateMapData();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    List<Vector3Int> path = AStar.SearchForPath(Vector3Int.zero, new Vector3Int(90, 90, 90), ref mMapData);
        //    print(path.Count);
        //    foreach (Vector3Int point in path)
        //    {
        //        GameObject newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //        newCube.transform.position = point;
        //    }
        //}
    }

    void GenerateMapData()
    {
        Vector3Int mapSize = mMapSize * mChunkSize;
        float voxelScale = Instance.VoxelScale;

        Vector3 planetCenter = new Vector3(112, 112, 112);
        float planetRadiusSqr = 60 * 60;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int z = 0; z < mapSize.z; z++)
                {
                    Vector3 voxelWorldPos = new Vector3(x, y, z) * voxelScale;

                    float perlinVal = PerlinValueAtPoint(voxelWorldPos);
                    float distSqr = Vector3.SqrMagnitude(voxelWorldPos - planetCenter);

                    bool pointWithinPlanet = distSqr < planetRadiusSqr;
                    if (perlinVal > Instance.PerlinThresholdMinMax.x && perlinVal < Instance.PerlinThresholdMinMax.y && pointWithinPlanet)
                    {
                        if (perlinVal > 0.47f)
                            mMapData[x, y, z] = 1;
                        else
                            mMapData[x, y, z] = 2;
                    }
                    else
                        mMapData[x, y, z] = 0;
                }
            }
        }
    }

    float PerlinValueAtPoint(Vector3 argPoint)
    {
        argPoint *= Instance.PerlinScale;

        float xy = Mathf.PerlinNoise(argPoint.x, argPoint.y);
        float yz = Mathf.PerlinNoise(argPoint.y, argPoint.z);
        float xz = Mathf.PerlinNoise(argPoint.z, argPoint.x);

        float yx = Mathf.PerlinNoise(argPoint.y, argPoint.x);
        float zy = Mathf.PerlinNoise(argPoint.z, argPoint.y);
        float zx = Mathf.PerlinNoise(argPoint.z, argPoint.x);

        float average = (xy + yz + xz + yx + zy + zx) / 6f;
        return average;
    }

    public ref byte[,,] GetMapData()
    {
        return ref mMapData;
    }

    [SerializeField] private int mChunkSize = 16;
    public int ChunkSize
    {
        get { return mChunkSize; }
    }

    [SerializeField] private float mVoxelScale = 1f;
    public float VoxelScale
    {
        get { return mVoxelScale; }
    }

    [SerializeField] private Vector2 mPerlinThresholdMinMax = new Vector2(0.3f, 0.5f);
    public Vector2 PerlinThresholdMinMax
    {
        get { return mPerlinThresholdMinMax; }
    }

    [SerializeField] private float mPerlinScale = 0.1f;
    public float PerlinScale
    {
        get { return mPerlinScale; }
    }
}
