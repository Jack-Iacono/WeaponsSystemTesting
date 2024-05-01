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
    public void AddMod(Mod m)
    {
        modList.Add(m);
    }
    public void RemoveMod(Mod m)
    {
        if(modList.Contains(m))
            modList.Remove(m);
    }
    public void AddFrame(Frame f)
    {
        frameList.Add(f);
    }
    public void RemoveFrame(Frame f)
    {
        if (frameList.Contains(f))
            frameList.Remove(f);
    }
    public void AddWeapon(Weapon w)
    {
        weaponList.Add(w);
    }
    public void RemoveWeapon(Weapon w)
    {
        if (weaponList.Contains(w))
            weaponList.Remove(w);
    }
    #endregion

    #region Function Methods

    public void ChangeFrame(int index)
    {
        WeaponController.instance.currentWeapon.SwapFrame(frameList[index]);
    }

    #endregion
}
