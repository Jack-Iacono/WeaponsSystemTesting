using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Projectile", order = 1)]
public class Projectile : ScriptableObject
{
    private ProjectileController controller;

    public float projectileLifetime;

    public enum CollisionAction { DESTROY, BOUNCE };
    public CollisionAction collisionAction;
    public float bounceAmp;

    public LayerMask interactLayers;

    private const string lifetimeKey = "LifetimeTimer";
    private TimerManager timerManager = new TimerManager();

    public void ProjectileInitialize(ProjectileController controller)
    {
        this.controller = controller;

        if(!timerManager.ContainsKey(lifetimeKey))
            timerManager.Add(lifetimeKey, new Timer(projectileLifetime, ProjectileLifetimeEnd));
    }
    public void ProjectileUpdate(float dt)
    {
        timerManager.IncrementTimers(dt);
    }

    public void StartProjectile()
    {
        timerManager.timers[lifetimeKey].Start();
    }
    public void StopProjectile()
    {
        timerManager.timers[lifetimeKey].Stop();
    }
    public void ProjectileLifetimeEnd(string timer)
    {
        controller.ProjectileLifetimeEnd();
    }
}
