using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, Interactable
{
    public virtual void ProjectileHit(ProjectileController projectile)
    {
        Debug.Log($"Hit by {projectile.parentFrame.name} for {projectile.parentFrame.currentStats.damage} damage");
    }
    public virtual void HitScanHit(Frame weaponFrame, Vector3 hitPoint) 
    {
        Debug.Log($"Hit by { weaponFrame.name } for { weaponFrame.currentStats.damage } damage");
    }
}
