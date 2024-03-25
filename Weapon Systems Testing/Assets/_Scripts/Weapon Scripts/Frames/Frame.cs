using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public abstract class Frame : ScriptableObject
{
    public string frameName;
    [TextArea]
    public string tooltip;

    public FrameStats baseStats;
    public FrameStats currentStats { get; protected set; }

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

    #region Events

    // Could be optimized depending on scenario, but I'll do this if I need to
    public delegate void FrameUpdate(Frame frame);
    public static event FrameUpdate OnFrameDataChange;

    #endregion

    public abstract void ActivatePrimary();
    public abstract void ActivateSecondary();

    public virtual void Initialize(Weapon connectedWeapon)
    {
        this.connectedWeapon = connectedWeapon;

        // Initialize the timers
        timerManager.Add(ActivateSequenceKey, new Timer(1, ActivateTimerCallback));
        timerManager.Add(BurstTimerKey, new Timer(1, BurstTimerCallback));
        timerManager.Add(RefillSequenceKey, new Timer(1, RefillTimerCallback));
        
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

    #region Stat Related Methods

    public abstract void CalculateStats(List<Mod> mods);

    #endregion

    #region Event Methods

    protected void NotifyFrameDataChange()
    {
        OnFrameDataChange?.Invoke(this);
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
            frameBehavior.StartSequence(RefillSequenceKey);
        }
    }
    protected abstract void Fire(Transform origin, Vector3 angleOffset);

    #region Behavior Methods

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

                    NotifyFrameDataChange();
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
                NotifyFrameDataChange();
                return Node.Status.SUCCESS;
            }
            else
            {
                currentAmmo = Mathf.Clamp(currentAmmo + currentStats.refillAmount, 0, currentStats.readyAmmo);

                refillBusy = true;
                timerManager.timers[RefillSequenceKey].Start();

                NotifyFrameDataChange();
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

    #endregion

    public override void CalculateStats(List<Mod> mods)
    {
        // Gets the stats that derive from the frame itself
        currentStats = new FrameStats();
        currentStats += baseStats;

        foreach(Mod m in mods)
        {
            Debug.Log($"Adding {m.modName} to the frame");
            currentStats += m.frameStats;
        }

        timerManager.timers[ActivateSequenceKey].SetMaxTime(currentStats.useTime);
        timerManager.timers[BurstTimerKey].SetMaxTime(currentStats.burstSpeed);
        timerManager.timers[RefillSequenceKey].SetMaxTime(currentStats.refillSpeed);

        currentAmmo = currentStats.readyAmmo;
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
    public int maxReserveAmmo;
    public int ammoPerShot;

    [Header("Refill Stats")]
    public float refillSpeed;
    [Tooltip("How much should be refilled per tick, 0 = all at once")]
    public int refillAmount;

    [Header("Fire Style Stats")]
    [Range(0f,100f)]
    public int burstLength;
    [Range(-5,5)]
    public float burstSpeed;

    [Header("Area Damage Stats Stats")]
    public bool doAreaDamage = false;
    public float areaDamageSize;

    [Header("Spread Shot Stats")]
    [Range(0,100)]
    public int shotsInSpread;
    [Range(-20,20)]
    [Tooltip("This number is representative of HALF of the spread angle, not the full cone")]
    public float spreadAngle;

    public static FrameStats operator +(FrameStats f1, FrameStats f2)
    {
        f1.autoUse = f2.autoUse ? true: f1.autoUse;
        f1.useTime = Mathf.Clamp(f1.useTime + f2.useTime, 0, float.MaxValue);

        f1.damage = Mathf.Clamp(f1.damage + f2.damage, 0.01f, float.MaxValue);
        f1.range = Mathf.Clamp(f1.range + f2.range, 0.01f, float.MaxValue);
        f1.recoil = Mathf.Clamp(f1.recoil + f2.recoil, 0.001f, float.MaxValue);
        f1.adsTime = Mathf.Clamp(f1.adsTime + f2.adsTime, 0.001f, float.MaxValue);
        f1.hipFire = Mathf.Clamp(f1.hipFire + f2.hipFire, 0.01f, float.MaxValue);

        f1.usePrimaryAmmo = f2.usePrimaryAmmo ? true : f1.usePrimaryAmmo;
        f1.readyAmmo = Mathf.Clamp(f1.readyAmmo + f2.readyAmmo, 1, int.MaxValue);
        f1.maxReserveAmmo = Mathf.Clamp(f1.maxReserveAmmo + f2.maxReserveAmmo, 1, int.MaxValue);
        f1.ammoPerShot = Mathf.Clamp(f1.ammoPerShot + f2.ammoPerShot, 1, int.MaxValue);

        f1.refillSpeed = Mathf.Clamp(f1.refillSpeed + f2.refillSpeed, 0.001f, float.MaxValue);
        f1.refillAmount = Mathf.Clamp(f1.refillAmount + f2.refillAmount, 1, int.MaxValue);

        f1.burstLength = Mathf.Clamp(f1.burstLength + f2.burstLength, 1, int.MaxValue);
        f1.burstSpeed = Mathf.Clamp(f1.burstSpeed + f2.burstSpeed, 0.001f, float.MaxValue);

        f1.doAreaDamage = f2.doAreaDamage ? true : f1.doAreaDamage;
        f1.areaDamageSize = Mathf.Clamp(f1.areaDamageSize + f2.areaDamageSize, 0.001f, float.MaxValue);

        f1.shotsInSpread = Mathf.Clamp(f1.shotsInSpread + f2.shotsInSpread, 1, int.MaxValue);
        f1.spreadAngle = Mathf.Clamp(f1.spreadAngle + f2.spreadAngle, 0.001f, float.MaxValue);

        return f1;
    }
}
