using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    public class Node
    {
        public Vector3Int mParentPosition;
        public Vector3Int mPosition;
        public float mGCost; // Cost from start point
        public float mHCost; // Distance from end point
        public float mFCost; // G + H
        public Node(Vector3Int argParentPos, Vector3Int argPos, float argGCost, float argHCost)
        {
            mParentPosition = argParentPos;
            mPosition = argPos;
            mGCost = argGCost;
            mHCost = argHCost;
            mFCost = argGCost + argHCost;
        }
    }

    public static List<Vector3Int> SearchForPath(Vector3Int argStartPos, Vector3Int argEndPos, ref byte[,,] argArray)
    {
        Dictionary<Vector3Int, Node> openNodes = new Dictionary<Vector3Int, Node>();
        Dictionary<Vector3Int, Node> closedNodes = new Dictionary<Vector3Int, Node>();

        if (argArray == null || argArray.Length == 0) return new List<Vector3Int>();

        argStartPos.x = Mathf.Clamp(argStartPos.x, 0, argArray.GetLength(0) - 1);
        argStartPos.y = Mathf.Clamp(argStartPos.y, 0, argArray.GetLength(1) - 1);
        argStartPos.z = Mathf.Clamp(argStartPos.z, 0, argArray.GetLength(2) - 1);

        argEndPos.x = Mathf.Clamp(argEndPos.x, 0, argArray.GetLength(0) - 1);
        argEndPos.y = Mathf.Clamp(argEndPos.y, 0, argArray.GetLength(1) - 1);
        argEndPos.z = Mathf.Clamp(argEndPos.z, 0, argArray.GetLength(2) - 1);

        if (argArray[argStartPos.x, argStartPos.y, argStartPos.z] != (byte)0) return new List<Vector3Int>();
        if (argArray[argEndPos.x, argEndPos.y, argEndPos.z] != (byte)0) return new List<Vector3Int>();

        openNodes.Add(argStartPos, new Node(argStartPos, argStartPos, 0f, Vector3.SqrMagnitude(argEndPos - argStartPos)));
        while (true)
        {
            Node nextBest = FindLowestFCost(openNodes);
            openNodes.Remove(nextBest.mPosition);
            closedNodes.Add(nextBest.mPosition, nextBest);

            if (nextBest.mPosition == argEndPos) break;

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        Vector3Int testingPosition = new Vector3Int(nextBest.mPosition.x + x, nextBest.mPosition.y + y, nextBest.mPosition.z + z);
                        if ((x != 0 && z != 0) || (x != 0 && y != 0) || (z != 0 && y != 0) || (x == 0 && y == 0 && z == 0)) continue;
                        if (testingPosition.x < 0 ||
                            testingPosition.y < 0 ||
                            testingPosition.z < 0 ||
                            testingPosition.x >= argArray.GetLength(0) ||
                            testingPosition.y >= argArray.GetLength(1) ||
                            testingPosition.z >= argArray.GetLength(2))
                            continue;
                        if (argArray[testingPosition.x, testingPosition.y, testingPosition.z] != (byte)0) continue;
                        if (closedNodes.ContainsKey(testingPosition)) continue;

                        float costToNeighbour = nextBest.mGCost + Vector3.SqrMagnitude(testingPosition - nextBest.mPosition);
                        if (openNodes.ContainsKey(testingPosition))
                        {
                            if (openNodes[testingPosition].mGCost <= costToNeighbour) continue;

                            openNodes[testingPosition].mGCost = costToNeighbour;
                            openNodes[testingPosition].mParentPosition = nextBest.mPosition;
                        }
                        else
                        {
                            Node neighbour = new Node(nextBest.mPosition, testingPosition, costToNeighbour, Vector3.SqrMagnitude(argEndPos - testingPosition));
                            openNodes.Add(testingPosition, neighbour);
                        }
                    }
                }
            }
        }

        // Construct path
        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int currentPos = argEndPos;

        while (currentPos != argStartPos)
        {
            path.Add(currentPos);
            currentPos = closedNodes[currentPos].mParentPosition;
        }

        path.Reverse();

        return path;
    }

    public static Node FindLowestFCost(Dictionary<Vector3Int, Node> argNodes)
    {
        Node node = null;
        float lowestF = float.MaxValue;
        foreach (var KVP in argNodes)
        {
            if (KVP.Value.mFCost > lowestF) continue;

            node = KVP.Value;
            lowestF = KVP.Value.mFCost;
        }

        return node;
    }
}
