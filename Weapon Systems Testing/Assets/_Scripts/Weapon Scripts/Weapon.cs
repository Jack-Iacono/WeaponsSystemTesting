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
        Debug.Log(frameCoolDownKeys.Count);
        controller = weapon;

        // Runs through every frame in the frames array
        for(int i = 0; i < frames.Count; i++)
        {
            frameCoolDownKeys.Add("Frame" + i.ToString());
            
            timerManager.Add(frameCoolDownKeys[i], new Timer(frames[i].useTime, TimerEnd));

            frames[i].ready = true;

            frames[i].Initialize(this);

            if (frames[i].usePrimaryAmmo)
                frames[i].readyAmmo = frames[0].readyAmmo;

            frames[i].currentAmmo = frames[i].readyAmmo;
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
        else
        {
            Debug.Log("Use Failed");
        }
    }
    public void TimerEnd(string timer)
    {
        // Handles a use cooldown timer expiring
        if (frameCoolDownKeys.Contains(timer))
            frames[frameCoolDownKeys.IndexOf(timer)].ready = true;
    }

    public virtual void CalculateStats() { }
}
