using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    [SerializeField][Tooltip("The PlayerController that this object is attached to")]
    private PlayerController player;

    [SerializeField][Tooltip("The main camera that will be associated with this script")]
    private Camera cam;

    [SerializeField][Tooltip("The LOCAL offset of the camera holder object")]
    private Vector3 cameraOffset;

    #region Camera Movement Variables

    // TEMPORARY: Remove when implementing sensitivity
    float sensitivity = 100;

    float xRotation;

    #endregion

    private void Awake()
    {
        // Create a singleton for the camera
        if (instance == null)
            instance = this;
        else
            Destroy(instance);
    }
    // Start is called before the first frame update
    void Start()
    {
        // TEMPORARY: Remove once another script dictates this behaviour
        Cursor.lockState = CursorLockMode.Locked;

        transform.localPosition = cameraOffset;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the game is paused
        if(!GameController.isGamePaused)
        {
            MoveCamera();
        }
    }

    private void MoveCamera()
    {
        // Get the input from the mouse and standardize using sensitivity and deltaTime
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        //Gets the real rotation of the camera
        xRotation = Mathf.Clamp(xRotation - mouseY, -90f, 90f);

        // Rotates the camera up and down ( X-axis )
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // Rotates the player around the y-axis
        player.transform.Rotate(0f, mouseX, 0f);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>The ray that represents the forward direction of the camera</returns>
    public Ray GetSightRay()
    {
        return new Ray(transform.position, transform.forward);
    }

    /// <summary>
    /// Sets the FOV of the camera
    /// </summary>
    /// <param name="fov">The FOV to change the camera to</param>
    public void SetFOV(float fov)
    {
        cam.fieldOfView = fov;
    }
}
