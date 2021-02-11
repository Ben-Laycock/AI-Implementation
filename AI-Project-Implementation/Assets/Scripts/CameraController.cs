using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private GameObject mCameraObject = null;
    [SerializeField] private float mMovementSpeed = 1f;
    [SerializeField] private float mCameraSensitivity = 1f;
    [SerializeField] private Vector2 mCameraVerticalMinMax = Vector2.zero;
    [SerializeField] private bool mInvertVertical = false;

    private float currentYRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 movementDir = GetMovementDirection();
        transform.position += transform.right * movementDir.x * mMovementSpeed * Time.deltaTime;
        transform.position += Vector3.up * movementDir.y * mMovementSpeed * Time.deltaTime;
        transform.position += transform.forward * movementDir.z * mMovementSpeed * Time.deltaTime;

        ManageCamera();

    }

    void ManageCamera()
    {

        Vector2 cameraRotationInput = new Vector2(Input.GetAxisRaw(GameConstants.Instance.HorizontalLookInput), Input.GetAxisRaw(GameConstants.Instance.VerticalLookInput));

        transform.eulerAngles += new Vector3(0, cameraRotationInput.x * mCameraSensitivity * Time.deltaTime, 0);

        currentYRotation += cameraRotationInput.y * mCameraSensitivity * Time.deltaTime;
        currentYRotation = Mathf.Clamp(currentYRotation, mCameraVerticalMinMax.x, mCameraVerticalMinMax.y);

        if (mInvertVertical)
            mCameraObject.transform.localRotation = Quaternion.Euler(currentYRotation, 0, 0);
        else
            mCameraObject.transform.localRotation = Quaternion.Euler(-currentYRotation, 0, 0);
    }

    Vector3 GetMovementDirection()
    {

        Vector3 movementDir = Vector3.zero;

        movementDir.x = Mathf.Clamp(Input.GetAxisRaw(GameConstants.Instance.HorizontalInput), -1, 1);
        movementDir.z = Mathf.Clamp(Input.GetAxisRaw(GameConstants.Instance.VerticalInput), -1, 1);

        if (Input.GetAxisRaw(GameConstants.Instance.JumpInput) > 0)
            movementDir.y = 1;
        else if (Input.GetAxisRaw(GameConstants.Instance.CrouchInput) > 0)
            movementDir.y = -1;

        return movementDir;
    }

}
