using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Weapon currentWeapon;
    public CameraController cameraController;

    public LayerMask hitLayers;

    // Start is called before the first frame update
    void Start()
    {
        currentWeapon.SetWeaponController(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            currentWeapon.UsePrimary();
        }

        // Updates the current weapon
        currentWeapon.UpdateWeapon(Time.deltaTime);
    }
}
