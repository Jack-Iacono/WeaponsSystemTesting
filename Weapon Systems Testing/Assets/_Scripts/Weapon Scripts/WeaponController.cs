using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Weapon currentWeapon;
    public CameraController cameraController;

    public Transform projectileSpawnPoint;

    public List<KeyCode> frameKeys = new List<KeyCode>();

    // Start is called before the first frame update
    void Start()
    {
        currentWeapon.Intialize(this);
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < frameKeys.Count; i++)
        {
            if(
                (currentWeapon.frames[i].autoUse && Input.GetKey(frameKeys[i])) || 
                (!currentWeapon.frames[i].autoUse && Input.GetKeyDown(frameKeys[i]))
              )
            {
                currentWeapon.UseFrame(i);
                break;
            }
        }

        // Updates the current weapon
        currentWeapon.UpdateWeapon(Time.deltaTime);
    }
}
