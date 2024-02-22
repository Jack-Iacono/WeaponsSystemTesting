using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Weapon/Ranged Weapon/Gun/Rifle", order = 1)]
public class Rifle : Gun
{
    public override void UpdateWeapon(float dt)
    {
        
    }

    public override void UsePrimary()
    {
        if(isHitScan)
        {
            RaycastHit hit;

            if (Physics.Raycast(controller.cameraController.GetSightRay(), out hit, 1000, controller.hitLayers))
            {
                Debug.Log(hit.collider.name);
            }
        }
    }
    public override void UseSecondary()
    {
        throw new System.NotImplementedException();
    }
    public override void UseTertiary()
    {
        throw new System.NotImplementedException();
    }
}
