using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public static bool isGamePaused;

    public event EventHandler<bool> OnGamePause;

    void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangePauseState(!isGamePaused);
        }
    }
    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    private void ChangePauseState(bool b)
    {
        isGamePaused = b;
        Cursor.lockState = b ? CursorLockMode.None : CursorLockMode.Locked;
        OnGamePause?.Invoke(this, isGamePaused);
    }

}
