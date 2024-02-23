using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : ScriptableObject
{
    protected WeaponController controller;

    public new string name;
    public string tooltip;

    public bool autoUse;

    public float primaryUseTime;
    public float secondaryUseTime;
    protected const string primaryUseKey = "primaryUseTime";
    protected const string secondaryUseKey = "secondaryUseTime";
    protected bool primaryReady = true;
    protected bool secondaryReady = true;

    public int modBudget;

    protected TimerManager timerManager = new TimerManager();

    public virtual void Intialize(WeaponController weapon) 
    {
        controller = weapon;

        timerManager.Add(primaryUseKey, new Timer(primaryUseTime, PrimaryTimerEnd));
        timerManager.Add(secondaryUseKey, new Timer(secondaryUseTime, SecondaryTimerEnd));

        primaryReady = true;
        secondaryReady = true;
    }
    public virtual void UpdateWeapon(float dt) { timerManager.IncrementTimers(dt); }

    public abstract void UsePrimary();
    public abstract void UseSecondary();

    public virtual void PrimaryTimerEnd() { }
    public virtual void SecondaryTimerEnd() { }
}

public abstract class RangedWeapon : Weapon
{
    [Header("Basic Ranged Weapon Stats")]
    public float damage;
    public float range;
    public float recoil;
    public float fireRate;
    public float adsTime;
    public float hipFire;

    [Header("Magazine Stats")]
    public float magazineSize;
    public float reserveAmmo;

    [Header("Burst Stats")]
    public float burstLength;
    public float burstSpeed;

    [Header("Area Damage Stats")]
    public bool doAreaDamage;
    public float areaDamageSize;

    [Header("Firing Mode Stats")]
    public bool isHitScan;
    public GameObject spawnedProjectile;
    public float projectileSpeed;
    public float projectileMass;
}
public abstract class Gun : RangedWeapon
{
    public override void UsePrimary()
    {
        if (primaryReady)
        {
            if (isHitScan)
            {
                RaycastHit hit;

                if (Physics.Raycast(controller.cameraController.GetSightRay(), out hit, 1000, controller.hitLayers))
                {
                    Debug.Log(hit.collider.name);
                }
            }
            else
            {
                GameObject projectile = ObjectPool.instance.GetPooledObject("TestProjectile");

                projectile.transform.position = controller.projectileSpawnPoint.position;
                projectile.SetActive(true);

                var rb = projectile.GetComponent<Rigidbody>();
                rb.mass = projectileMass;
                rb.AddForce(controller.cameraController.transform.forward * projectileSpeed);
            }

            primaryReady = false;
            timerManager.timers[primaryUseKey].Start();
        }
    }
    public override void UseSecondary()
    {
        throw new System.NotImplementedException();
    }

    public override void PrimaryTimerEnd()
    {
        primaryReady = true;
    }
}