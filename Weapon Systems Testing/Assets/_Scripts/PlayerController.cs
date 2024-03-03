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

    private KeyCode keyJump = KeyCode.Space;

    private float moveY = 0;

    // Start is called before the first frame update
    void Start()
    {
        charCont = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
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
            if(Mathf.Abs(0 - moveY) < 0.3f)
                moveY += gravity * 0.75f * Time.deltaTime;
            else
                moveY += gravity * Time.deltaTime;
        }

        Vector3 finalMove = moveX * transform.right + moveY * transform.up + moveZ * transform.forward;

        charCont.Move(finalMove * Time.deltaTime * moveSpeed);
    }
}
