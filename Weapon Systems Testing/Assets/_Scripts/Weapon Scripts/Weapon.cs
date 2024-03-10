using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Weapon/Weapon", order = 1)]
public class Weapon : ScriptableObject
{
    public WeaponController controller { get; private set; }

    public new string name;
    public string tooltip;

    // This exists purely to allow me to add more frames easily later
    private const int maxFrames = 2;
    public List<Frame> frames = new List<Frame>();

    public int modBudget;

    public virtual void Intialize(WeaponController weapon) 
    {
        controller = weapon;

        // Runs through every frame in the frames array
        for(int i = 0; i < frames.Count; i++)
        {
            frames[i].Initialize(this);

            // Overrides the weapon's normal ammo stuff
            if (frames[i].currentStats.usePrimaryAmmo)
            {
                frames[i].currentStats.readyAmmo = frames[0].currentStats.readyAmmo;
                frames[i].currentAmmo = frames[i].currentStats.readyAmmo;
            }
        }
    }
    public virtual void UpdateWeapon(float dt) 
    {
        foreach(Frame f in frames)
        {
            f.UpdateFrame(dt);
        }
    }

    /// <summary>
    /// This will use the frame that is denoted by the frame index variable i.e. 1 = Primary Frame, 2 = Secondary Frame, etc...
    /// </summary>
    /// <param name="frameIndex">The index of the frame that is being used</param>
    public void UseFrame(int frameIndex)
    {
        if (CheckActiveFrames(frameIndex))
        {
            frames[frameIndex].Activate();
        }
    }
    private bool CheckActiveFrames(int queryFrame)
    {
        foreach (Frame f in frames) { if (f.IsFrameActivated() && f != frames[queryFrame]) return false; } return true;
    }

    public virtual void CalculateStats() { }
}
