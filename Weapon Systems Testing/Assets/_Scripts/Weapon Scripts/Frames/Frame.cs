using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Frame : ScriptableObject
{
    public FrameStats baseStats;
    public FrameStats currentStats { get; private set; }

    protected TimerManager timerManager = new TimerManager();

    public Weapon connectedWeapon { get; private set; }

    #region Behavior Tree

    protected Node.Status status = Node.Status.RUNNING;

    public Tree frameBehavior = new Tree("Frame Behavior");

    protected Sequence activateActions;
    protected Sequence refillActions;
    protected Sequence readyActions;

    protected const string activateSequenceKey = "Activate";
    protected const string refillSequenceKey = "Refill";
    protected const string readySequenceKey = "Ready";

    #endregion

    public abstract bool Activate();
    public virtual void Initialize(Weapon connectedWeapon)
    {
        this.connectedWeapon = connectedWeapon;

        // TEMPORARY: Change later for weapon crafting
        currentStats = baseStats;

        timerManager.Add(activateSequenceKey, new Timer(currentStats.useTime, ActivateTimerCallback));
    }
    public virtual void UpdateFrame()
    {
        //controls the behavior of the method
        if (status == Node.Status.RUNNING)
        {
            status = frameBehavior.Check();
        }
        else if (status == Node.Status.SUCCESS)
        {
            switch (frameBehavior.GetCurrentSequenceName())
            {
                default:
                    frameBehavior.StartSequence(readySequenceKey);
                    break;
            }

            status = frameBehavior.Check();
        }
    }

    #region Firing Behaviors

    protected virtual Node.Status BeginActivate() { return Node.Status.SUCCESS; }
    protected virtual Node.Status UpdateActivate() { return Node.Status.SUCCESS; }
    protected virtual Node.Status EndActivate() { return Node.Status.SUCCESS; }

    protected virtual Node.Status BeginRefill() { return Node.Status.SUCCESS; }
    protected virtual Node.Status UpdateRefill() { return Node.Status.SUCCESS; }
    protected virtual Node.Status EndRefill() { return Node.Status.SUCCESS; }

    protected virtual Node.Status BeginReady() { return Node.Status.SUCCESS; }
    protected virtual Node.Status UpdateReady() { return Node.Status.SUCCESS; }
    protected virtual Node.Status EndReady() { return Node.Status.SUCCESS; }

    #endregion

    #region Timer Callbacks

    protected virtual void ActivateTimerCallback(string key) { }

    #endregion
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
    public bool ready = true;

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
    public int burstLength;
    public float burstSpeed;
    private int currentBurst = -1;
    private bool burstFiring = false;

    [Header("Area Damage Stats Stats")]
    public bool doAreaDamage;
    public float areaDamageSize;

    [Header("Spread Shot Stats")]
    public int shotsInSpread;
    public float spreadGrouping;
}
