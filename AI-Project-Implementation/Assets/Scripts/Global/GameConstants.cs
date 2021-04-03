using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConstants : MonoBehaviour
{

    private static GameConstants sInstance = null;

    // Get Singleton Instance
    public static GameConstants Instance
    {
        get { return sInstance; }
    }

    private void Awake()
    {
        if (null != sInstance && this != sInstance) Destroy(this.gameObject);

        sInstance = this;
        DontDestroyOnLoad(this.gameObject);
    }


    /*
     * Character control inputs
     */
    [SerializeField] private string mHorizontalInput = "Horizontal";
    public string HorizontalInput
    {
        get { return mHorizontalInput; }
        set { mHorizontalInput = value; }
    }

    [SerializeField] private string mVerticalInput = "Vertical";
    public string VerticalInput
    {
        get { return mVerticalInput; }
        set { mVerticalInput = value; }
    }

    [SerializeField] private string mJumpInput = "Jump";
    public string JumpInput
    {
        get { return mJumpInput; }
        set { mJumpInput = value; }
    }

    [SerializeField] private string mCrouchInput = "Crouch";
    public string CrouchInput
    {
        get { return mCrouchInput; }
        set { mCrouchInput = value; }
    }


    /*
     * Camera control inputs
     */
    [SerializeField] private string mHorizontalLookInput = "HorizontalLook";
    public string HorizontalLookInput
    {
        get { return mHorizontalLookInput; }
        set { mHorizontalLookInput = value; }
    }

    [SerializeField] private string mVerticalLookInput = "VerticalLook";
    public string VerticalLookInput
    {
        get { return mVerticalLookInput; }
        set { mVerticalLookInput = value; }
    }


    /*
     * Controller Stick Deadzones
     */
    [SerializeField] private float mControllerLeftStickDeadzone = 0.025f;
    public float LeftStickDeadzone
    {
        get { return mControllerLeftStickDeadzone; }
        set { mControllerLeftStickDeadzone = value; }
    }


    [SerializeField] private float mControllerRightStickDeadzone = 0.025f;
    public float RightStickDeadzone
    {
        get { return mControllerRightStickDeadzone; }
        set { mControllerRightStickDeadzone = value; }
    }


    [Space][Space][Space]
    

    /*
     * World Variables
     */
    [SerializeField] private Vector3 mGravityDirection = Vector3.down;
    public Vector3 GravityDirection
    {
        get { return mGravityDirection; }
        set { mGravityDirection = value; }
    }


    [SerializeField] private float mGlobalGravityScale = 9.81f;
    public float GlobalGravityScale
    {
        get { return mGlobalGravityScale; }
        set { mGlobalGravityScale = value; }
    }

    [Header("Player Objects")]
    [SerializeField] private GameObject mPlayerObject;
    public GameObject PlayerObject
    {
        get { return mPlayerObject; }
        set { mPlayerObject = value; }
    }

    [Header("Agent Objects")]
    [SerializeField] private BoidsManager mBoidsManager;
    public BoidsManager BoidsManager
    {
        get { return mBoidsManager; }
        set { mBoidsManager = value; }
    }

    [SerializeField] private int mNumberOfMines = 0;
    public int NumberOfMines
    {
        get { return mNumberOfMines; }
        set { mNumberOfMines = value; }
    }

    [SerializeField] private int mMaxNumberOfMines = 5;
    public int MaxNumberOfMines
    {
        get { return mMaxNumberOfMines; }
    }

}
