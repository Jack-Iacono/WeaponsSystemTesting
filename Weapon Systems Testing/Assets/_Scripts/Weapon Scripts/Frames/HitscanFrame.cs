using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Frame/Hitscan Frame", order = 1)]
public class HitscanFrame : RangedFrame
{
    protected override void Fire(Transform origin, Vector3 angleOffset)
    {
        SoundManager.PlaySound(SoundManager.instance.shoot);

        RaycastHit hit;
        Vector3 rayOrigin = connectedWeapon.controller.cameraController.transform.position;
        // This gets a vector representing the forward direction of the camera
        // The number at the end controls the severity of the spread angle's effect on the spread grouping
        Vector3 direction = rayOrigin + connectedWeapon.controller.cameraController.transform.forward * 50;
        direction = (direction + angleOffset - rayOrigin).normalized;

        if (Physics.Raycast(new Ray(rayOrigin, direction), out hit, currentStats.range, interactionLayers))
        {
            var particle = ObjectPool.instance.GetPooledObject("HitParticle");
            particle.transform.position = hit.point;
            particle.SetActive(true);
            particle.SendMessage("StartParticle");

            Interactable col;

            if (hit.collider.gameObject.TryGetComponent(out col))
                col.HitScanHit(this, hit.point);
        }
    }
}
