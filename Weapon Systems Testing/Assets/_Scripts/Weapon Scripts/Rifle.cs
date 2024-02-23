using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Weapon/Ranged Weapon/Gun/Rifle", order = 1)]
public class Rifle : Gun
{
    public override void UsePrimary()
    {
        if (primaryReady)
        {
            if (isHitScan)
            {
                RaycastHit hit;

                if (Physics.Raycast(controller.cameraController.GetSightRay(), out hit, 1000, controller.hitLayers))
                {
                    Debug.Log(hit.collider.name);
                }

                primaryReady = false;
                timerManager.timers[primaryUseKey].Start();
            }
            else
            {
                GameObject projectile = ObjectPool.instance.GetPooledObject("TestProjectile");

                projectile.transform.position = controller.transform.position;
                projectile.SetActive(true);
                projectile.GetComponent<Rigidbody>().AddForce(Vector3.up * 20);
            }
        }
    }
    public override void UseSecondary()
    {
        throw new System.NotImplementedException();
    }

    public override void PrimaryTimerEnd()
    {
        primaryReady = true;
        Debug.Log("Test");
    }
}
