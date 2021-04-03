using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float mMovementSpeed = 1f;
    public float mMouseSensitivity = 100.0f;
    public Vector2 mMinMaxRotation = new Vector2(-80f, 80f);

    private float mRotationY = 0.0f; // rotation around the up/y axis
    private float mRotationX = 0.0f; // rotation around the right/x axis

    void Start()
    {
    }

    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;

        float mouseX = Input.GetAxis("HorizontalLook");
        float mouseY = -Input.GetAxis("VerticalLook");

        mRotationY += mouseX * mMouseSensitivity * Time.deltaTime;
        mRotationX += mouseY * mMouseSensitivity * Time.deltaTime;

        mRotationX = Mathf.Clamp(mRotationX, mMinMaxRotation.x, mMinMaxRotation.y);

        Quaternion localRotation = Quaternion.Euler(mRotationX, mRotationY, 0.0f);
        transform.rotation = localRotation;


        Vector3 movementDirection = Vector3.zero;

        movementDirection += Input.GetAxisRaw("Vertical") * Vector3.forward;
        movementDirection += Input.GetAxisRaw("Horizontal") * Vector3.right;

        transform.Translate(movementDirection.normalized * mMovementSpeed * Time.deltaTime);
    }

}
