using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController charCont;

    [SerializeField]
    private float moveSpeed = 10;
    [SerializeField]
    private float jumpHeight = 10;
    [SerializeField]
    [Tooltip("Negative values will pull player downward, Positive value will push them up")]
    private float gravity = -0.98f;

    public LayerMask environmentLayers;

    private KeyCode keyJump = KeyCode.Space;

    public float moveY = 0;

    // Start is called before the first frame update
    void Start()
    {
        charCont = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameController.isGamePaused)
        {
            MovePlayer();
        }
    }

    private void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        if (charCont.isGrounded)
        {
            if(Input.GetKeyDown(keyJump))
            {
                moveY = jumpHeight;
            }
        }
        else
        {
            // Sets into fall if hitting a ceiling
            if (Physics.Raycast(transform.position, Vector3.up, 1.1f, environmentLayers) && moveY > 0)
                moveY = 0;

            if(Mathf.Abs(0 - moveY) < 0.3f)
                moveY += gravity * 0.75f * Time.deltaTime;
            else
                moveY += gravity * Time.deltaTime;
        }

        Vector3 finalMove = moveX * transform.right + moveY * transform.up + moveZ * transform.forward;

        charCont.Move(finalMove * Time.deltaTime * moveSpeed);
    }

    #region Wallrun Methods

    public void RegisterWallrun(WallRunController wallRunCont)
    {
        wallRunCont.OnWallrunStatusChange += OnWallrunStatusChange;
    }
    private void OnWallrunStatusChange(bool startEnd)
    {
        Debug.Log("Wallrunning: " + startEnd);
    }

    #endregion
}
