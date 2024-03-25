using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public static bool isPaused;

    public event EventHandler<bool> OnGamePause;

    void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;

        Debug.Log("Instance Set");
    }

    // Start is called before the first frame update
    void Start()
    {
        UIController.instance.OnMenuOpen += OnMenuOpen;
        UIController.instance.OnMenuClose += OnMenuClose;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isPaused = !isPaused;
            OnGamePause?.Invoke(this, isPaused);
        }
    }

    private void OnDestroy()
    {
        instance = null;
    }

    #region Events
    private void OnMenuClose(object sender, EventArgs e)
    {
        isPaused = false;
    }
    private void OnMenuOpen(object sender, EventArgs e)
    {
        isPaused = true;
    }
    #endregion
}
