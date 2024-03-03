using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, Interactable
{
    public virtual void ProjectileHit(ProjectileController projectile)
    {
        Debug.Log($"Hit by {projectile.parentFrame.name} for {projectile.parentFrame.damage} damage");
    }
}
