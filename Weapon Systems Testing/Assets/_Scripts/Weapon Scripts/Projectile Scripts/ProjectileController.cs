using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public Projectile projectile;
    public Rigidbody rb;

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
            switch (projectile.collisionAction)
            {
                case Projectile.CollisionAction.DESTROY:
                    StopProjectile();
                    break;
                case Projectile.CollisionAction.BOUNCE:
                    Vector3 normal = collision.GetContact(0).normal;
                    Vector3 bounce = new Vector3(normal.x * Mathf.Abs(velocityStore.x), normal.y * Mathf.Abs(velocityStore.y), normal.z * Mathf.Abs(velocityStore.z));

                    // stupid bounce stuff
                    rb.AddForce(bounce * projectile.bounceAmp);
                    break;
            }
        }
    }

    public void StartProjectile(Transform spawnPoint, Vector3 velocity, float mass)
    {
        gameObject.SetActive(true);

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
