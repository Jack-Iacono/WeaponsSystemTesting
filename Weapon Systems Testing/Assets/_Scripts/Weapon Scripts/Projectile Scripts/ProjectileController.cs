using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public Projectile projectile;
    private Rigidbody rb;

    public void Initialize()
    {
        rb = GetComponent<Rigidbody>();

        // Instantiates a new projectile script
        projectile = Instantiate(projectile);
        projectile.ProjectileInitialize(this);
    }

    private void Update()
    {
        projectile.ProjectileUpdate(Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (HelperScripts.LayermaskContains(projectile.interactLayers, collision.gameObject.layer))
        {
            if (projectile.contactDestroy)
            {
                StopProjectile();
            }
            else
            {
                // stupid bounce stuff
                rb.AddForce(collision.GetContact(0).normal * rb.velocity.magnitude * projectile.bounceAmp);
            }
        }
    }

    public void StartProjectile(Vector3 velocity, float mass)
    {
        gameObject.SetActive(true);

        rb.mass = mass;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(velocity);

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
