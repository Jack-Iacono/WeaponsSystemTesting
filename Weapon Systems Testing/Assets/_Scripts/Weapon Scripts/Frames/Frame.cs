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
    protected bool refillBusy = false;

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

    public abstract void ActivatePrimary();
    public abstract void ActivateSecondary();

    public virtual void Initialize(Weapon connectedWeapon)
    {
        this.connectedWeapon = connectedWeapon;

        // TEMPORARY: Change later for weapon crafting
        currentStats = baseStats;

        currentAmmo = currentStats.readyAmmo;

        timerManager.Add(ActivateSequenceKey, new Timer(currentStats.useTime, ActivateTimerCallback));
        timerManager.Add(BurstTimerKey, new Timer(currentStats.burstSpeed, BurstTimerCallback));
        timerManager.Add(RefillSequenceKey, new Timer(currentStats.refillSpeed, RefillTimerCallback));

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
    protected virtual void RefillTimerCallback(string key) { }

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

    public override void ActivatePrimary()
    {
        if(frameBehavior.GetCurrentSequenceName() != ActivateSequenceKey)
        {
            frameBehavior.StartSequence(ActivateSequenceKey);
        }
    }
    public override void ActivateSecondary()
    {
        if (frameBehavior.GetCurrentSequenceName() == ReadySequenceKey)
        {
            Debug.Log("Start Refill");
            frameBehavior.StartSequence(RefillSequenceKey);
        }
    }
    protected abstract void Fire(Transform origin, Vector3 angleOffset);

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
                        if(i == 0)
                            Fire(connectedWeapon.controller.projectileSpawnPoint, Vector3.zero);
                        else
                            Fire(connectedWeapon.controller.projectileSpawnPoint, new Vector3(UnityEngine.Random.Range(-currentStats.spreadAngle, currentStats.spreadAngle), UnityEngine.Random.Range(-currentStats.spreadAngle, currentStats.spreadAngle), UnityEngine.Random.Range(-currentStats.spreadAngle, currentStats.spreadAngle)));
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
        if (!activationBusy || currentAmmo == 0)
        {
            activationBusy = false;
            return Node.Status.SUCCESS;
        }

        return Node.Status.RUNNING;
    }

    protected override Node.Status BeginRefill()
    {
        refillBusy = true;
        timerManager.timers[RefillSequenceKey].Start();
        return Node.Status.SUCCESS;
    }
    protected override Node.Status UpdateRefill()
    {
        if(currentAmmo < currentStats.readyAmmo && !refillBusy)
        {
            if(currentStats.refillAmount == 0)
            {
                currentAmmo = currentStats.readyAmmo;
                return Node.Status.SUCCESS;
            }
            else
            {
                currentAmmo = Mathf.Clamp(currentAmmo + currentStats.refillAmount, 0, currentStats.readyAmmo);
                Debug.Log("Current Ammo: " + currentAmmo);
            }
        }

        if (currentAmmo == currentStats.readyAmmo)
            return Node.Status.SUCCESS;

        return Node.Status.RUNNING;
    }
    protected override Node.Status EndRefill()
    {
        refillBusy = false;
        return Node.Status.SUCCESS;
    }

    protected override void ActivateTimerCallback(string key) 
    {
        activationBusy = false;
    }
    protected override void BurstTimerCallback(string key) 
    {
        burstActivationBusy = false;
    }
    protected override void RefillTimerCallback(string key)
    {
        refillBusy = false;
    }
}

[Serializable]
public class FrameStats
{
    public string name;
    [TextArea]
    public string tooltip;

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
    public int maxReserveAmmo;
    public int ammoPerShot;

    [Header("Refill Stats")]
    public float refillSpeed;
    [Tooltip("How much should be refilled per tick, 0 = all at once")]
    public int refillAmount;

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
    [Range(0,20)]
    [Tooltip("This number is representative of HALF of the spread angle, not the full cone")]
    public float spreadAngle;
}
