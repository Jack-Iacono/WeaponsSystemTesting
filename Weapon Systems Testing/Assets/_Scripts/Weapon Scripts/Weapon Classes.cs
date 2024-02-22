using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : ScriptableObject
{
    protected WeaponController controller;

    public new string name;
    public string tooltip;

    public float useTime;

    public int modBudget;

    public abstract void UsePrimary();
    public abstract void UseSecondary();
    public abstract void UseTertiary();

    public abstract void UpdateWeapon(float dt);

    // Attachment Get/Set Methods

    public void SetWeaponController(WeaponController w)
    {
        controller = w;
    }
}
public abstract class RangedWeapon : Weapon
{
    [Header("Basic Gun Stats")]
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
}
public abstract class Gun : RangedWeapon
{
    // Implement Gun Specific Stuff
}