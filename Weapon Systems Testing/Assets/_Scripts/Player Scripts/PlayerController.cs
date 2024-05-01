using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController charCont;
    [SerializeField]
    private CameraController cameraCont;

    [Header("Movement Variables")]
    [SerializeField]
    private float moveSpeed = 10;
    [SerializeField]
    private float jumpHeight = 10;
    [SerializeField]
    [Tooltip("Negative values will pull player downward, Positive value will push them up")]
    private float gravity = -0.98f;

    [Header("Acceleration Variables", order = 2)]
    [SerializeField]
    private float groundAcceleration = 1;
    [SerializeField]
    private float airAcceleration = 1;
    [SerializeField]
    private float groundDeceleration = 1;
    [SerializeField]
    private float airDeceleration = 1;

    [Header("Movement Contstraints", order = 2)]
    [SerializeField]
    private float maxNormalSpeed = 10;
    [SerializeField]
    private float maxCapSpeed = 100;

    [Header("Interaction Variables")]
    public LayerMask environmentLayers;

    private KeyCode keyJump = KeyCode.Space;

    private Vector3 currentInput = Vector3.zero;
    private Vector3 currentMove = Vector3.zero;

    private Vector3 previousFramePosition = Vector3.zero;
    private Vector3 velocity = Vector3.zero;

    #region Wallrun Variables

    private bool isWallrun;
    private WallRunController wallRunController;

    private float wallJumpTimer;
    private const float wallJumpTime = 0.5f;

    [SerializeField]
    private float wallRunTime;
    private float wallRunTimer;

    #endregion

    #region Double Jumping Variables

    [Header("Double Jump Variables")]
    [SerializeField]
    private bool canDoubleJump = true;
    [SerializeField]
    private int doubleJumpCount = 1;
    private int currentDoubleJumps;
    [SerializeField]
    private float doubleJumpHeightAmp = 1;
    [SerializeField]
    private float doubleJumpDirectionAmp = 1;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        charCont = GetComponent<CharacterController>();
        TryGetComponent(out wallRunController);
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameController.isGamePaused)
        {
            GetInput();

            if (wallRunController && isWallrun)
            {
                CalculateWallrunPlayerMove();   
            }
            else
            {
                CalculateNormalPlayerMove();
            }

            MovePlayer();
        }
    }
    private void FixedUpdate()
    {
        if (!GameController.isGamePaused)
        {
            velocity = (transform.position - previousFramePosition) / Time.fixedDeltaTime;
            previousFramePosition = transform.position;
        }
    }

    private void GetInput()
    {
        currentInput = new Vector3
            (
                Input.GetAxis("Horizontal"),
                Input.GetKeyDown(keyJump) ? 1 : 0,
                Input.GetAxis("Vertical")
            );
    }

    private void CalculateNormalPlayerMove()
    {
        float moveX = currentInput.x * transform.right.x * maxNormalSpeed + currentInput.z * transform.forward.x * maxNormalSpeed;
        float moveZ = currentInput.x * transform.right.z * maxNormalSpeed + currentInput.z * transform.forward.z * maxNormalSpeed;

        if (charCont.isGrounded)
        {
            currentDoubleJumps = doubleJumpCount;

            if(currentInput.y != 0)
            {
                currentMove.y = jumpHeight;
            }

            float accelX = moveX == 0 ? groundDeceleration : groundAcceleration; 
            float accelZ = moveZ == 0 ? groundDeceleration : groundAcceleration;

            currentMove.x = Mathf.MoveTowards(currentMove.x, moveX, accelX * Time.deltaTime);
            currentMove.z = Mathf.MoveTowards(currentMove.z, moveZ, accelZ * Time.deltaTime);
        }
        else
        {
            if (canDoubleJump && currentDoubleJumps > 0 && currentInput.y != 0)
            {
                currentMove.y = jumpHeight * doubleJumpHeightAmp;

                currentMove.x += moveX * doubleJumpDirectionAmp;
                currentMove.z += moveZ * doubleJumpDirectionAmp;

                currentDoubleJumps--;
            }

            // Sets into fall if hitting a ceiling
            if (Physics.Raycast(transform.position, Vector3.up, 1.1f, environmentLayers) && currentMove.y > 0)
                currentMove.y = 0;

            currentMove.y -= gravity * -2 * Time.deltaTime;

            float accelX = moveX == 0 ? airDeceleration : airAcceleration;
            float accelZ = moveZ == 0 ? airDeceleration : airAcceleration;

            currentMove.x = Mathf.MoveTowards(currentMove.x, moveX, accelX * Time.deltaTime);
            currentMove.z = Mathf.MoveTowards(currentMove.z, moveZ, accelZ * Time.deltaTime);
        }
    }
    private void CalculateWallrunPlayerMove()
    {
        currentDoubleJumps = doubleJumpCount;

        if (currentInput.y > 0)
        {
            currentMove += ( wallRunController.wallPerp * 1.5f + Vector3.up / 2 ) * jumpHeight * 1.5f;
            wallJumpTimer = wallJumpTime;
        }
        else if(wallJumpTimer == 0)
        {
            if (wallRunTimer > 0)
            {
                currentMove = maxNormalSpeed * 2f * wallRunController.wallParallel;
                wallRunTimer -= Time.deltaTime;
            }
            else
            {
                currentMove.y -= gravity * -1 * Time.deltaTime;
            }
        }

        if(wallJumpTimer > 0)
            wallJumpTimer -= Time.deltaTime;
    }

    private void MovePlayer()
    {
        charCont.Move(currentMove * Time.deltaTime);
    }

    public void Die()
    {
        charCont.enabled = false;
        transform.position = Vector3.up * 5;
        charCont.enabled = true;
        Debug.Log("Die");
    }

    #region Wallrun Methods

    public void RegisterWallrun(WallRunController wallRunCont)
    {
        wallRunCont.OnWallrunStatusChange += OnWallrunStatusChange;
    }
    private void OnWallrunStatusChange(bool startEnd)
    {
        isWallrun = startEnd;

        if (isWallrun)
        {
            wallRunTimer = wallRunTime;
            currentMove.y = 0;
            wallJumpTimer = 0;
        }
    }

    #endregion

    #region Get Methods

    public Vector3 GetVelocity()
    {
        return velocity;
    }
    public Vector3 GetHorizontalVelocity()
    {
        return new Vector3(velocity.x, 0, velocity.z);
    }

    #endregion
}
