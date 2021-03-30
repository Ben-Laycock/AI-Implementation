using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boids : MonoBehaviour
{
    [SerializeField] private Boids mFlockLeader = null;
    [SerializeField] private LayerMask mCollisionDetectionMask = new LayerMask();
    [SerializeField] private GameObject mExplosionVFXPrefab = null;
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
    [SerializeField] private float mLeaderFollowWeight = 1f;
    [SerializeField] private float mObstacleAvoidanceWeight = 1f;
    
    [Space]
    private Flock mFlock = null;
    public int mHealth = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (mHealth <= 0)
        {
            if (mFlock != null) mFlock.RemoveAgent(this);

            if (mExplosionVFXPrefab != null)
            {
                GameObject explosion = PoolSystem.Instance.GetObjectFromPool(mExplosionVFXPrefab, true);
                explosion.transform.position = transform.position;
                explosion.GetComponent<DeactivateVFX>().WaitForDeactivation();
            }

            Destroy(this.gameObject);
        }
    }

    public void UpdateBoid(List<Boids> argNeighbours)
    {
        mAcceleration = Vector3.zero;

        if (mFlockLeader != null)
        {
            if (CanSee(mFlockLeader.transform.position, mObstacleAvoidanceDistance))
            {
                Vector3 directionToTarget = mFlockLeader.transform.position - transform.position;
                AddAccelerationForce(SteerTo(directionToTarget) * mLeaderFollowWeight);
            }
        }

        Vector3 separation = SteerTo(Separate(argNeighbours)) * mSeparationWeight;
        Vector3 alignment = SteerTo(Alignment(argNeighbours)) * mAlignmentWeight;
        Vector3 cohesion = SteerTo(Cohesion(argNeighbours)) * mCohesionWeight;

        AddAccelerationForce(alignment);
        AddAccelerationForce(cohesion);
        AddAccelerationForce(separation);

        if (CollisionAhead())
        {
            Vector3 obstacleAvoidance = SteerTo(ObstacleAvoidanceDirection()) * mObstacleAvoidanceWeight;
            AddAccelerationForce(obstacleAvoidance);
        }

        mVelocity += mAcceleration * Time.deltaTime;
        float speed = mVelocity.magnitude;
        speed = Mathf.Clamp(speed, mMinMaxSpeed.x, mMinMaxSpeed.y);
        mVelocity = mVelocity.normalized * speed;

        transform.position += mVelocity * Time.deltaTime;
        if (mVelocity.normalized != Vector3.zero)
        {
            if (mUseSmoothRotation)
            {
                Vector3 pointOfInterest = transform.position + mVelocity;
                if (CanSee(pointOfInterest, mObstacleAvoidanceDistance))
                    transform.forward = mVelocity.normalized;
            }
            else transform.forward = mVelocity.normalized;
        }
    }

    void AddAccelerationForce(Vector3 argForce)
    {
        mAcceleration += argForce;
    }

    Vector3 SteerTo(Vector3 argVector)
    {
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
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, mCollisionRadiusCheck, transform.forward, out hit, mObstacleAvoidanceDistance, mCollisionDetectionMask)) return true;

        return false;
    }

    Vector3 ObstacleAvoidanceDirection()
    {
        RaycastHit hit;
        int numberOfPaths = BoidsManager.mCollisionTestDirections.Length;
        for (int i = 0; i < numberOfPaths; i++)
        {
            Vector3 path = transform.TransformDirection(BoidsManager.mCollisionTestDirections[i]);
            if (!Physics.SphereCast(transform.position, mCollisionRadiusCheck, path, out hit, mObstacleAvoidanceDistance, mCollisionDetectionMask)) return path;
            
        }

        return transform.forward;
    }

    Vector3 Separate(List<Boids> argNeighbours)
    {
        Vector3 averageAvoidanceDirection = Vector3.zero;
        int separationNeighbours = 0;

        foreach (Boids other in argNeighbours)
        {
            if (ReferenceEquals(this, other)) continue; // Continue to next boid, testing against self?

            float distanceToNeighbour = Vector3.Distance(transform.position, other.transform.position);
            if (distanceToNeighbour > mSeparationDistance || distanceToNeighbour <= 0) continue;

            Vector3 seperationDirection = transform.position - other.transform.position;
            //seperationDirection.Normalize();
            seperationDirection /= distanceToNeighbour;

            averageAvoidanceDirection += seperationDirection;
            separationNeighbours++;

            if (separationNeighbours >= mMaxNeighbours) break;
        }

        return averageAvoidanceDirection;
    }

    Vector3 Alignment(List<Boids> argNeighbours)
    {
        Vector3 averageFlockDirection = Vector3.zero;
        int alignmentNeighbours = 0;

        foreach (Boids other in argNeighbours)
        {
            if (ReferenceEquals(this, other)) continue; // Continue to next boid, testing against self?

            float distanceToNeighbour = Vector3.Distance(transform.position, other.transform.position);
            if (distanceToNeighbour > mAlignmentDistance || distanceToNeighbour <= 0) continue;

            averageFlockDirection += other.mVelocity;
            alignmentNeighbours++;

            if (alignmentNeighbours >= mMaxNeighbours) break;
        }

        return averageFlockDirection;
    }

    Vector3 Cohesion(List<Boids> argNeighbours)
    {
        Vector3 flockCentre = Vector3.zero;
        int cohesionNeighbours = 0;

        foreach (Boids other in argNeighbours)
        {
            if (ReferenceEquals(this, other)) continue; // Continue to next boid, testing against self?

            float distanceToNeighbour = Vector3.Distance(transform.position, other.transform.position);
            if (distanceToNeighbour > mCohesionDistance || distanceToNeighbour <= 0) continue;

            flockCentre += other.transform.position;
            cohesionNeighbours++;

            if (cohesionNeighbours >= mMaxNeighbours) break;
        }

        if (cohesionNeighbours > 0) flockCentre /= cohesionNeighbours;

        return (flockCentre - transform.position);
    }

    public void SetLeader(Boids argLeader)
    {
        mFlockLeader = argLeader;
    }

    public Boids GetLeader()
    {
        return mFlockLeader;
    }

    public void SetFlock(Flock argFlock)
    {
        mFlock = argFlock;
    }

    public int GetHealth()
    {
        return mHealth;
    }

    public void Kill()
    {
        Destroy(this.gameObject);
    }
}
