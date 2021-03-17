using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock
{
    public Boids mLeader = null;
    public List<Boids> mBoids = new List<Boids>();

    public void UpdateFlock()
    {
        foreach (Boids boid in mBoids)
        {
            boid.UpdateBoid(mBoids);
        }
    }
}

public class BoidsManager : MonoBehaviour
{
    [SerializeField] private GameObject mBoidPrefab = null;
    [SerializeField] private Material mLeaderMaterial = null;
    [SerializeField] private GameObject mAgentContainer = null;
    [SerializeField] private List<Flock> mFlocks = new List<Flock>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (!mBoidPrefab || !mAgentContainer) return;

            GameObject newObject = new GameObject("Flock " + mFlocks.Count);
            newObject.transform.parent = mAgentContainer.transform;

            Flock newFlock = new Flock();
            mFlocks.Add(newFlock);

            // Leader of the flock
            GameObject flockLeader = Instantiate(mBoidPrefab, new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10)), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
            flockLeader.name = "Flock Leader";
            flockLeader.transform.parent = newObject.transform;
            Boids leaderAgentScript = flockLeader.GetComponent<Boids>();
            newFlock.mLeader = leaderAgentScript;

            for (int i = 0; i < 100; i++)
            {
                GameObject newAgent = Instantiate(mBoidPrefab, new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10)), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
                newAgent.name = "Boid " + i;
                newAgent.transform.parent = newObject.transform;
                Boids agentScript = newAgent.GetComponent<Boids>();
                newFlock.mBoids.Add(agentScript);

                agentScript.SetLeader(newFlock.mLeader);
            }
        }

        foreach (Flock f in mFlocks)
        {
            f.UpdateFlock();
        }
    }
}
