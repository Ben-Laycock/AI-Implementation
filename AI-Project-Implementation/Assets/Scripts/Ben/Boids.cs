using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boids : MonoBehaviour
{
    [SerializeField] private Vector3 mTarget = Vector3.zero;
    [SerializeField] private LayerMask mCollisionDetectionMask = new LayerMask();
    [SerializeField] private float mCollisionRadiusCheck = 1f;
    [SerializeField] private int mMaxNeighbours = 10;

    private Vector3 mVelocity = Vector3.zero;
    private Vector3 mAcceleration = Vector3.zero;

    [Space]
    [Header("Movement")]
    [SerializeField] private Vector2 mMinMaxSpeed = new Vector2(2.5f, 5f);
    [SerializeField] private float mMaxSteering = 0.1f;
    [SerializeField] private bool mUseSmoothRotation = false;

    [Space]
    [Header("View Distances")]
    [SerializeField] private float mSeparationDistance = 1f;
    [SerializeField] private float mAlignmentDistance = 1f;
    [SerializeField] private float mCohesionDistance = 1f;
    [SerializeField] private float mObstacleAvoidanceDistance = 1f;

    [Space]
    [Header("Weights")]
    [SerializeField] private float mSeparationWeight = 1f;
    [SerializeField] private float mAlignmentWeight = 1f;
    [SerializeField] private float mCohesionWeight = 1f;
    [SerializeField] private float mTargetFollowWeight = 1f;
    [SerializeField] private float mObstacleAvoidanceWeight = 1f;
    
    [Space]
    private BoidsManager mManager = null;
    private bool mShouldFlock = false;


    public void RemoveBoidFromManager()
    {
        if (mManager != null) mManager.RemoveBoid(this);
    }    

    public void ResetBoid()
    {
        mVelocity = Vector3.zero;
        mAcceleration = Vector3.zero;
    }

    public void SetPosition(Vector3 argPosition)
    {
        transform.position = argPosition;
    }

    public void UpdateBoid(List<Boids> argNeighbours)
    {
        // Calculate and apply separation force from neighbours
        Separate(argNeighbours);

        // Should this boid use flocking behaviour?
        if (mShouldFlock)
        {
            // If target is not null and is visible then move towards the target
            if (mTarget != null)
                if (CanSee(mTarget, mObstacleAvoidanceDistance)) MoveToTarget();

            // Calculate and apply alignment force with neighbours
            Alignment(argNeighbours);
            // Calculate and apply cohesion force with neighbours
            Cohesion(argNeighbours);
        }

        // Check for collision on current path
        if (CollisionAhead())
        {
            // Find an unobstructed direction and calculate a force towards it
            Vector3 obstacleAvoidance = SteerTo(ObstacleAvoidanceDirection()) * mObstacleAvoidanceWeight;
            AddAccelerationForce(obstacleAvoidance);
        }

        // Uses the forces calculated to move and rotate
        CalculateMovementAndRotation();
    }

    void CalculateMovementAndRotation()
    {
        // Calculate velocity
        mVelocity += mAcceleration * Time.deltaTime;
        // Limit velocity to min / max speed
        float speed = mVelocity.magnitude;
        speed = Mathf.Clamp(speed, mMinMaxSpeed.x, mMinMaxSpeed.y);
        mVelocity = mVelocity.normalized * speed;

        // Move boid
        transform.position += mVelocity * Time.deltaTime;

        // Rotate boid if moving
        if (mVelocity.normalized != Vector3.zero)
        {
            // Does boid use smooth rotation
            if (mUseSmoothRotation)
            {
                // Find point of interest for boid to look at in the direction of its velocity
                Vector3 pointOfInterest = transform.position + mVelocity;
                // If the boids view is unobstrcted then rotate to velocity direction
                if (CanSee(pointOfInterest, mObstacleAvoidanceDistance))
                    transform.forward = mVelocity.normalized;
            }
            else transform.forward = mVelocity.normalized;
        }

        // Reset acceleration
        mAcceleration = Vector3.zero;
    }

    void AddAccelerationForce(Vector3 argForce)
    {
        mAcceleration += argForce;
    }

    Vector3 SteerTo(Vector3 argVector)
    {
        // Craig Reynolds - https://www.red3d.com/cwr/steer/gdc99/   (desired_velocity - velocity)
        Vector3 result = argVector.normalized * mMinMaxSpeed.y - mVelocity;
        return Vector3.ClampMagnitude(result, mMaxSteering);
    }

    bool CanSee(Vector3 argTargetPosition, float argRange)
    {
        Vector3 directionToTarget = argTargetPosition - transform.position;
        //if (Physics.SphereCast(new Ray(transform.position, directionToTarget.normalized), mCollisionRadiusCheck, directionToTarget.magnitude, mCollisionDetectionMask))
        if (Physics.Raycast(new Ray(transform.position, directionToTarget.normalized), argRange, mCollisionDetectionMask))
        {
            Debug.DrawRay(transform.position, directionToTarget.normalized * argRange, Color.red, 0.1f);
            return false;
        }
        Debug.DrawRay(transform.position, directionToTarget.normalized * argRange, Color.green, 0.1f);
        return true;
    }

    bool CollisionAhead()
    {
        // Sphere cast forward from the boids to check for collisions
        if (Physics.SphereCast(new Ray(transform.position, transform.forward), mCollisionRadiusCheck, mObstacleAvoidanceDistance, mCollisionDetectionMask)) return true;
        return false;
    }

    Vector3 ObstacleAvoidanceDirection()
    {
        // Loop through all collision avoidance directions to find an unobstructed direction
        int numberOfPaths = BoidsManager.mCollisionTestDirections.Length;
        for (int i = 0; i < numberOfPaths; i++)
        {
            // Get test direction and convert it to world space relative to the boid
            Vector3 path = transform.TransformDirection(BoidsManager.mCollisionTestDirections[i]);
            // Check for collisions in the test direction, if non then return this as the new path
            if (!Physics.SphereCast(new Ray(transform.position, path), mCollisionRadiusCheck, mObstacleAvoidanceDistance, mCollisionDetectionMask)) return path;   
        }

        // No clear path found, continue forward
        return transform.forward;
    }

    void Separate(List<Boids> argNeighbours)
    {
        Vector3 averageAvoidanceDirection = Vector3.zero;
        int separationNeighbours = 0;

        // Loop through all neighbouring boids
        foreach (Boids other in argNeighbours)
        {
            if (ReferenceEquals(this, other)) continue; // Continue to next boid, testing against self?

            float distanceToNeighbour = Vector3.Distance(transform.position, other.transform.position);
            // Move to next neighbour if current is too far away or in the same position as this boid
            if (distanceToNeighbour > mSeparationDistance || distanceToNeighbour <= 0) continue;

            // Get direction to seperate from neighbour
            Vector3 separationDirection = transform.position - other.transform.position;
            // Weight separation direction by distance
            separationDirection /= distanceToNeighbour;

            // Add separation direction to average direction
            averageAvoidanceDirection += separationDirection;
            // Increase neighbour count
            separationNeighbours++;

            // Break from loop if enough neighbours have been considered
            if (separationNeighbours >= mMaxNeighbours) break;
        }

        // Calculate and apply separation force
        Vector3 separation = SteerTo(averageAvoidanceDirection) * mSeparationWeight;
        AddAccelerationForce(separation);
    }

    void Alignment(List<Boids> argNeighbours)
    {
        Vector3 averageFlockDirection = Vector3.zero;
        int alignmentNeighbours = 0;

        // Loop through all neighbouring boids
        foreach (Boids other in argNeighbours)
        {
            if (!other.GetShouldFlock()) continue; // Continue to next, current isnt flocking
            if (ReferenceEquals(this, other)) continue; // Continue to next, testing against self?

            float distanceToNeighbour = Vector3.Distance(transform.position, other.transform.position);
            // Move to next neighbour if current is too far away or in the same position as this boid
            if (distanceToNeighbour > mAlignmentDistance || distanceToNeighbour <= 0) continue;

            // Add other boids velocity to the average flock firection
            averageFlockDirection += other.mVelocity;
            // Increase neighbour count
            alignmentNeighbours++;

            // Break from loop if enough neighbours have been considered
            if (alignmentNeighbours >= mMaxNeighbours) break;
        }

        // Calculate and apply alignment force
        Vector3 alignment = SteerTo(averageFlockDirection) * mAlignmentWeight;
        AddAccelerationForce(alignment);
    }

    void Cohesion(List<Boids> argNeighbours)
    {
        Vector3 flockCentre = Vector3.zero;
        int cohesionNeighbours = 0;

        // Loop through all neighbouring boids
        foreach (Boids other in argNeighbours)
        {
            if (!other.GetShouldFlock()) continue; // Continue to next, current isnt flocking
            if (ReferenceEquals(this, other)) continue; // Continue to next boid, testing against self?

            float distanceToNeighbour = Vector3.Distance(transform.position, other.transform.position);
            // Move to next neighbour if current is too far away or in the same position as this boid
            if (distanceToNeighbour > mCohesionDistance || distanceToNeighbour <= 0) continue;

            // Add other boids position to flock centre position
            flockCentre += other.transform.position;
            // Increase neighbour count
            cohesionNeighbours++;

            // Break from loop if enough neighbours have been considered
            if (cohesionNeighbours >= mMaxNeighbours) break;
        }

        // Calculate average position of flock
        if (cohesionNeighbours > 0) flockCentre /= cohesionNeighbours;

        // Calculate and apply cohesion force
        Vector3 directionToFlockCentre = flockCentre - transform.position;
        Vector3 cohesion = SteerTo(directionToFlockCentre) * mCohesionWeight;
        AddAccelerationForce(cohesion);
    }

    public void MoveToTarget()
    {
        // Get direction to target
        Vector3 directionToTarget = mTarget - transform.position;
        // Calculate and apply force
        AddAccelerationForce(SteerTo(directionToTarget) * mTargetFollowWeight);
    }

    public void FleePoint(Vector3 argPoint, float argFleeWeighting)
    {
        // Get direction away from argPoint
        Vector3 directionToFlee = transform.position - argPoint;
        // Calculate and apply force
        AddAccelerationForce(SteerTo(directionToFlee) * argFleeWeighting);
    }

    public void SetTarget(Vector3 argTaget)
    {
        mTarget = argTaget;
    }

    public Vector3 GetTarget()
    {
        return mTarget;
    }

    public BoidsManager GetManager()
    {
        return mManager;
    }

    public void SetManager(BoidsManager argManager)
    {
        mManager = argManager;
    }

    public void SetShouldFlock(bool argState)
    {
        mShouldFlock = argState;
    }

    public bool GetShouldFlock()
    {
        return mShouldFlock;
    }
}
