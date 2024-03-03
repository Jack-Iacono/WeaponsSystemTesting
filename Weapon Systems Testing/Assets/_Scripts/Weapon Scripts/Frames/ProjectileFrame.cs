using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Frame/Projectile Frame", order = 1)]
public class ProjectileFrame : RangedFrame
{
    [Header("Projectile Frame Varibales")]
    public GameObject spawnedProjectile;
    public float projectileSpeed;
    public float projectileMass;

    public override bool Activate()
    {
        GameObject projectile = ObjectPool.instance.GetPooledObject(spawnedProjectile.name);
        projectile.GetComponent<ProjectileController>().StartProjectile(this, connectedWeapon.controller.projectileSpawnPoint, connectedWeapon.controller.cameraController.transform.forward * projectileSpeed, projectileMass);

        return true;
    }
}
