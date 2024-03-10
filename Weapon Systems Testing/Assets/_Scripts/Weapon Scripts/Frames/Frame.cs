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

    #region Functionality Variables

    [NonSerialized]
    public int currentAmmo = 0;

    protected bool activationBusy = false;
    protected bool burstActivationBusy = false;

    protected const string BurstTimerKey = "BurstTimer";

    protected int burstRemaining = 0;

    #endregion

    #region Behavior Tree

    protected Node.Status status = Node.Status.RUNNING;

    public Tree frameBehavior = new Tree("Frame Behavior");

    protected Sequence activateActions;
    protected Sequence refillActions;
    protected Sequence readyActions;

    protected const string ActivateSequenceKey = "Activate";
    protected const string RefillSequenceKey = "Refill";
    protected const string ReadySequenceKey = "Ready";

    #endregion

    public abstract void Activate();

    public virtual void Initialize(Weapon connectedWeapon)
    {
        this.connectedWeapon = connectedWeapon;

        // TEMPORARY: Change later for weapon crafting
        currentStats = baseStats;

        timerManager.Add(ActivateSequenceKey, new Timer(currentStats.useTime, ActivateTimerCallback));
        timerManager.Add(BurstTimerKey, new Timer(currentStats.burstSpeed, BurstTimerCallback));

        activateActions = new Sequence(ActivateSequenceKey);
        activateActions.AddChild(new Action("BeginActivate", BeginActivate));
        activateActions.AddChild(new Action("UpdateActivate", UpdateActivate));
        activateActions.AddChild(new Action("EndActivate", EndActivate));

        refillActions = new Sequence(RefillSequenceKey);
        refillActions.AddChild(new Action("BeginRefill", BeginRefill));
        refillActions.AddChild(new Action("UpdateRefill", UpdateRefill));
        refillActions.AddChild(new Action("EndRefill", EndRefill));

        readyActions = new Sequence(ReadySequenceKey);
        readyActions.AddChild(new Action("BeginReady", BeginReady));
        readyActions.AddChild(new Action("UpdateReady", UpdateReady));
        readyActions.AddChild(new Action("EndReady", EndReady));

        frameBehavior.AddChild(readyActions);
        frameBehavior.AddChild(activateActions);
        frameBehavior.AddChild(refillActions);

        frameBehavior.StartSequence(ReadySequenceKey);

        activationBusy = false;
        burstActivationBusy = false;
}
    public virtual void UpdateFrame(float dt)
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
                    frameBehavior.StartSequence(ReadySequenceKey);
                    break;
            }

            status = frameBehavior.Check();
        }

        timerManager.IncrementTimers(dt);
    }

    #region Frame Behaviors

    protected virtual Node.Status BeginActivate() { return Node.Status.SUCCESS; }
    protected virtual Node.Status UpdateActivate() { return Node.Status.SUCCESS; }
    protected virtual Node.Status EndActivate() { return Node.Status.SUCCESS; }

    protected virtual Node.Status BeginRefill() { return Node.Status.SUCCESS; }
    protected virtual Node.Status UpdateRefill() { return Node.Status.SUCCESS; }
    protected virtual Node.Status EndRefill() { return Node.Status.SUCCESS; }

    protected virtual Node.Status BeginReady() { return Node.Status.SUCCESS; }
    protected virtual Node.Status UpdateReady() { return Node.Status.RUNNING; }
    protected virtual Node.Status EndReady() { return Node.Status.SUCCESS; }

    #endregion

    #region Timer Callbacks

    protected virtual void ActivateTimerCallback(string key) { }
    protected virtual void BurstTimerCallback(string key) { }

    #endregion

    #region Helper Scripts

    public virtual bool IsFrameActivated()
    {
        return frameBehavior.GetCurrentSequenceName() != ReadySequenceKey;
    }

    #endregion
}
public abstract class RangedFrame : Frame
{
    public LayerMask interactionLayers;

    public override void Activate()
    {
        if(frameBehavior.GetCurrentSequenceName() != ActivateSequenceKey)
        {
            frameBehavior.StartSequence(ActivateSequenceKey);
        }
    }
    protected abstract void Fire(Transform origin);

    protected override Node.Status BeginActivate()
    {
        burstRemaining = currentStats.burstLength;

        return Node.Status.SUCCESS;
    }
    protected override Node.Status UpdateActivate() 
    {
        if (burstRemaining > 0)
        {
            if(currentAmmo >= currentStats.ammoPerShot)
            {
                if (!burstActivationBusy)
                {
                    for(int i = 0; i < currentStats.shotsInSpread; i++)
                    {
                        Fire(connectedWeapon.controller.projectileSpawnPoint);
                    }

                    burstRemaining--;
                    currentAmmo -= currentStats.ammoPerShot;

                    burstActivationBusy = true;
                    timerManager.timers[BurstTimerKey].Start();
                }
            }
            else
            {
                activationBusy = true;
                timerManager.timers[ActivateSequenceKey].Start();
                return Node.Status.SUCCESS;
            }
        }
        else if (!burstActivationBusy)
        {
            activationBusy = true;
            timerManager.timers[ActivateSequenceKey].Start();
            return Node.Status.SUCCESS;
        }

        return Node.Status.RUNNING;
    }
    protected override Node.Status EndActivate()
    {
        if (!activationBusy)
            return Node.Status.SUCCESS;
        return Node.Status.RUNNING;
    }

    protected override void ActivateTimerCallback(string key) 
    {
        activationBusy = false;
    }
    protected override void BurstTimerCallback(string key) 
    {
        burstActivationBusy = false;
    }
}

[Serializable]
public class FrameStats
{
    [Header("Use Stats")]
    public bool autoUse;
    public float useTime;

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
    public int ammoPerShot;

    [Header("Fire Style Stats")]
    [Range(1f,100f)]
    public int burstLength = 1;
    [Range(0,5)]
    public float burstSpeed;

    [Header("Area Damage Stats Stats")]
    public bool doAreaDamage = false;
    public float areaDamageSize;

    [Header("Spread Shot Stats")]
    [Range(1,100)]
    public int shotsInSpread = 1;
    [Range(0,1)]
    public float spreadGrouping;
    [Range(0,1)]
    public float spreadAngle;
}
