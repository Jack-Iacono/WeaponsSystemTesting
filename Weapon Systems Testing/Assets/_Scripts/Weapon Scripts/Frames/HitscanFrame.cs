using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Frame/Hitscan Frame", order = 1)]
public class HitscanFrame : RangedFrame
{
    protected override void Fire()
    {
        RaycastHit hit;

        if (Physics.Raycast(connectedWeapon.controller.cameraController.GetSightRay(), out hit, currentStats.range, interactionLayers))
        {
            Debug.Log(hit.collider.name);
        }
    }
}
