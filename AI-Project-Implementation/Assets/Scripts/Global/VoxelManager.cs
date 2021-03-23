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

    void GenerateMapData()
    {
        Vector3Int mapSize = mMapSize * mChunkSize;
        float voxelScale = Instance.VoxelScale;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int z = 0; z < mapSize.z; z++)
                {
                    Vector3 voxelWorldPos = new Vector3(x, y, z) * voxelScale;

                    if (PerlinValueAtPoint(voxelWorldPos) > Instance.PerlinThreshold)
                        mMapData[x, y, z] = 1;
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

    [SerializeField] private float mPerlinThreshold = 0.5f;
    public float PerlinThreshold
    {
        get { return mPerlinThreshold; }
    }

    [SerializeField] private float mPerlinScale = 0.1f;
    public float PerlinScale
    {
        get { return mPerlinScale; }
    }
}
