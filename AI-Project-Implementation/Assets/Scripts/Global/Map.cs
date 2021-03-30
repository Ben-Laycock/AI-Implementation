using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType
{
    Air,
    Wall
}

public struct Node
{
    private GameObject mCube;
    public GameObject cube
    {
        get
        {
            return mCube;
        }
        set
        {
            mCube = value;
        }
    }

    private NodeType mType;
    public NodeType type
    {
        get
        {
            return mType;
        }
        set
        {
            mType = value;
        }
    }

    private int mHeuristic;
    public int heuristic
    {
        get
        {
            return mHeuristic;
        }
        set
        {
            mHeuristic = value;
        }
    }
}

public class Map : MonoBehaviour
{
    private static Map sInstance = null;

    [SerializeField]
    private GameObject exampleAgentGO;

    [SerializeField]
    private Vector3Int mMapSize = new Vector3Int(20,20,20);
    public Vector3Int mapSize
    {
        get
        {
            return mMapSize;
        }
    }

    [SerializeField]
    private Material mAirMaterial;
    [SerializeField]
    private Material mWallMaterial;
    [SerializeField]
    private Material mVisitedMaterial;
    public Material visitedMaterial
    {
        get
        {
            return mVisitedMaterial;
        }
    }

    private Node[,,] mMapAry = null;
    public Node[,,] mapAry
    {
        get
        {
            return mMapAry;
        }
    }


    // Get Singleton Instance
    private void Start()
    {
        mMapAry = new Node[mMapSize.x, mMapSize.y, mMapSize.z];
        GenerateMap();

        exampleAgentGO.GetComponent<Agent>().SearchForPath();
        List<Vector3Int> points = exampleAgentGO.GetComponent<Agent>().GetPath();
    }

    public static Map Instance
    {
        get
        {
            return sInstance;
        }
    }

    private void Awake()
    {
        if (null != sInstance && this != sInstance)
            Destroy(this.gameObject);

        sInstance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void GenerateMap()
    {
        // Default the map array to be walls.
        for (int x = 0; x < mMapSize.x; ++x)
        {
            for (int y = 0; y < mMapSize.y; ++y)
            {
                for (int z = 0; z < mMapSize.z; ++z)
                {
                    // For each point in our map, we create a new Node.
                    Node tempNode = new Node();

                    // Set the properties of the cube primitive such as position, scale and material.
                    //tempNode.cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //tempNode.cube.transform.position = new Vector3Int(x, y, z);
                    //tempNode.cube.transform.localScale = new Vector3Int(1, 1, 1);
                    //tempNode.cube.transform.parent = null;
                    //tempNode.cube.GetComponent<MeshRenderer>().material = mAirMaterial;

                    // Set the node type to be air, so that agents can walk through these points.
                    tempNode.type = NodeType.Air;

                    if (y == 2 && z != (mMapSize.z - 1))
                    {
                        tempNode.cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        tempNode.cube.transform.position = new Vector3Int(x, y, z);
                        tempNode.cube.transform.localScale = new Vector3Int(1, 1, 1);
                        tempNode.cube.transform.parent = null;
                        tempNode.type = NodeType.Wall;
                        tempNode.cube.GetComponent<MeshRenderer>().material = mWallMaterial;
                    }
                    //if (y == 3 && z == mMapSize.z-1)
                    //{
                    //    tempNode.cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //    tempNode.cube.transform.position = new Vector3Int(x, y, z);
                    //    tempNode.cube.transform.localScale = new Vector3Int(1, 1, 1);
                    //    tempNode.cube.transform.parent = null;
                    //    tempNode.type = NodeType.Wall;
                    //    tempNode.cube.GetComponent<MeshRenderer>().material = mWallMaterial;
                    //}

                    // Set the index of [x,y,z] of mMapArray to be the tempNode.
                    mMapAry[x, y, z] = tempNode;
                }
            }
        }

        // Run perlin noise over the array.

    }
}

