using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    [SerializeField] private GameObject mPathLine = null;
    private DroneWayFinder mWayFinder = null;
    [SerializeField] private Vector3Int mTarget = Vector3Int.zero;
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
            FindAndDrawPathFromTo(new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z), mTarget);
        }

        // Check if drone is within range of target
        HasReachedTarget();
    }

    public void FindAndDrawPathFromTo(Vector3Int argFrom, Vector3Int argTo)
    {
        mWayFinder.FindPathToPoint(argFrom, argTo);
        mWayFinder.SetShouldRenderPath(true);
    }

    public void SetTarget(Vector3Int argTarget)
    {
        mTarget = argTarget;
        mHasReachedTarget = false;
    }

    void HasReachedTarget()
    {
        if (!mHasReachedTarget)
        {
            if (Vector3.SqrMagnitude(mTarget - transform.position) < (mDroneTargetProximityRange * mDroneTargetProximityRange))
            {
                mWayFinder.SetShouldRenderPath(false);
                mWayFinder.ClearPathRender();
                mHasReachedTarget = true;
            }
        }
    }
}
