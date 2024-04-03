using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Mod/ProjectileFrameMod", order = 1)]
public class ProjectileFrameMod : Mod
{
    [SerializeField]
    public ProjectileFrameStats projectileFrameStats;
}
