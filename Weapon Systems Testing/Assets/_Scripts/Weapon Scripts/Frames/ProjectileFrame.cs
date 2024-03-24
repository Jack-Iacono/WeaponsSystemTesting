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
        SoundManager.PlaySound(SoundManager.shoot);

        RaycastHit hit;
        Vector3 rayOrigin = connectedWeapon.controller.cameraController.transform.position;

        // This gets a vector representing the forward direction of the camera
        // The number at the end controls the severity of the spread angle's effect on the spread grouping
        Vector3 direction = rayOrigin + connectedWeapon.controller.cameraController.transform.forward * 100;
        direction = (direction + angleOffset - rayOrigin).normalized;

        Ray ray = new Ray(rayOrigin, direction);
        Vector3 velocity;

        if (Physics.Raycast(ray, out hit, currentStats.range, interactionLayers))
        {
            velocity = (hit.point - origin.position).normalized * projectileCurrentStats.projectileSpeed;
        }
        else
        {
            velocity = (ray.GetPoint(10) - origin.position).normalized * projectileCurrentStats.projectileSpeed;
        }

        GameObject projectile = ObjectPool.instance.GetPooledObject(projectileCurrentStats.spawnedProjectile.name);
        projectile.GetComponent<ProjectileController>().StartProjectile(this, origin, velocity, projectileCurrentStats.projectileMass);
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
