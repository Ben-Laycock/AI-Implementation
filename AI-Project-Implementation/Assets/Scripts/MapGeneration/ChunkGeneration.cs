using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TextureData
{
    public string mName = "";
    public Vector2 mOffset = Vector2.zero;
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ChunkGeneration : MonoBehaviour
{
    //private byte[,,] mChunkData = null;
    [SerializeField] private Vector3Int mChunkPosition = Vector3Int.zero;
    [SerializeField] private Vector2 mNumTextures = Vector2.one;
    [SerializeField] private List<TextureData> mTextureData = new List<TextureData>();

    private int mFaceCount = 0;
    private List<Vector3> mVertices = null;
    private List<int> mTriangles = null;
    private List<Vector2> mUvs = null;

    private Mesh mMesh = null;
    private MeshCollider mCollider = null;


    public void SetChunkPosition(Vector3Int argChunkPosition)
    {
        mChunkPosition = argChunkPosition;
    }

    public void Generate()
    {
        if (mVertices == null) mVertices = new List<Vector3>();
        if (mTriangles == null) mTriangles = new List<int>();
        if (mUvs == null) mUvs = new List<Vector2>();

        mMesh = gameObject.GetComponent<MeshFilter>().mesh;
        mCollider = gameObject.GetComponent<MeshCollider>();

        GenerateMeshData();
        UpdateMesh();
    }

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
                    int blockType = VoxelManager.Instance.GetMapDataAt(dataPointToCheck.x, dataPointToCheck.y, dataPointToCheck.z);
                    if (blockType == 0) continue; // Air block, move to next

                    float voxelScale = VoxelManager.Instance.VoxelScale;
                    float voxelRadius = voxelScale / 2;

                    // Gets the x, y, z positions of the voxel in chunk space
                    float relativeWorldX = x * voxelScale;
                    float relativeWorldY = y * voxelScale;
                    float relativeWorldZ = z * voxelScale;

                    // Generate top face
                    if (VoxelManager.Instance.GetMapDataAt(dataPointToCheck.x, dataPointToCheck.y + 1, dataPointToCheck.z) == 0)
                        GenerateTopFace(relativeWorldX, relativeWorldY, relativeWorldZ, voxelRadius, blockType);

                    // Generate bottom face
                    if (VoxelManager.Instance.GetMapDataAt(dataPointToCheck.x, dataPointToCheck.y - 1, dataPointToCheck.z) == 0)
                        GenerateBottomFace(relativeWorldX, relativeWorldY, relativeWorldZ, voxelRadius, blockType);

                    // Generate east face
                    if (VoxelManager.Instance.GetMapDataAt(dataPointToCheck.x + 1, dataPointToCheck.y, dataPointToCheck.z) == 0)
                        GenerateEastFace(relativeWorldX, relativeWorldY, relativeWorldZ, voxelRadius, blockType);

                    // Generate west face
                    if (VoxelManager.Instance.GetMapDataAt(dataPointToCheck.x - 1, dataPointToCheck.y, dataPointToCheck.z) == 0)
                        GenerateWestFace(relativeWorldX, relativeWorldY, relativeWorldZ, voxelRadius, blockType);

                    // Generate north face
                    if (VoxelManager.Instance.GetMapDataAt(dataPointToCheck.x, dataPointToCheck.y, dataPointToCheck.z + 1) == 0)
                        GenerateNorthFace(relativeWorldX, relativeWorldY, relativeWorldZ, voxelRadius, blockType);

                    // Generate south face
                    if (VoxelManager.Instance.GetMapDataAt(dataPointToCheck.x, dataPointToCheck.y, dataPointToCheck.z - 1) == 0)
                        GenerateSouthFace(relativeWorldX, relativeWorldY, relativeWorldZ, voxelRadius, blockType);
                }
            }
        }
    }

    void UpdateMesh()
    {
        mMesh.Clear();

        mMesh.vertices = mVertices.ToArray();
        mMesh.triangles = mTriangles.ToArray();
        mMesh.uv = mUvs.ToArray();
        mMesh.RecalculateNormals();

        // Update Collider
        mCollider.sharedMesh = mMesh;

        mVertices.Clear();
        mTriangles.Clear();

        mFaceCount = 0;
    }

    void GenerateTopFace(float argX, float argY, float argZ, float argVoxelRadius, int argBlockId)
    {
        mVertices.Add(new Vector3(argX - argVoxelRadius, argY + argVoxelRadius, argZ + argVoxelRadius));
        mVertices.Add(new Vector3(argX + argVoxelRadius, argY + argVoxelRadius, argZ + argVoxelRadius));
        mVertices.Add(new Vector3(argX + argVoxelRadius, argY + argVoxelRadius, argZ - argVoxelRadius));
        mVertices.Add(new Vector3(argX - argVoxelRadius, argY + argVoxelRadius, argZ - argVoxelRadius));
        AddFace();
        AddUv(mTextureData[argBlockId - 1].mOffset);
    }

    void GenerateBottomFace(float argX, float argY, float argZ, float argVoxelRadius, int argBlockId)
    {
        mVertices.Add(new Vector3(argX - argVoxelRadius, argY - argVoxelRadius, argZ - argVoxelRadius));
        mVertices.Add(new Vector3(argX + argVoxelRadius, argY - argVoxelRadius, argZ - argVoxelRadius));
        mVertices.Add(new Vector3(argX + argVoxelRadius, argY - argVoxelRadius, argZ + argVoxelRadius));
        mVertices.Add(new Vector3(argX - argVoxelRadius, argY - argVoxelRadius, argZ + argVoxelRadius));
        AddFace();
        AddUv(mTextureData[argBlockId - 1].mOffset);
    }

    void GenerateEastFace(float argX, float argY, float argZ, float argVoxelRadius, int argBlockId)
    {
        mVertices.Add(new Vector3(argX + argVoxelRadius, argY + argVoxelRadius, argZ - argVoxelRadius));
        mVertices.Add(new Vector3(argX + argVoxelRadius, argY + argVoxelRadius, argZ + argVoxelRadius));
        mVertices.Add(new Vector3(argX + argVoxelRadius, argY - argVoxelRadius, argZ + argVoxelRadius));
        mVertices.Add(new Vector3(argX + argVoxelRadius, argY - argVoxelRadius, argZ - argVoxelRadius));
        AddFace();
        AddUv(mTextureData[argBlockId - 1].mOffset);
    }

    void GenerateWestFace(float argX, float argY, float argZ, float argVoxelRadius, int argBlockId)
    {
        mVertices.Add(new Vector3(argX - argVoxelRadius, argY - argVoxelRadius, argZ + argVoxelRadius));
        mVertices.Add(new Vector3(argX - argVoxelRadius, argY + argVoxelRadius, argZ + argVoxelRadius));
        mVertices.Add(new Vector3(argX - argVoxelRadius, argY + argVoxelRadius, argZ - argVoxelRadius));
        mVertices.Add(new Vector3(argX - argVoxelRadius, argY - argVoxelRadius, argZ - argVoxelRadius));
        AddFace();
        AddUv(mTextureData[argBlockId - 1].mOffset);
    }

    void GenerateNorthFace(float argX, float argY, float argZ, float argVoxelRadius, int argBlockId)
    {
        mVertices.Add(new Vector3(argX + argVoxelRadius, argY - argVoxelRadius, argZ + argVoxelRadius));
        mVertices.Add(new Vector3(argX + argVoxelRadius, argY + argVoxelRadius, argZ + argVoxelRadius));
        mVertices.Add(new Vector3(argX - argVoxelRadius, argY + argVoxelRadius, argZ + argVoxelRadius));
        mVertices.Add(new Vector3(argX - argVoxelRadius, argY - argVoxelRadius, argZ + argVoxelRadius));
        AddFace();
        AddUv(mTextureData[argBlockId - 1].mOffset);
    }

    void GenerateSouthFace(float argX, float argY, float argZ, float argVoxelRadius, int argBlockId)
    {
        mVertices.Add(new Vector3(argX - argVoxelRadius, argY - argVoxelRadius, argZ - argVoxelRadius));
        mVertices.Add(new Vector3(argX - argVoxelRadius, argY + argVoxelRadius, argZ - argVoxelRadius));
        mVertices.Add(new Vector3(argX + argVoxelRadius, argY + argVoxelRadius, argZ - argVoxelRadius));
        mVertices.Add(new Vector3(argX + argVoxelRadius, argY - argVoxelRadius, argZ - argVoxelRadius));
        AddFace();
        AddUv(mTextureData[argBlockId - 1].mOffset);
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

    void AddUv(Vector2 argOffset)
    {
        float textureSizeX = 1 / mNumTextures.x;
        float textureSizeY = 1 / mNumTextures.y;
        float smallOffsetX = textureSizeX / 10;
        float smallOffsetY = textureSizeY / 10;
        mUvs.Add(new Vector2(argOffset.x * textureSizeX + smallOffsetX, argOffset.y * textureSizeY + textureSizeY - smallOffsetY));
        mUvs.Add(new Vector2(argOffset.x * textureSizeX + textureSizeX - smallOffsetX, argOffset.y * textureSizeY + textureSizeY - smallOffsetY));
        mUvs.Add(new Vector2(argOffset.x * textureSizeX + textureSizeX - smallOffsetX, argOffset.y * textureSizeY + smallOffsetY));
        mUvs.Add(new Vector2(argOffset.x * textureSizeX + smallOffsetX, argOffset.y * textureSizeY + smallOffsetY));
    }
}
