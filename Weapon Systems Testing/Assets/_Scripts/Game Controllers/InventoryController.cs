using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public static InventoryController instance;

    public List<Mod> modList = new List<Mod>();
    public List<Frame> frameList = new List<Frame>();
    public List<Weapon> weaponList { get; private set; } = new List<Weapon>();

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(this);
    }
    private void OnDestroy()
    {
        if(instance == this)
            instance = null;
    }

    private void Start()
    {
        // TEMPORARY, remove when I have all weapons added
        weaponList.Add(new Weapon());
    }

    #region Inventory Access Methods

    /// <summary>
    /// Adds a mod to the list of mods within the player's inventory
    /// </summary>
    /// <param name="m">The mod that is being added</param>
    public void AddMod(Mod m)
    {
        modList.Add(m);
    }
    /// <summary>
    /// Removes a mod to the list of mods within the player's inventory
    /// </summary>
    /// <param name="m">The mod that is being removed</param>
    public void RemoveMod(Mod m)
    {
        // If the player has the given mode, remove it
        if(modList.Contains(m))
            modList.Remove(m);
    }
    /// <summary>
    /// Adds a frame to the list of frames within the player's inventory
    /// </summary>
    /// <param name="m">The frame that is being added</param>
    public void AddFrame(Frame f)
    {
        frameList.Add(f);
    }
    /// <summary>
    /// Removes a frame to the list of frames within the player's inventory
    /// </summary>
    /// <param name="m">The frame that is being removed</param>
    public void RemoveFrame(Frame f)
    {
        // Checks to see if the player has this frame and, if so, removes it
        if (frameList.Contains(f))
            frameList.Remove(f);
    }
    /// <summary>
    /// Adds a weapon to the list of weapons within the player's inventory
    /// </summary>
    /// <param name="m">The weapon that is being added</param>
    public void AddWeapon(Weapon w)
    {
        weaponList.Add(w);
    }
    /// <summary>
    /// Removes a weapon to the list of weapons within the player's inventory
    /// </summary>
    /// <param name="m">The weapon that is being removed</param>
    public void RemoveWeapon(Weapon w)
    {
        if (weaponList.Contains(w))
            weaponList.Remove(w);
    }
    #endregion

    #region Function Methods

    /// <summary>
    /// Changes the frame on the currently equipped weapon
    /// </summary>
    /// <param name="index">The index of the frame to swap</param>
    public void ChangeFrame(int index)
    {
        WeaponController.instance.currentWeapon.SwapFrame(frameList[index]);
    }

    #endregion
}
