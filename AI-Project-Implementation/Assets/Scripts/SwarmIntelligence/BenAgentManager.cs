using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group
{
    public BenAgent mLeader = null;
    public List<BenAgent> mAgents = new List<BenAgent>();

    public void UpdateAgents()
    {
        foreach (BenAgent agent in mAgents)
        {
            agent.CalculateWithNeighbours(mAgents);
        }
    }
}

public class BenAgentManager : MonoBehaviour
{

    [SerializeField] private GameObject mAgentPrefab = null;
    [SerializeField] private Material mLeaderMaterial = null;
    [SerializeField] private GameObject mAgentContainer = null;
    [SerializeField] private List<Group> mGroups = new List<Group>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            if (!mAgentPrefab || !mAgentContainer) return;

            GameObject newObject = new GameObject("Group " + mGroups.Count);
            newObject.transform.parent = mAgentContainer.transform;

            Group newGroup = new Group();
            mGroups.Add(newGroup);

            GameObject newLeader = Instantiate(mAgentPrefab, Vector3.zero, Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
            newLeader.name = newObject.name + " Leader";
            newLeader.transform.parent = newObject.transform;
            newLeader.GetComponent<MeshRenderer>().material = mLeaderMaterial;
            newGroup.mLeader = newLeader.GetComponent<BenAgent>();
            
            for (int i = 0; i < 100; i++)
            {
                GameObject newAgent = Instantiate(mAgentPrefab, Vector3.zero, Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
                newAgent.name = "Agent " + i;
                newAgent.transform.parent = newObject.transform;
                BenAgent agentScript = newAgent.GetComponent<BenAgent>();
                agentScript.SetTarget(newLeader);
                newGroup.mAgents.Add(agentScript);
            }
        }

        foreach (Group group in mGroups)
        {
            group.UpdateAgents();
        }
    }
}
