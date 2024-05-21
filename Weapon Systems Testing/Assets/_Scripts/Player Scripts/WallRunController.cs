using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class WallRunController : MonoBehaviour
{
    private float wallCheckDistance;
    [SerializeField]
    private float wallCheckDistOnWall = 3f;
    [SerializeField]
    private float wallCheckDistOffWall = 0.5f;
    [SerializeField]
    private float groundCheckDistance;
    [SerializeField]
    private float wallMinSpeed = 10;
    [SerializeField]
    [Tooltip("How slanted a wall can be before you cannot wallrun on it. Applies to both directions of slant.")]
    private float wallDotDeviation = 0;

    private RaycastHit wallHit;
    public Vector3 wallParallel { get; private set; }
    public Vector3 wallPerp { get; private set; }
    private Transform parentTransform;

    private PlayerController playerController;
    private CharacterController charCont;

    private LayerMask environmentLayers;

    public bool isWallrunning;
    public delegate void WallRunEvent(bool startEnd);
    public event WallRunEvent OnWallrunStatusChange;

    // Start is called before the first frame update
    void Start()
    {
        // Gets the transform of the GameObject
        parentTransform = transform;

        // Gets the PlayerController component
        playerController = GetComponent<PlayerController>();

        // Gets the CharacterController Component
        charCont = GetComponent<CharacterController>();

        // Gets the environment layers set by the PlayerController
        environmentLayers = playerController.environmentLayers;

        // Initializes the wall check distance
        wallCheckDistance = wallCheckDistOffWall;

        // Tells the playerController to register to this script
        playerController.RegisterWallrun(this);
    }
    private void FixedUpdate()
    {
        // Checks if the game is paused
        if (!GameController.isGamePaused)
        {
            // Checks if there is a wall in range and if the player is moving the necessary speed
            if (CheckForWall() && playerController.GetHorizontalVelocity().magnitude > wallMinSpeed)
            {
                // Checks if the player is not yet wallrunning
                if(!isWallrunning)
                {
                    // Sends out an event to notify of wallrun status change
                    OnWallrunStatusChange?.Invoke(true);

                    // Sets the player to be wallrunning
                    isWallrunning = true;

                    // Changes the distance at which the wall is checked for
                    wallCheckDistance = wallCheckDistOnWall;
                }
            }
            else
            {
                // Checks if the player is already wallrunning
                if (isWallrunning)
                {
                    // Sends out an event to notify of wallrun status change
                    OnWallrunStatusChange?.Invoke(false);

                    // Sets the player to not be wallrunning
                    isWallrunning = false;

                    // Changes the distance at which the wall is checked for
                    wallCheckDistance = wallCheckDistOffWall;
                }
            } 
        }
    }

    /// <summary>
    /// Checks if there is a wall that is suitable for wallrunning
    /// </summary>
    /// <returns>True if there is a suitable wall, otherwise false</returns>
    public bool CheckForWall()
    {
        // If the player is on the ground, don't bother checking for walls
        if (Physics.Raycast(parentTransform.position, -parentTransform.up, groundCheckDistance, environmentLayers)) return false;

        // Uses a raycast from each side of the player to check for a wall. Rays are cast in order: right, left
        // onWall is set to 0 if a right wall is hit, 1 if a left wall is hit and -1 if no wall was hit
        int onWall = Physics.Raycast(parentTransform.position, parentTransform.right, out wallHit, wallCheckDistance, environmentLayers) ? 0 : Physics.Raycast(parentTransform.position, -parentTransform.right, out wallHit, wallCheckDistance, environmentLayers) ? 1 : -1;

        // Checks to see if there was a wall found
        if (onWall != -1)
        {
            Vector3 normal = wallHit.normal;

            // Removes the vertical component from the wall's normal
            Vector3 check = new Vector3(normal.x, 0, normal.z).normalized;

            wallPerp = normal;

            // Gets the vector parallel to the wall in the direction that the player is facing
            if (onWall == 0)
                wallParallel = new Vector3(normal.z, 0, -normal.x).normalized;
            else if (onWall == 1)
                wallParallel = new Vector3(-normal.z, 0, normal.x).normalized;

            // Checks if the wall is too slanted to be run on
            // Uses the DotProduct to check if the wall is within a certain angle of vertical
            return Vector3.Dot(normal, check) >= 1 - wallDotDeviation;
        }

        return false;
    }

}
