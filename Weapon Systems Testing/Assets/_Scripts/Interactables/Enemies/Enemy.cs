using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, Interactable
{
    private ObjectPool objPool;

    private void Start()
    {
        objPool = ObjectPool.instance;
    }
    public virtual void ProjectileHit(ProjectileController projectile)
    {
        Debug.Log($"Hit by {projectile.parentFrame.name} for {projectile.parentFrame.currentStats.damage} damage");
    }
    public virtual void HitScanHit(Frame weaponFrame, Vector3 hitPoint) 
    {
        var popUp = objPool.GetPooledObject("Damage Pop-up");
        popUp.transform.position = hitPoint;
        popUp.SetActive(true);
        popUp.GetComponent<Popup>().StartPopup(weaponFrame.currentStats.damage.ToString());
    }
}
