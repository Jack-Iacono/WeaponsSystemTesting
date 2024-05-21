using UnityEngine;

/// <summary>
/// This is an interface that dictates what functionality an interactable item will have
/// </summary>
interface Interactable
{
    // Called when the object is interacted with
    public virtual void InteractUse() { }
    // Called when the object is hit by something
    public virtual void InteractHit() { }
    // Called when a projectile attack hits this object
    public virtual void ProjectileHit(ProjectileController projectile) { }
    // Called when a hitscan attack hits this object
    public virtual void HitScanHit(Frame weaponFrame, Vector3 hitPoint) { }
}
