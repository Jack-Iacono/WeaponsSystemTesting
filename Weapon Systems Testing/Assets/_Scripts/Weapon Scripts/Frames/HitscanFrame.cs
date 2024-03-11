using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Frame/Hitscan Frame", order = 1)]
public class HitscanFrame : RangedFrame
{
    protected override void Fire(Transform origin, Vector3 angleOffset)
    {
        RaycastHit hit;
        Ray camOffset = connectedWeapon.controller.cameraController.GetSightRay();

        if (Physics.Raycast(connectedWeapon.controller.cameraController.GetSightRay(), out hit, currentStats.range, interactionLayers))
        {
            var particle = ObjectPool.instance.GetPooledObject("HitParticle");
            particle.transform.position = hit.point;
            particle.SetActive(true);
            particle.SendMessage("StartParticle");
        }
    }
}
