using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    public TMP_Text text;

    public float lifetime = 1;
    public float movementAmp = 1;
    public float gravity = 1;

    [Header("Movement Constrains")]
    public Vector2 xVar = Vector2.one;
    public Vector2 yVar = Vector2.one;
    public Vector2 zVar = Vector2.one;

    private Vector3 currentVelocity = Vector3.zero;
    private float currentLifetime = 0;

    private GameObject playerCam;

    private bool isActive = false;

    public void Initialize()
    {
        playerCam = CameraController.instance.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(isActive && !GameController.isGamePaused)
        {
            transform.position += currentVelocity * Time.deltaTime;
            currentVelocity.y -= gravity * Time.deltaTime;

            currentLifetime -= Time.deltaTime;
            if (currentLifetime <= 0)
            {
                StopPopup();
            }
        }
    }

    public void StartPopup()
    {
        currentLifetime = lifetime;
        currentVelocity =
            UnityEngine.Random.Range(xVar.x, xVar.y) * transform.right * movementAmp +
            UnityEngine.Random.Range(yVar.x, yVar.y) * Vector3.up * movementAmp +
            UnityEngine.Random.Range(zVar.x, zVar.y) * transform.forward * movementAmp
            ;

        isActive = true;
    }
    public void StartPopup(string s)
    {
        text.text = s;

        transform.LookAt(playerCam.transform.position);

        StartPopup();
    }

    public void StopPopup()
    {
        isActive = false;
        gameObject.SetActive(false);
    }
}
