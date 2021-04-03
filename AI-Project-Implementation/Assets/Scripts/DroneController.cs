using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    [SerializeField] private List<GameObject> mRelics = new List<GameObject>();
    [SerializeField] private GameObject mPortal = null;
    [SerializeField] private GameObject mPathLine = null;
    private DroneWayFinder mWayFinder = null;
    [SerializeField] private GameObject mTarget = null;
    [SerializeField] private float mDroneTargetProximityRange = 10f;
    [SerializeField] private bool mHasReachedTarget = false;

    // Start is called before the first frame update
    void Start()
    {
        if (mPathLine != null) mWayFinder = mPathLine.GetComponent<DroneWayFinder>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            mTarget = FindClosestRelic();
            if (mTarget != null)
            {
                mHasReachedTarget = false;
                FindAndDrawPathFromTo(new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z),
                    new Vector3Int((int)mTarget.transform.position.x, (int)mTarget.transform.position.y, (int)mTarget.transform.position.z));
            }
        }

        // Check if drone is within range of target
        HasReachedTarget();
    }

    GameObject FindClosestRelic()
    {
        GameObject closest = null;
        float closestDistanceSqrd = float.MaxValue;
        foreach (GameObject relic in mRelics)
        {
            if (relic == null) continue;
            float distanceSqrd = Vector3.SqrMagnitude(relic.transform.position - transform.position);
            if (distanceSqrd < closestDistanceSqrd)
            {
                closest = relic;
                closestDistanceSqrd = distanceSqrd;
            }
        }
        if (closest == null) closest = mPortal;
        return closest;
    }

    public void FindAndDrawPathFromTo(Vector3Int argFrom, Vector3Int argTo)
    {
        mWayFinder.FindPathToPoint(argFrom, argTo);
        mWayFinder.SetShouldRenderPath(true);
    }

    void HasReachedTarget()
    {
        if (!mHasReachedTarget)
        {
            if (mTarget == null || Vector3.SqrMagnitude(mTarget.transform.position - transform.position) < (mDroneTargetProximityRange * mDroneTargetProximityRange))
            {
                if (mTarget != null) mRelics.Remove(mTarget);
                mWayFinder.SetShouldRenderPath(false);
                mWayFinder.ClearPathRender();
                mHasReachedTarget = true;
            }
        }
    }
}
