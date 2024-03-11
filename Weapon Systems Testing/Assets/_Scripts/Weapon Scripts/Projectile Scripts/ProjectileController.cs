using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour, Interactable
{
    public Projectile projectile;
    public Rigidbody rb;

    public Frame parentFrame { get; private set; }

    private Vector3 velocityStore = Vector3.zero;
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
        velocityStore = rb.velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (HelperScripts.LayermaskContains(projectile.interactLayers, collision.gameObject.layer))
        {
            Interactable col;

            if (collision.gameObject.TryGetComponent(out col))
                col.ProjectileHit(this);

            // Unique Interactions for each collision action
            switch (projectile.collisionAction)
            {
                case Projectile.CollisionAction.DESTROY:
                    StopProjectile();
                    break;
                case Projectile.CollisionAction.BOUNCE:
                    
                    break;
            }
        }
    }

    public void StartProjectile(Frame parentFrame, Vector3 positionOffset, Vector3 rotationOffset, float velocity, float mass = 1)
    {
        gameObject.SetActive(true);

        this.parentFrame = parentFrame;

        rb.mass = mass;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.MovePosition(positionOffset);
        rb.MoveRotation(Quaternion.Euler(rotationOffset));

        rb.velocity = transform.forward * velocity;

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
