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
        // Create a singleton for this script
        if (instance != null)
            Destroy(this);
        else
            instance = this;
    }

    private void Update()
    {
        // Pause the game if the Q key is pressed
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangePauseState(!isGamePaused);
        }
    }
    private void OnDestroy()
    {
        // Nullify the instance if destoyed
        if (instance == this)
            instance = null;
    }

    private void ChangePauseState(bool b)
    {
        // Set the game's paused state and lock the cursor
        // Invoke the game paused event for all other scripts to receive
        isGamePaused = b;
        Cursor.lockState = b ? CursorLockMode.None : CursorLockMode.Locked;
        OnGamePause?.Invoke(this, isGamePaused);
    }

}
