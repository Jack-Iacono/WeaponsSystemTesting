using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public static WeaponController instance;

    [NonSerialized]
    public Weapon currentWeapon = new Weapon();
    public CameraController cameraController;

    public Transform projectileSpawnPoint;

    public KeyCode framePrimaryKey;
    public KeyCode frameRefillKey;
    public KeyCode frameSwapKey;

    private void Awake()
    {
        instance = this;
    }
    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        // TEMPORARY: Remove once database is setup
        currentWeapon.frames[0] = WeaponPartStore.instance.frames[0];

        currentWeapon.Initialize(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameController.isGamePaused)
        {
            if (currentWeapon.frames[currentWeapon.activeFrame] != null)
            {
                if (
                (currentWeapon.frames[currentWeapon.activeFrame].currentStats.autoUse && Input.GetKey(framePrimaryKey)) ||
                (!currentWeapon.frames[currentWeapon.activeFrame].currentStats.autoUse && Input.GetKeyDown(framePrimaryKey))
                )
                {
                    currentWeapon.UseFramePrimary();
                }

                if (Input.GetKeyDown(frameRefillKey))
                {
                    currentWeapon.UseFrameRefill();
                }

                if (Input.GetKeyDown(frameSwapKey))
                    currentWeapon.ChangeFrame();

                // Updates the current weapon
                currentWeapon.UpdateWeapon(Time.deltaTime);
            }
        }
    }
}
