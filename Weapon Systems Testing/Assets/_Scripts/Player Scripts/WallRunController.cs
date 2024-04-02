using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class WallRunController : MonoBehaviour
{
    public float wallCheckDistance;
    public float groundCheckDistance;
    [Tooltip("How slanted a wall can be before you cannot wallrun on it. Applies to both directions of slant.")]
    public float wallDotDeviation = 0;

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
        parentTransform = transform;
        playerController = GetComponent<PlayerController>();
        charCont = GetComponent<CharacterController>();
        environmentLayers = playerController.environmentLayers;

        playerController.RegisterWallrun(this);
    }
    private void FixedUpdate()
    {
        if (!GameController.isGamePaused)
        {
            if (CheckForWall())
            {
                if(!isWallrunning)
                {
                    OnWallrunStatusChange?.Invoke(true);
                    isWallrunning = true;
                }
            }
            else
            {
                if (isWallrunning)
                {
                    OnWallrunStatusChange?.Invoke(false);
                    isWallrunning = false;
                }
            } 
        }
    }

    public bool CheckForWall()
    {
        // If the player is on the ground, don't bother checking for walls
        if (Physics.Raycast(parentTransform.position, -parentTransform.up, groundCheckDistance, environmentLayers)) return false;

        int onWall = Physics.Raycast(parentTransform.position, parentTransform.right, out wallHit, wallCheckDistance, environmentLayers) ? 0 : Physics.Raycast(parentTransform.position, -parentTransform.right, out wallHit, wallCheckDistance, environmentLayers) ? 1 : -1;

        if (onWall != -1)
        {
            Vector3 normal = wallHit.normal;
            Vector3 check = new Vector3(normal.x, 0, normal.z).normalized;

            wallPerp = normal;

            // Gets the vector parallel to the wall in the direction that the player is facing
            if (onWall == 0)
                wallParallel = new Vector3(normal.z, 0, -normal.x).normalized;
            else if (onWall == 1)
                wallParallel = new Vector3(-normal.z, 0, normal.x).normalized;

            // Returns the deviation of the normal from the check number
            return Vector3.Dot(normal, check) >= 1 - wallDotDeviation;
        }

        return false;
    }

}
