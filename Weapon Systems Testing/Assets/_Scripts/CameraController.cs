using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class CameraController : MonoBehaviour
{
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
        MoveCamera();   
    }

    private void MoveCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        //Gets the real rotation of the camera
        xRotation = Mathf.Clamp(xRotation - mouseY, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        player.transform.Rotate(0f, mouseX, 0f);
    }
    
}
