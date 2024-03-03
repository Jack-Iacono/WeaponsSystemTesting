interface Interactable
{
    public virtual void InteractUse() { }
    public virtual void InteractHit() { }

    public virtual void ProjectileHit(ProjectileController projectile) { }
}
