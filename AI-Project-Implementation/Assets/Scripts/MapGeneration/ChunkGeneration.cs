using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ChunkGeneration : MonoBehaviour
{
    //private byte[,,] mChunkData = null;
    [SerializeField] private Vector3Int mChunkPosition = Vector3Int.zero;
    private int mFaceCount = 0;

    private List<Vector3> mVertices = null;
    private List<int> mTriangles = null;

    private Mesh mMesh = null;
    private MeshCollider mCollider = null;

    // Start is called before the first frame update
    void Start()
    {
        //if (null == mChunkData)
        //{
        //    int chunkSize = VoxelManager.Instance.ChunkSize;
        //    mChunkData = new byte[chunkSize, chunkSize, chunkSize];
        //}

        //if (null == mVertices) mVertices = new List<Vector3>();
        //if (null == mTriangles) mTriangles = new List<int>();

        //mMesh = gameObject.GetComponent<MeshFilter>().mesh;
        //mCollider = gameObject.GetComponent<MeshCollider>();

        ////GenerateChunkData();
        //GenerateMeshData();
        //UpdateMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetChunkPosition(Vector3Int argChunkPosition)
    {
        mChunkPosition = argChunkPosition;
    }

    public void Generate()
    {
        if (null == mVertices) mVertices = new List<Vector3>();
        if (null == mTriangles) mTriangles = new List<int>();

        mMesh = gameObject.GetComponent<MeshFilter>().mesh;
        mCollider = gameObject.GetComponent<MeshCollider>();

        //GenerateChunkData();
        GenerateMeshData();
        UpdateMesh();
    }

    //void GenerateChunkData()
    //{
    //    int chunkSize = VoxelManager.Instance.ChunkSize;
    //    float voxelScale = VoxelManager.Instance.VoxelScale;

    //    for (int x = 0; x < chunkSize; x++)
    //    {
    //        for (int y = 0; y < chunkSize; y++)
    //        {
    //            for (int z = 0; z < chunkSize; z++)
    //            {
    //                Vector3 voxelWorldPos = transform.position + (new Vector3(x, y, z) * voxelScale);

    //                if (PerlinValueAtPoint(voxelWorldPos) > VoxelManager.Instance.PerlinThreshold)
    //                    mChunkData[x, y, z] = 1;
    //                else
    //                    mChunkData[x, y, z] = 0;
    //            }
    //        }
    //    }

    //}

    void GenerateMeshData()
    {
        int chunkSize = VoxelManager.Instance.ChunkSize;

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    Vector3Int dataPointToCheck = (mChunkPosition * chunkSize) + new Vector3Int(x, y, z);
                    if (VoxelManager.Instance.GetMapDataAt(dataPointToCheck.x, dataPointToCheck.y, dataPointToCheck.z) == 0) continue; // Air block, move to next

                    float voxelScale = VoxelManager.Instance.VoxelScale;
                    float voxelRadius = voxelScale / 2;

                    // Gets the x, y, z positions of the voxel in chunk space
                    float relativeWorldX = x * voxelScale;
                    float relativeWorldY = y * voxelScale;
                    float relativeWorldZ = z * voxelScale;

                    // Generate top face
                    if (VoxelManager.Instance.GetMapDataAt(dataPointToCheck.x, dataPointToCheck.y + 1, dataPointToCheck.z) == 0)
                        GenerateTopFace(relativeWorldX, relativeWorldY, relativeWorldZ, voxelRadius);

                    // Generate bottom face
                    if (VoxelManager.Instance.GetMapDataAt(dataPointToCheck.x, dataPointToCheck.y - 1, dataPointToCheck.z) == 0)
                        GenerateBottomFace(relativeWorldX, relativeWorldY, relativeWorldZ, voxelRadius);

                    // Generate east face
                    if (VoxelManager.Instance.GetMapDataAt(dataPointToCheck.x + 1, dataPointToCheck.y, dataPointToCheck.z) == 0)
                        GenerateEastFace(relativeWorldX, relativeWorldY, relativeWorldZ, voxelRadius);

                    // Generate west face
                    if (VoxelManager.Instance.GetMapDataAt(dataPointToCheck.x - 1, dataPointToCheck.y, dataPointToCheck.z) == 0)
                        GenerateWestFace(relativeWorldX, relativeWorldY, relativeWorldZ, voxelRadius);

                    // Generate north face
                    if (VoxelManager.Instance.GetMapDataAt(dataPointToCheck.x, dataPointToCheck.y, dataPointToCheck.z + 1) == 0)
                        GenerateNorthFace(relativeWorldX, relativeWorldY, relativeWorldZ, voxelRadius);

                    // Generate south face
                    if (VoxelManager.Instance.GetMapDataAt(dataPointToCheck.x, dataPointToCheck.y, dataPointToCheck.z - 1) == 0)
                        GenerateSouthFace(relativeWorldX, relativeWorldY, relativeWorldZ, voxelRadius);
                }
            }
        }
    }

    void UpdateMesh()
    {
        mMesh.Clear();

        mMesh.vertices = mVertices.ToArray();
        mMesh.triangles = mTriangles.ToArray();
        mMesh.RecalculateNormals();

        // Update Collider
        mCollider.sharedMesh = mMesh;

        mVertices.Clear();
        mTriangles.Clear();

        mFaceCount = 0;
    }

    void GenerateTopFace(float argX, float argY, float argZ, float argVoxelRadius)
    {
        mVertices.Add(new Vector3(argX - argVoxelRadius, argY + argVoxelRadius, argZ + argVoxelRadius));
        mVertices.Add(new Vector3(argX + argVoxelRadius, argY + argVoxelRadius, argZ + argVoxelRadius));
        mVertices.Add(new Vector3(argX + argVoxelRadius, argY + argVoxelRadius, argZ - argVoxelRadius));
        mVertices.Add(new Vector3(argX - argVoxelRadius, argY + argVoxelRadius, argZ - argVoxelRadius));
        AddFace();
    }

    void GenerateBottomFace(float argX, float argY, float argZ, float argVoxelRadius)
    {
        mVertices.Add(new Vector3(argX - argVoxelRadius, argY - argVoxelRadius, argZ - argVoxelRadius));
        mVertices.Add(new Vector3(argX + argVoxelRadius, argY - argVoxelRadius, argZ - argVoxelRadius));
        mVertices.Add(new Vector3(argX + argVoxelRadius, argY - argVoxelRadius, argZ + argVoxelRadius));
        mVertices.Add(new Vector3(argX - argVoxelRadius, argY - argVoxelRadius, argZ + argVoxelRadius));
        AddFace();
    }

    void GenerateEastFace(float argX, float argY, float argZ, float argVoxelRadius)
    {
        mVertices.Add(new Vector3(argX + argVoxelRadius, argY + argVoxelRadius, argZ - argVoxelRadius));
        mVertices.Add(new Vector3(argX + argVoxelRadius, argY + argVoxelRadius, argZ + argVoxelRadius));
        mVertices.Add(new Vector3(argX + argVoxelRadius, argY - argVoxelRadius, argZ + argVoxelRadius));
        mVertices.Add(new Vector3(argX + argVoxelRadius, argY - argVoxelRadius, argZ - argVoxelRadius));
        AddFace();
    }

    void GenerateWestFace(float argX, float argY, float argZ, float argVoxelRadius)
    {
        mVertices.Add(new Vector3(argX - argVoxelRadius, argY - argVoxelRadius, argZ + argVoxelRadius));
        mVertices.Add(new Vector3(argX - argVoxelRadius, argY + argVoxelRadius, argZ + argVoxelRadius));
        mVertices.Add(new Vector3(argX - argVoxelRadius, argY + argVoxelRadius, argZ - argVoxelRadius));
        mVertices.Add(new Vector3(argX - argVoxelRadius, argY - argVoxelRadius, argZ - argVoxelRadius));
        AddFace();
    }

    void GenerateNorthFace(float argX, float argY, float argZ, float argVoxelRadius)
    {
        mVertices.Add(new Vector3(argX + argVoxelRadius, argY - argVoxelRadius, argZ + argVoxelRadius));
        mVertices.Add(new Vector3(argX + argVoxelRadius, argY + argVoxelRadius, argZ + argVoxelRadius));
        mVertices.Add(new Vector3(argX - argVoxelRadius, argY + argVoxelRadius, argZ + argVoxelRadius));
        mVertices.Add(new Vector3(argX - argVoxelRadius, argY - argVoxelRadius, argZ + argVoxelRadius));
        AddFace();
    }

    void GenerateSouthFace(float argX, float argY, float argZ, float argVoxelRadius)
    {
        mVertices.Add(new Vector3(argX - argVoxelRadius, argY - argVoxelRadius, argZ - argVoxelRadius));
        mVertices.Add(new Vector3(argX - argVoxelRadius, argY + argVoxelRadius, argZ - argVoxelRadius));
        mVertices.Add(new Vector3(argX + argVoxelRadius, argY + argVoxelRadius, argZ - argVoxelRadius));
        mVertices.Add(new Vector3(argX + argVoxelRadius, argY - argVoxelRadius, argZ - argVoxelRadius));
        AddFace();
    }

    void AddFace()
    {
        // Create triangles for new face
        mTriangles.Add(mFaceCount * 4); //0
        mTriangles.Add(mFaceCount * 4 + 1); //1
        mTriangles.Add(mFaceCount * 4 + 2); //2
        mTriangles.Add(mFaceCount * 4); //0
        mTriangles.Add(mFaceCount * 4 + 2); //2
        mTriangles.Add(mFaceCount * 4 + 3); //3

        mFaceCount++;
    }

    //public byte GetChunkDataAt(int x, int y, int z)
    //{
    //    // Returns 0 if requested data is out of range
    //    if (x >= mChunkData.GetLength(0) || x < 0 
    //        || y >= mChunkData.GetLength(1) || y < 0 
    //        || z >= mChunkData.GetLength(2) || z < 0) return (byte)0;

    //    return mChunkData[x, y, z];
    //}

    //float PerlinValueAtPoint(Vector3 argPoint)
    //{
    //    argPoint *= VoxelManager.Instance.PerlinScale;

    //    float xy = Mathf.PerlinNoise(argPoint.x, argPoint.y);
    //    float yz = Mathf.PerlinNoise(argPoint.y, argPoint.z);
    //    float xz = Mathf.PerlinNoise(argPoint.z, argPoint.x);

    //    float yx = Mathf.PerlinNoise(argPoint.y, argPoint.x);
    //    float zy = Mathf.PerlinNoise(argPoint.z, argPoint.y);
    //    float zx = Mathf.PerlinNoise(argPoint.z, argPoint.x);

    //    float average = (xy + yz + xz + yx + zy + zx) / 6f;
    //    return average;
    //}
}
