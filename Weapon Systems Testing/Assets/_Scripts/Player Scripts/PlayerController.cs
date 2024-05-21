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
        // Get the needed components
        charCont = GetComponent<CharacterController>();
        TryGetComponent(out wallRunController);
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the game is paused
        if(!GameController.isGamePaused)
        {
            // Retrieve the keybaord input
            GetInput();

            // If the player is wallrunning, use the wallrun movement, else use the normal movement
            if (wallRunController && isWallrun)
            {
                CalculateWallrunPlayerMove();   
            }
            else
            {
                CalculateNormalPlayerMove();
            }

            // Move the player
            MovePlayer();
        }
    }
    private void FixedUpdate()
    {
        // Check if the game is paused
        if (!GameController.isGamePaused)
        {
            // Calculate the velocity based on the new position and the old position
            velocity = (transform.position - previousFramePosition) / Time.fixedDeltaTime;
            // Set the old position for the next frame
            previousFramePosition = transform.position;
        }
    }

    private void GetInput()
    {
        // Get the input from the keyboard 
        currentInput = new Vector3
            (
                Input.GetAxis("Horizontal"),
                Input.GetKeyDown(keyJump) ? 1 : 0,
                Input.GetAxis("Vertical")
            );
    }

    private void CalculateNormalPlayerMove()
    {
        // Convert the keyboard input into global position change using the player's current rotation
        float moveX = currentInput.x * transform.right.x * maxNormalSpeed + currentInput.z * transform.forward.x * maxNormalSpeed;
        float moveZ = currentInput.x * transform.right.z * maxNormalSpeed + currentInput.z * transform.forward.z * maxNormalSpeed;

        // Check if the player is on the ground, i.e. not in the air
        if (charCont.isGrounded)
        {
            // Refresh the double jump count
            currentDoubleJumps = doubleJumpCount;

            // Makes the player jump if the input is received
            if(currentInput.y != 0)
            {
                currentMove.y = jumpHeight;
            }

            // Determines the type of acceleration to use based on the player's input
            float accelX = moveX == 0 ? groundDeceleration : groundAcceleration; 
            float accelZ = moveZ == 0 ? groundDeceleration : groundAcceleration;

            // Moves the currentMove value toward the desired movement direction based on the acceleration
            currentMove.x = Mathf.MoveTowards(currentMove.x, moveX, accelX * Time.deltaTime);
            currentMove.z = Mathf.MoveTowards(currentMove.z, moveZ, accelZ * Time.deltaTime);
        }
        else
        {
            // Checks if the player can and has double jumps
            if (canDoubleJump && currentDoubleJumps > 0 && currentInput.y != 0)
            {
                // Makes the player jump
                currentMove.y = jumpHeight * doubleJumpHeightAmp;

                // Adds a horizontal component to the jump to make it feel quicker
                currentMove.x += moveX * doubleJumpDirectionAmp;
                currentMove.z += moveZ * doubleJumpDirectionAmp;

                // Subtracts from the amount of double jumps that can be performed
                currentDoubleJumps--;
            }

            // Sets into fall if hitting a ceiling
            if (Physics.Raycast(transform.position, Vector3.up, 1.1f, environmentLayers) && currentMove.y > 0)
                currentMove.y = 0;

            // Applies gravity
            currentMove.y -= gravity * -2 * Time.deltaTime;

            // Checks what acceleration value should be used
            float accelX = moveX == 0 ? airDeceleration : airAcceleration;
            float accelZ = moveZ == 0 ? airDeceleration : airAcceleration;

            // Moves the currentMove value toward the desired movement direction based on the acceleration
            currentMove.x = Mathf.MoveTowards(currentMove.x, moveX, accelX * Time.deltaTime);
            currentMove.z = Mathf.MoveTowards(currentMove.z, moveZ, accelZ * Time.deltaTime);
        }
    }
    private void CalculateWallrunPlayerMove()
    {
        // Refreshes the double jump count
        currentDoubleJumps = doubleJumpCount;

        // Checks if the player has input a jump command
        if (currentInput.y > 0)
        {
            // Adds the jump to the currentMove
            // Similar to the double jump, but with even more horizontal movement
            currentMove += ( wallRunController.wallPerp * 1.5f + Vector3.up / 2 ) * jumpHeight * 1.5f;

            // Sets a timer so that the player does not instantly snap back to the wall after jumping
            wallJumpTimer = wallJumpTime;
        }
        // Checks if the player should be wallrunning
        else if(wallJumpTimer == 0)
        {
            // Check if the wallrun is not timed out
            if (wallRunTimer > 0)
            {
                // Move the player along the wall
                currentMove = maxNormalSpeed * 2f * wallRunController.wallParallel;

                // Decrease the amount of time left for the wallrun
                wallRunTimer -= Time.deltaTime;
            }
            else
            {
                // Apply gravity
                currentMove.y -= gravity * -1 * Time.deltaTime;
            }
        }

        // Decrease the time remaining until the player can reattach to the wall
        if(wallJumpTimer > 0)
            wallJumpTimer -= Time.deltaTime;
    }

    private void MovePlayer()
    {
        // Uses the Character Controller component to move the player
        charCont.Move(currentMove * Time.deltaTime);
    }

    public void Die()
    {
        // Moves the player back to the "spawn"
        charCont.enabled = false;
        transform.position = Vector3.up * 5;
        charCont.enabled = true;
    }

    #region Wallrun Methods

    /// <summary>
    /// Registers this script to check for wallrun related events
    /// </summary>
    /// <param name="wallRunCont">The WallRunController to register to</param>
    public void RegisterWallrun(WallRunController wallRunCont)
    {
        wallRunCont.OnWallrunStatusChange += OnWallrunStatusChange;
    }

    /// <summary>
    /// Triggered when the wallrun status changes
    /// </summary>
    /// <param name="startEnd">True: The wallrun is ongoing. False: The wallrun is not ongoing</param>
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

    /// <returns>The velocity of the player</returns>
    public Vector3 GetVelocity()
    {
        return velocity;
    }

    /// <returns>The velocity of the player without the vertical component</returns>
    public Vector3 GetHorizontalVelocity()
    {
        return new Vector3(velocity.x, 0, velocity.z);
    }

    #endregion
}
