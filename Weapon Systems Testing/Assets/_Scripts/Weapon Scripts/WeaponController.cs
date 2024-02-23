using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Weapon currentWeapon;
    public CameraController cameraController;

    public Transform projectileSpawnPoint;

    public LayerMask hitLayers;

    // Start is called before the first frame update
    void Start()
    {
        currentWeapon.Intialize(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWeapon.autoUse && Input.GetKey(KeyCode.Mouse0))
        {
            currentWeapon.UsePrimary();
        }
        else if(!currentWeapon.autoUse && Input.GetKeyDown(KeyCode.Mouse0))
        {
            currentWeapon.UsePrimary();
        }

        // Updates the current weapon
        currentWeapon.UpdateWeapon(Time.deltaTime);
    }
}
