using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boids : MonoBehaviour
{
    [SerializeField] private Boids mFlockLeader = null;

    [SerializeField] private Vector3 mVelocity = Vector3.zero;
    [SerializeField] private Vector3 mAcceleration = Vector3.zero;
    [SerializeField] private float mMaxSpeed = 1f;
    [SerializeField] private float mMaxSteering = 0.1f;

    [SerializeField] private float mSeparationDistance = 1f;
    [SerializeField] private float mAlignmentDistance = 1f;
    [SerializeField] private float mCohesionDistance = 1f;

    [SerializeField] private float mSeparationWeight = 1f;
    [SerializeField] private float mAlignmentWeight = 1f;
    [SerializeField] private float mCohesionWeight = 1f;
    [SerializeField] private float mLeaderFollowWeight = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mVelocity += mAcceleration;
        if (mVelocity.magnitude > mMaxSpeed)
        {
            mVelocity.Normalize();
            mVelocity *= mMaxSpeed;
        }

        if (mVelocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(mVelocity);

        transform.position += transform.forward * mVelocity.magnitude * Time.deltaTime;

        mAcceleration = Vector3.zero;
    }

    public void UpdateBoid(List<Boids> argNeighbours)
    {
        Vector3 separation = Separate(argNeighbours);
        Vector3 alignment = Alignment(argNeighbours);
        Vector3 cohesion = Cohesion(argNeighbours);

        separation *= mSeparationWeight;
        alignment *= mAlignmentWeight;
        cohesion *= mCohesionWeight;

        AddAccelerationForce(separation);
        AddAccelerationForce(alignment);
        AddAccelerationForce(cohesion);

        if (mFlockLeader != null)
        {
            Vector3 seekLeader = Seek(mFlockLeader.transform.position);
            seekLeader *= mLeaderFollowWeight;
            AddAccelerationForce(seekLeader);
        }
    }

    void AddAccelerationForce(Vector3 argForce)
    {
        mAcceleration += argForce;
    }

    Vector3 Separate(List<Boids> argNeighbours)
    {
        Vector3 steering = Vector3.zero;
        int separationNeighbours = 0;
        
        foreach (Boids boid in argNeighbours)
        {
            if (ReferenceEquals(this, boid)) continue; // Continue to next boid, testing against self?

            float distanceToNeighbour = Vector3.Distance(transform.position, boid.gameObject.transform.position);
            if (distanceToNeighbour > mSeparationDistance || distanceToNeighbour == 0) continue;

            Vector3 seperationDirection = transform.position - boid.gameObject.transform.position;
            seperationDirection.Normalize();
            seperationDirection /= distanceToNeighbour;

            steering += seperationDirection;
            separationNeighbours++;
        }

        if (separationNeighbours > 0) steering /= separationNeighbours;

        if (steering.magnitude > 0)
        {
            steering.Normalize();
            steering *= mMaxSpeed;
            steering = steering - mVelocity;
            if (steering.magnitude > mMaxSteering)
            {
                steering.Normalize();
                steering *= mMaxSteering;
            }
        }

        return steering;
    }

    Vector3 Alignment(List<Boids> argNeighbours)
    {
        Vector3 alignmentDirection = Vector3.zero;
        int alignmentNeighbours = 0;

        foreach (Boids boid in argNeighbours)
        {
            if (ReferenceEquals(this, boid)) continue; // Continue to next boid, testing against self?

            float distanceToNeighbour = Vector3.Distance(transform.position, boid.gameObject.transform.position);
            if (distanceToNeighbour > mAlignmentDistance || distanceToNeighbour == 0) continue;

            alignmentDirection += boid.mVelocity;
            alignmentNeighbours++;
        }

        if (alignmentNeighbours > 0)
        {
            alignmentDirection /= alignmentNeighbours;

            alignmentDirection.Normalize();
            alignmentDirection *= mMaxSpeed;
            alignmentDirection = alignmentDirection - mVelocity;
            if (alignmentDirection.magnitude > mMaxSteering)
            {
                alignmentDirection.Normalize();
                alignmentDirection *= mMaxSteering;
            }
        }

        return alignmentDirection;
    }

    Vector3 Cohesion(List<Boids> argNeighbours)
    {
        Vector3 cohesionMidPoint = Vector3.zero;
        int cohesionNeighbours = 0;

        foreach (Boids boid in argNeighbours)
        {
            if (ReferenceEquals(this, boid)) continue; // Continue to next boid, testing against self?

            float distanceToNeighbour = Vector3.Distance(transform.position, boid.gameObject.transform.position);
            if (distanceToNeighbour > mCohesionDistance || distanceToNeighbour == 0) continue;

            cohesionMidPoint += boid.gameObject.transform.position;
            cohesionNeighbours++;
        }

        if (cohesionNeighbours > 0)
        {
            cohesionMidPoint /= cohesionNeighbours;
            return Seek(cohesionMidPoint);
        }

        return Vector3.zero;
    }

    Vector3 Seek(Vector3 argTarget)
    {
        Vector3 targetDirection = argTarget - transform.position;

        targetDirection.Normalize();
        targetDirection *= mMaxSpeed;

        targetDirection = targetDirection - mVelocity;
        if (targetDirection.magnitude > mMaxSteering)
        {
            targetDirection.Normalize();
            targetDirection *= mMaxSteering;
        }

        return targetDirection;
    }

    public void SetLeader(Boids argLeader)
    {
        mFlockLeader = argLeader;
    }
}
