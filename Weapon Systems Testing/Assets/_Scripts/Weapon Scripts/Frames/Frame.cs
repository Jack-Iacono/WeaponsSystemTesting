using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Frame : ScriptableObject
{
    [Header("Use Stats")]
    public bool autoUse;
    public float useTime;
    public bool ready;

    [Header("General Stats")]
    public float damage;
    public float range;
    public float recoil;
    public float fireRate;
    public float adsTime;
    public float hipFire;

    [Header("Ammo Stats")]
    [Tooltip("This bool dictates if this frame should pull ammo from the primary pool instead of its own")]
    public bool usePrimaryAmmo;
    public int readyAmmo;
    public int reserveAmmo;
    public int currentAmmo;
    public int ammoPerShot;

    [Header("Fire Style Stats")]
    public float burstLength;
    public float burstSpeed;

    [Header("Area Damage Stats Stats")]
    public bool doAreaDamage;
    public float areaDamageSize;

    public Weapon connectedWeapon { get; private set; }

    public abstract bool Activate();
    public virtual void Initialize(Weapon connectedWeapon)
    {
        this.connectedWeapon = connectedWeapon;
    }
}
public abstract class RangedFrame : Frame
{
    public LayerMask interactionLayers;

    public override bool Activate()
    {
        // this functionality stays here in case of usePrimaryAmmo
        if (ready && currentAmmo >= ammoPerShot)
        {
            Activate();

            ready = false;
            currentAmmo -= ammoPerShot;

            return true;
        }

        return false;
    }
}
