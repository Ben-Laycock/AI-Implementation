using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject mChunkPrefab = null;

    // Start is called before the first frame update
    void Start()
    {
        if (null == mChunkPrefab) return;

        Vector3Int mapSize = VoxelManager.Instance.MapSize;
        int chunkSize = VoxelManager.Instance.ChunkSize;
        float voxelSize = VoxelManager.Instance.VoxelScale;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int z = 0; z < mapSize.z; z++)
                {
                    Vector3 chunkPosition = new Vector3(x, y, z);

                    GameObject newChunk = Instantiate(mChunkPrefab, chunkPosition * chunkSize * voxelSize, Quaternion.identity);
                    ChunkGeneration chunkGen = newChunk.GetComponent<ChunkGeneration>();

                    chunkGen.SetChunkPosition(new Vector3Int(x, y, z));
                    chunkGen.Generate();
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
