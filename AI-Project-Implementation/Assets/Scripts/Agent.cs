using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PQueue<Vector3Int>
{
    /* Create a KeyValuePair of <Vector3Int, float>, the Vector3 representing the node/point, 
    the float representing how much of a priority it has in the queue.*/
    private List<KeyValuePair<Vector3Int, float>> pointsQueue = new List<KeyValuePair<Vector3Int, float>>();

    public int Count
    {
        get
        {
            return pointsQueue.Count;
        }
    }

    public void AddToQueue(Vector3Int item, float priorityVar)
    {
        pointsQueue.Add(new KeyValuePair<Vector3Int, float>(item, priorityVar));
    }

    public Vector3Int PopFromQueue()
    {
        // Keep track of the element index with the lowest priorityValue (low is most prioritised).
        int bestIndex = 0;

        /* For all elements, if the value in the KeyValue pair of pointsQueue is less 
        than the best index, replace the bestIndex with i. */
        for (int i = 0; i < pointsQueue.Count; i++)
        {
            if (pointsQueue[i].Value < pointsQueue[bestIndex].Value)
                bestIndex = i;
        }

        // Return the point with the most priority, then remove it from the queue.
        Vector3Int prioritisedPoint = pointsQueue[bestIndex].Key;
        pointsQueue.RemoveAt(bestIndex);

        return prioritisedPoint;
    }
}

public class Agent : MonoBehaviour
{
    [SerializeField]
    private Vector3Int mStartPos;
    [SerializeField]
    private Vector3Int mDestPos;

    private Dictionary<Vector3Int, Vector3Int> positionsVisited = new Dictionary<Vector3Int, Vector3Int>();
    private Dictionary<Vector3Int, float> collectiveCost = new Dictionary<Vector3Int, float>();

    // Start is called before the first frame update,
    void Start()
    {
        Transform agentTransform = this.gameObject.GetComponent<Transform>();
        if (null == agentTransform)
            agentTransform = this.gameObject.AddComponent<Transform>();

        if (mDestPos.x < 0)
            mDestPos.x = 0;
        if (mDestPos.y < 0)
            mDestPos.y = 0;
        if (mDestPos.z < 0)
            mDestPos.z = 0;

        if (mDestPos.x > Map.Instance.mapSize.x - 1)
            mDestPos.x = Map.Instance.mapSize.x - 1;
        if (mDestPos.y > Map.Instance.mapSize.y - 1)
            mDestPos.y = Map.Instance.mapSize.y - 1;
        if (mDestPos.z > Map.Instance.mapSize.z - 1)
            mDestPos.z = Map.Instance.mapSize.z - 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SearchForPath()
    {
        PQueue<Vector3Int> queue = new PQueue<Vector3Int>();
        queue.AddToQueue(mStartPos, 0f);

        positionsVisited.Add(mStartPos, mStartPos);
        collectiveCost.Add(mStartPos, 0f);

        while (queue.Count > 0f)
        {
            Vector3Int currentPoint = queue.PopFromQueue();

            if (currentPoint == mDestPos)
                break;

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        if ((currentPoint.x + x) < 0 || (currentPoint.y + y) < 0 || (currentPoint.z + z) < 0 || (currentPoint.x + x) > Map.Instance.mapSize.x - 1 || (currentPoint.y + y) > Map.Instance.mapSize.y - 1 || (currentPoint.z + z) > Map.Instance.mapSize.z - 1 || (x == 0 && y == 0 && z == 0))
                            continue;

                        if (Map.Instance.mapAry[currentPoint.x + x, currentPoint.y + y, currentPoint.z + z].type == NodeType.Wall)
                            continue;

                        Vector3Int neighbourPoint = new Vector3Int(currentPoint.x + x, currentPoint.y + y, currentPoint.z + z);
                        float newCost = collectiveCost[currentPoint] + (currentPoint - neighbourPoint).magnitude;

                        if (!collectiveCost.ContainsKey(neighbourPoint) || newCost < collectiveCost[neighbourPoint])
                        {
                            if (collectiveCost.ContainsKey(neighbourPoint))
                            {
                                collectiveCost.Remove(neighbourPoint);
                                positionsVisited.Remove(neighbourPoint);
                            }

                            collectiveCost.Add(neighbourPoint, newCost);
                            positionsVisited.Add(neighbourPoint, currentPoint);

                            float priorityValue = newCost + GetHeuristic(neighbourPoint, mDestPos);
                            queue.AddToQueue(neighbourPoint, priorityValue);
                        }
                    }
                }
            }
        }
    }

    public List<Vector3Int> GetPath()
    {
        List<Vector3Int> agentPath = new List<Vector3Int>();
        Vector3Int currentPos = mDestPos;

        while (currentPos != mStartPos)
        {
            if (!positionsVisited.ContainsKey(currentPos))
                return new List<Vector3Int>();
                
            agentPath.Add(currentPos);
            currentPos = positionsVisited[currentPos];
        }

        foreach (Vector3Int point in agentPath)
        {
            Map.Instance.mapAry[point.x, point.y, point.z].cube.GetComponent<MeshRenderer>().material = Map.Instance.visitedMaterial;
        }

        List<Vector3Int> correctedAgentPath = new List<Vector3Int>();
        while (agentPath.Count > 0)
        {
            correctedAgentPath.Add(agentPath[agentPath.Count - 1]);
            agentPath.RemoveAt(agentPath.Count - 1);
        }

        return correctedAgentPath;
    }

    public float GetHeuristic(Vector3Int loc1, Vector3Int loc2)
    {
        return Mathf.Abs(loc1.x - loc2.x) + Mathf.Abs(loc1.y - loc2.y) + Mathf.Abs(loc1.z - loc2.z);
    }
}
