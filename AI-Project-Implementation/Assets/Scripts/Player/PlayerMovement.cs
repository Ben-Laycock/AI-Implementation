using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CurrentMovementType
{
    None = 0,
    Normal = 1,
    Reverse = 2,
    Boosting = 3
}

public class PlayerMovement : MonoBehaviour
{ 

    [Header("Player objects")]
    [SerializeField] private GameObject mPlayerCameraTarget;
    [SerializeField] private GameObject mPlayerCameraObject;
    private Camera mPlayerCamera;
    private Rigidbody mCameraTargetRigidbody;

    [Header("Player Movement Values")]
    [SerializeField] private float mPlayerNormalSpeed = 5.0f;
    [SerializeField] private float mPlayerBoostMultiplier = 1.5f;
    [SerializeField] private float mPlayerCameraSensitivity = 1.0f;
    [SerializeField] private float mCameraFOVLerpSpeed = 1.0f;
    [SerializeField] private float mCameraRotationLerpSpeed = 1.0f;

    [SerializeField] private float mTimeScaleSpeedFactor = 60.0f;

    [Header("Player Current Values")]
    [SerializeField] private float mPlayerMovementSpeed = 0.0f;
    [SerializeField] private CurrentMovementType mCurrentMovementType = CurrentMovementType.None;

    [Header("Player Movement Inputs")]
    [SerializeField] private bool mADInput = true;
    [SerializeField] private bool mSpaceControlInput = true;
    [SerializeField] private bool mQEInput = true;
    [SerializeField] private bool mWSInput = true;
    [SerializeField] private bool mShiftInput = true;

    private float mTargetVelocity = 0.0f;
    private float mLerpTimer = 0.0f;

    private CurrentMovementType mPreviousMovementType = CurrentMovementType.None;

    private Rigidbody mPlayerRigidbody = null;
    private Vector3 mMovementDirection = Vector3.zero;
    private Vector3 mInputDirections = Vector3.zero;

    private void Start()
    {
        mPlayerCameraTarget = GameObject.Find("CameraTarget");
        mPlayerCameraObject = GameObject.Find("PlayerCamera");
        mCameraTargetRigidbody = mPlayerCameraTarget.GetComponent<Rigidbody>();

        mPlayerCamera = mPlayerCameraObject.GetComponent<Camera>();

        mPlayerRigidbody = gameObject.GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        ResetPlayerMovementValues();
        GetPlayerInput();

        CalculatePlayerCamera();

        MovePlayer();

    }

    public void ResetPlayerMovementValues()
    {
        mMovementDirection = Vector3.zero;
        mInputDirections = Vector3.zero;
    }

    public void GetPlayerInput()
    {
        Vector3 inputDirections = new Vector3(Input.GetAxisRaw(GameConstants.Instance.HorizontalInput), 
                                              Input.GetAxisRaw(GameConstants.Instance.JumpInput) - Input.GetAxisRaw(GameConstants.Instance.CrouchInput), 
                                              Input.GetAxisRaw(GameConstants.Instance.VerticalInput)
                                              );

        mInputDirections = new Vector3(Input.GetAxisRaw(GameConstants.Instance.HorizontalInput),
                                       0,
                                       Input.GetAxisRaw(GameConstants.Instance.VerticalInput)
                                       );

        if(mQEInput)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                mInputDirections.y -= 1;
            }

