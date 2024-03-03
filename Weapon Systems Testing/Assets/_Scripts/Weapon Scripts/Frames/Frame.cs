using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Frame : ScriptableObject
{
    public FrameStats baseStats;
    public FrameStats currentStats { get; private set; }

    public Weapon connectedWeapon { get; private set; }

    public abstract bool Activate();
    public virtual void Initialize(Weapon connectedWeapon)
    {
        this.connectedWeapon = connectedWeapon;

        // TEMPORARY: Change later for weapon crafting
        currentStats = baseStats;
    }
}
public abstract class RangedFrame : Frame
{
    public LayerMask interactionLayers;

    public override bool Activate()
    {
        // this functionality stays here in case of usePrimaryAmmo
        if (currentStats.ready && currentStats.currentAmmo >= currentStats.ammoPerShot)
        {
            Fire();

            currentStats.ready = false;
            currentStats.currentAmmo -= currentStats.ammoPerShot;

            return true;
        }

        return false;
    }
    protected abstract void Fire();
}

[Serializable]
public class FrameStats
{
    [Header("Use Stats")]
    public bool autoUse;
    public float useTime;
    [NonSerialized]
    public bool ready;

    [Header("General Stats")]
    public float damage;
    public float range;
    public float recoil;
    public float adsTime;
    public float hipFire;

    [Header("Ammo Stats")]
    [Tooltip("This bool dictates if this frame should pull ammo from the primary pool instead of its own")]
    public bool usePrimaryAmmo;
    public int readyAmmo;
    public int reserveAmmo;
    [NonSerialized]
    public int currentAmmo;
    public int ammoPerShot;

    [Header("Fire Style Stats")]
    public float burstLength;
    public float burstSpeed;

    [Header("Area Damage Stats Stats")]
    public bool doAreaDamage;
    public float areaDamageSize;
}
