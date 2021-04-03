using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BenAgent : MonoBehaviour
{

    private Vector3 mMovementDirection = Vector3.zero;
    [SerializeField] private float mMovementSpeed = 1f;
    [SerializeField] private float mTurnSpeed = 1f;

    [SerializeField] private GameObject mTarget = null;

    [SerializeField] private float mPerceptionAngle = 50.0f;

    [Space]

    [SerializeField] private float mSeperationDistance = 1f;
    [SerializeField] private float mSeperationWeighting = 1f;

    [Space]

    [SerializeField] private float mAlignmentDistance = 1f;
    [SerializeField] private float mAlignmentWeighting = 1f;

    [Space]

    [SerializeField] private float mCohesionDistance = 1f;
    [SerializeField] private float mCohesionWeighting = 1f;

    public bool printData = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        float rotationStep = mTurnSpeed * Time.deltaTime;
        Vector3 targetRotationVector = Vector3.RotateTowards(transform.forward, mMovementDirection, rotationStep, 0.0f);
        transform.rotation = Quaternion.LookRotation(targetRotationVector);

        transform.position += transform.forward * mMovementSpeed * Time.deltaTime;

    }

    public void SetTarget(GameObject argTarget)
    {
        mTarget = argTarget;
    }

    public void CalculateWithNeighbours(List<BenAgent> argNeighbours)
    {

        Vector3 targetSeperationDirection = Vector3.zero;
        int seperationNeighbours = 0;

        Vector3 targetAlignmentDirection = Vector3.zero;
        int alignmentNeighbours = 0;

        Vector3 targetCohesionPosition = Vector3.zero;
        int cohesionNeighbours = 0;

        foreach (BenAgent agent in argNeighbours)
        {
            if (ReferenceEquals(this, agent)) continue;

            float distanceToAgent = Vector3.Distance(transform.position, agent.transform.position);
            Vector3 dirToOther = agent.transform.position - transform.position;
            float angleToAgent = Vector3.Angle(transform.forward, dirToOther.normalized);
            Vector3 dirToCurrent = transform.position - agent.transform.position;

            if (distanceToAgent < mSeperationDistance && angleToAgent < mPerceptionAngle)
            {
                targetSeperationDirection += dirToCurrent.normalized;
                seperationNeighbours += 1;
            }

            if (distanceToAgent < mAlignmentDistance && angleToAgent < mPerceptionAngle)
            {
                targetAlignmentDirection += agent.transform.forward;
                alignmentNeighbours += 1;
            }

            if (distanceToAgent < mCohesionDistance && angleToAgent < mPerceptionAngle)
            {
                targetCohesionPosition += agent.transform.position;
                cohesionNeighbours += 1;
            }
        }

        targetSeperationDirection /= seperationNeighbours;
        targetAlignmentDirection /= alignmentNeighbours;
        //targetCohesionPosition /= cohesionNeighbours;
        Vector3 cohesionDirection = targetCohesionPosition - transform.position;


        targetSeperationDirection *= mSeperationWeighting;
        targetAlignmentDirection *= mAlignmentWeighting;
        cohesionDirection *= mCohesionWeighting;


        Vector3 targetDirection = Vector3.zero;

        // If current agent has a target
        // Add weight in its direction

        if (printData)
        {
            print(transform.forward + "     " + (mTarget.transform.position - transform.position).normalized);
            print(seperationNeighbours + "      " + alignmentNeighbours + "     " + cohesionNeighbours);
        }    

        if (seperationNeighbours != 0)
            targetDirection += targetSeperationDirection;
        else if (alignmentNeighbours != 0)
            targetDirection += targetAlignmentDirection;
        else if (cohesionNeighbours != 0)
            targetDirection += cohesionDirection;


        Vector3 leaderDirection = mTarget.transform.position - transform.position;
        if (targetDirection != Vector3.zero)
            targetDirection += leaderDirection.normalized * (targetDirection.magnitude * 0.75f);
        else
            targetDirection = leaderDirection;

        mMovementDirection = targetDirection.normalized;

    }

}