            if (Input.GetKey(KeyCode.E))
            {
                mInputDirections.y += 1;
            }
        }
        

        if (inputDirections.x != 0 && mADInput)
        {
           mMovementDirection += transform.right * inputDirections.x;
        }

        if (inputDirections.y != 0 && mSpaceControlInput)
        {
            mMovementDirection += transform.up * inputDirections.y;
        }

        if (inputDirections.z != 0 && mWSInput)
        {
            mMovementDirection += transform.forward * inputDirections.z;
        }

        mMovementDirection.Normalize();

        //Debug.Log("Target: " + mTargetVelocity + "Current: " + mPlayerMovementSpeed + "Change as edited range: " + mCameraDistanceDifference);

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W) && mShiftInput)
        {
            mTargetVelocity = mPlayerBoostMultiplier * mPlayerNormalSpeed;

            mCurrentMovementType = CurrentMovementType.Boosting;
            
            if(mCurrentMovementType != mPreviousMovementType)
            {
                mPreviousMovementType = CurrentMovementType.Boosting;
                mLerpTimer = 0.0f;
            }
        }
        else if (inputDirections.z == 1)
        {
            mTargetVelocity = mPlayerNormalSpeed;

            mCurrentMovementType = CurrentMovementType.Normal;

            if (mCurrentMovementType != mPreviousMovementType)
            {
                mPreviousMovementType = CurrentMovementType.Normal;
                mLerpTimer = 0.0f;
            }    
        }
        else if (inputDirections.z == -1)
        {
            mTargetVelocity = -mPlayerNormalSpeed / 2;

            mCurrentMovementType = CurrentMovementType.Reverse;

            if (mCurrentMovementType != mPreviousMovementType)
            {
                mPreviousMovementType = CurrentMovementType.Reverse;
                mLerpTimer = 0.0f;
            }
        }
        else
        {
            mTargetVelocity = 0.0f;

            mCurrentMovementType = CurrentMovementType.None;

            if (mCurrentMovementType != mPreviousMovementType)
            {
                mPreviousMovementType = CurrentMovementType.None;
                mLerpTimer = 0.0f;
            }
        }

        if (mLerpTimer < 1.0f)
        {
            mLerpTimer += Time.deltaTime * mCameraFOVLerpSpeed;
            
        }

        mPlayerMovementSpeed = Mathf.Lerp(mPlayerMovementSpeed, mTargetVelocity, mLerpTimer);

    }

    public void CalculatePlayerCamera()
    {
        float rotateHorizontal = Input.GetAxis(GameConstants.Instance.HorizontalLookInput);
        float rotateVertical = -Input.GetAxis(GameConstants.Instance.VerticalLookInput);

        Vector3 mShipRotationVector = new Vector3(rotateVertical * mPlayerCameraSensitivity, rotateHorizontal * mPlayerCameraSensitivity, -mInputDirections.y);
        mShipRotationVector.Normalize();

        mPlayerRigidbody.angularVelocity = (transform.rotation * (mShipRotationVector * 1.5f)) * Time.deltaTime * mTimeScaleSpeedFactor;

        mPlayerCamera.fieldOfView = Mathf.Clamp(60.0f + ((30 / (mPlayerBoostMultiplier * mPlayerNormalSpeed)) * mPlayerMovementSpeed), 50.0f, 90.0f);

        mPlayerCameraTarget.transform.rotation = Quaternion.Lerp(mPlayerCameraTarget.transform.rotation, transform.rotation, mCameraRotationLerpSpeed * Time.deltaTime);
    }

    public void MovePlayer()
    {
        mPlayerCameraTarget.transform.position = transform.position;
        mPlayerRigidbody.velocity = transform.forward * mPlayerMovementSpeed;
    }

    public float Remap(float from, float fromMin, float fromMax, float toMin, float toMax)
    {
        var fromAbs = from - fromMin;
        var fromMaxAbs = fromMax - fromMin;

        var normal = fromAbs / fromMaxAbs;

        var toMaxAbs = toMax - toMin;
        var toAbs = toMaxAbs * normal;

        var to = toAbs + toMin;

        return to;
    }


    public float GetPlayerMovementSpeed()
    {
        return mPlayerMovementSpeed;
    }

    public float GetPlayerNormalSpeed()
    {
        return mPlayerNormalSpeed;
    }

    public float GetPlayerTargetSpeed()
    {
        return mTargetVelocity;
    }

    public float GetPlayerBoostMultiplier()
    {
        return mPlayerBoostMultiplier;    
    }
}