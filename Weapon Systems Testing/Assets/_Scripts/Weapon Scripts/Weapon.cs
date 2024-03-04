using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Weapon/Weapon", order = 1)]
public class Weapon : ScriptableObject
{
    public WeaponController controller { get; private set; }

    protected TimerManager timerManager = new TimerManager();
    private List<string> frameCoolDownKeys;

    public new string name;
    public string tooltip;

    // This exists purely to allow me to add more frames easily later
    private const int maxFrames = 2;
    public List<Frame> frames = new List<Frame>();

    public int modBudget;

    public virtual void Intialize(WeaponController weapon) 
    {
        frameCoolDownKeys = new List<string>();
        controller = weapon;

        // Runs through every frame in the frames array
        for(int i = 0; i < frames.Count; i++)
        {
            frameCoolDownKeys.Add("Frame" + i.ToString());
            
            timerManager.Add(frameCoolDownKeys[i], new Timer(frames[i].currentStats.useTime, TimerEnd));

            frames[i].Initialize(this);

            if (frames[i].currentStats.usePrimaryAmmo)
                frames[i].currentStats.readyAmmo = frames[0].currentStats.readyAmmo;

            frames[i].currentStats.currentAmmo = frames[i].currentStats.readyAmmo;
        }
    }
    public virtual void UpdateWeapon(float dt) 
    {
        timerManager.IncrementTimers(dt); 
    }

    /// <summary>
    /// This will use the frame that is denoted by the frame index variable i.e. 1 = Primary Frame, 2 = Secondary Frame, etc...
    /// </summary>
    /// <param name="frameIndex">The index of the frame that is being used</param>
    public void UseFrame(int frameIndex)
    {
        if (frames[frameIndex].Activate())
        {
            timerManager.timers[frameCoolDownKeys[frameIndex]].Start();
        }
    }
    public void TimerEnd(string timer)
    {
        // Handles a use cooldown timer expiring
        if (frameCoolDownKeys.Contains(timer))
            frames[frameCoolDownKeys.IndexOf(timer)].currentStats.ready = true;
    }

    public virtual void CalculateStats() { }
}
