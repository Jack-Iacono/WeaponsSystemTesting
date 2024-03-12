using UnityEngine;

interface Interactable
{
    public virtual void InteractUse() { }
    public virtual void InteractHit() { }

    public virtual void ProjectileHit(ProjectileController projectile) { }
    public virtual void HitScanHit(Frame weaponFrame, Vector3 hitPoint) { }
}
