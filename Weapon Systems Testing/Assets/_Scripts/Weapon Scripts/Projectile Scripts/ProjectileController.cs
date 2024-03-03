using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour, Interactable
{
    public Projectile projectile;
    public Rigidbody rb;

    public Weapon parentWeapon { get; private set; }

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

    public void StartProjectile(Weapon parentWeapon, Transform spawnPoint, Vector3 velocity, float mass = 1)
    {
        gameObject.SetActive(true);

        this.parentWeapon = parentWeapon;

        rb.mass = mass;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.MovePosition(spawnPoint.position);
        rb.MoveRotation(spawnPoint.rotation);

        //rb.AddForce(velocity);
        rb.velocity = velocity;

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
