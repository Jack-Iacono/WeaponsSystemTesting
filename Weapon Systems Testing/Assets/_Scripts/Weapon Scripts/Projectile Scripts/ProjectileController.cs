using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour, Interactable
{
    public Projectile projectile;
    public Rigidbody rb;

    public Frame parentFrame { get; private set; }

    private Vector3 currentMoveStore = Vector3.zero;
    public void Initialize()
    {
        // Instantiates a new projectile script
        projectile = Instantiate(projectile);
        projectile.ProjectileInitialize(this);
    }

    private void Update()
    {
        projectile.ProjectileUpdate(Time.deltaTime);
    }
    private void LateUpdate()
    {
        currentMoveStore = rb.velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the layermask that is being collided with is one which is within the collision layers for this object
        if (HelperScripts.LayermaskContains(projectile.interactLayers, collision.gameObject.layer))
        {
            Interactable col;

            // Check if this collider is an Interactable
            if (collision.gameObject.TryGetComponent(out col))
                col.ProjectileHit(this);

            // Unique Interactions for each collision action
            switch (projectile.collisionAction)
            {
                case Projectile.CollisionAction.DESTROY:
                    StopProjectile();
                    break;
            }
        }
    }

    /// <summary>
    /// Begins the projectile's movement
    /// </summary>
    /// <param name="parentFrame">The frame that this projectile came from</param>
    /// <param name="origin">The spawn point of this projectile</param>
    /// <param name="currentMove">The movement that should be applied to the projectile</param>
    /// <param name="mass">The mass that this projectile should have</param>
    public void StartProjectile(Frame parentFrame, Transform origin, Vector3 currentMove, float mass = 1)
    {
        gameObject.SetActive(true);

        this.parentFrame = parentFrame;

        rb.mass = mass;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.MovePosition(origin.position);
        rb.MoveRotation(origin.rotation);

        rb.velocity = currentMove;

        projectile.StartProjectile();
    }
    public void StopProjectile()
    {
        projectile.StopProjectile();
        gameObject.SetActive(false);
    }
    public void ProjectileLifetimeEnd()
    {
        StopProjectile();
    }
}
