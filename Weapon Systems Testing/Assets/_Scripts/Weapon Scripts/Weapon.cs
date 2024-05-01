using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class Weapon
{
    public WeaponController controller { get; private set; }

    public string name;
    public string tooltip;

    // This exists purely to allow me to add more frames easily later, not for inital use
    private const int MaxFrames = 1;
    public List<Frame> frames = new List<Frame> { null };

    [NonSerialized]
    public int activeFrame = 0;

    public List<Mod> mods = new List<Mod>();
    public int modBudget;

    public delegate void FrameChange(Frame frame, List<Mod> mods);
    public static event FrameChange OnFrameChange;

    public Weapon()
    {
        // Make the default weapon
        name = "Default Weapon";
        tooltip = "Your Basic Weapon, not sure if you should have this. Check with Jack";

        modBudget = 5;
    }

    public virtual void Initialize(WeaponController weapon) 
    {
        controller = weapon;

        // Runs through every frame in the frames array
        for(int i = 0; i < frames.Count; i++)
        {
            frames[i].Initialize(this);
            frames[i].CalculateStats(mods);

            // Overrides the weapon's normal ammo stuff
            if (frames[i].currentStats.usePrimaryAmmo)
            {
                frames[i].currentStats.readyAmmo = frames[0].currentStats.readyAmmo;
                frames[i].currentAmmo = frames[i].currentStats.readyAmmo;
            }
        }

        OnFrameChange?.Invoke(frames[activeFrame], mods);
    }
    public virtual void UpdateWeapon(float dt) 
    {
        frames[activeFrame].UpdateFrame(dt);
    }

    /// <summary>
    /// This will use the frame that is denoted by the frame index variable i.e. 1 = Primary Frame, 2 = Secondary Frame, etc...
    /// </summary>
    /// <param name="frameIndex">The index of the frame that is being used</param>
    public void UseFramePrimary()
    {
        frames[activeFrame].ActivatePrimary();
    }
    public void UseFrameRefill()
    {
        frames[activeFrame].ActivateRefill();
    }

    public virtual void CalculateStats() { }
    public virtual void ChangeFrame()
    {
        if (activeFrame < MaxFrames - 1)
            activeFrame++;
        else
            activeFrame = 0;

        OnFrameChange?.Invoke(frames[activeFrame], mods);
    }
    public virtual void SwapFrame(Frame frame)
    {
        activeFrame = 0;
        frames[0] = frame;
        Initialize(controller);
    }

}
