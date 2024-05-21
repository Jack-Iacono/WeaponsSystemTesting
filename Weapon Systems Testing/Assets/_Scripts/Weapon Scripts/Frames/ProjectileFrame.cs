using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Frame/Projectile Frame", order = 1)]
public class ProjectileFrame : RangedFrame
{
    public ProjectileFrameStats projectileBaseStats;
    public ProjectileFrameStats projectileCurrentStats { get; private set; }

    public override void Initialize(Weapon connectedWeapon)
    {
        base.Initialize(connectedWeapon);

        // TEMPORARY: Change later for weapon crafting
        projectileCurrentStats = projectileBaseStats;
    }
    protected override void Fire(Transform origin, Vector3 angleOffset)
    {
        SoundManager.PlaySound(SoundManager.instance.shoot);

        RaycastHit hit;
        Vector3 rayOrigin = connectedWeapon.controller.cameraController.transform.position;

        // This gets a vector representing the forward direction of the camera
        // The number at the end controls the severity of the spread angle's effect on the spread grouping
        Vector3 direction = rayOrigin + connectedWeapon.controller.cameraController.transform.forward * 100;
        direction = (direction + angleOffset - rayOrigin).normalized;

        Ray ray = new Ray(rayOrigin, direction);
        Vector3 currentMove;

        // The below code is used to mimic accuracy despite the projectiles coming from a gun that is not necessarily pointing
        //  in the direction that the projectiles should move

        // If the reticle is on an object, make the projectile move in that general direction
        if (Physics.Raycast(ray, out hit, currentStats.range, interactionLayers))
        {
            currentMove = (hit.point - origin.position).normalized * projectileCurrentStats.projectileSpeed;
        }
        // If not, have the projectile move in a straight line
        else
        {
            currentMove = (ray.GetPoint(10) - origin.position).normalized * projectileCurrentStats.projectileSpeed;
        }

        // Set up the projectile
        GameObject projectile = ObjectPool.instance.GetPooledObject(projectileCurrentStats.spawnedProjectile.name);
        projectile.GetComponent<ProjectileController>().StartProjectile(this, origin, currentMove, projectileCurrentStats.projectileMass);
    }
}

[Serializable]
public class ProjectileFrameStats
{
    [Header("Projectile Frame Varibales")]
    public GameObject spawnedProjectile;
    public float projectileSpeed;
    public float projectileMass;
}
