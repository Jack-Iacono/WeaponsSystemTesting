using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public static WeaponController instance;

    public Weapon currentWeapon;
    public CameraController cameraController;

    public Transform projectileSpawnPoint;

    public KeyCode framePrimaryKey;
    public KeyCode frameSecondaryKey;
    public KeyCode frameSwapKey;

    // Start is called before the first frame update
    void Start()
    {
        currentWeapon.Intialize(this);
    }

    // Update is called once per frame
    void Update()
    {
        if  (
            (currentWeapon.frames[currentWeapon.activeFrame].currentStats.autoUse && Input.GetKey(framePrimaryKey)) ||
            (!currentWeapon.frames[currentWeapon.activeFrame].currentStats.autoUse && Input.GetKeyDown(framePrimaryKey))
            )
        {
            currentWeapon.UseFramePrimary();
        }

        if (Input.GetKeyDown(frameSecondaryKey))
        {
            currentWeapon.UseFrameSecondary();
        }

        if (Input.GetKeyDown(frameSwapKey))
            currentWeapon.ChangeFrame();

        // Updates the current weapon
        currentWeapon.UpdateWeapon(Time.deltaTime);
    }
}
