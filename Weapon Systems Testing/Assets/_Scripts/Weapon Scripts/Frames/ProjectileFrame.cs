using System;
using System.Collections;
using System.Collections.Generic;
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
    protected override void Fire(Transform origin)
    {
        GameObject projectile = ObjectPool.instance.GetPooledObject(projectileCurrentStats.spawnedProjectile.name);
        projectile.GetComponent<ProjectileController>().StartProjectile(this, origin, origin.transform.forward * projectileCurrentStats.projectileSpeed, projectileCurrentStats.projectileMass);
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
