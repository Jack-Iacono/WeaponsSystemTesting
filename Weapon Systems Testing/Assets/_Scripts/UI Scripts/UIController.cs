using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIController : MonoBehaviour
{
    public static UIController instance;

    protected List<ScreenController> screens = new List<ScreenController>();
    protected int currentScreen = 0;
    protected int nextScreen;

    public event EventHandler OnMenuOpen;
    public event EventHandler OnMenuClose;

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;
    }
    protected virtual void Start()
    {
        for (int i = 0; i < screens.Count; i++)
        {
            screens[i].Initialize(this);

            if (i == 0)
                screens[i].ShowScreen();
            else
                screens[i].HideScreen();
        }

        screens[0].ShowScreen();

        GameController.instance.OnGamePause += OnGamePause;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public void ScreenHideCallback()
    {
        //screens[nextScreen].ShowScreen();
        //currentScreen = nextScreen;
        //nextScreen = -1;
    }
    public void ScreenShowCallback()
    {
        // Do nothing for now
    }

    public void ChangeToScreen(int i)
    {
        nextScreen = i;

        if (currentScreen != -1)
        {
            screens[currentScreen].HideScreen();
            screens[nextScreen].ShowScreen();

            currentScreen = nextScreen;
            nextScreen = -1;
        }
        else
            ScreenHideCallback();
    }

    #region Events

    private void OnGamePause(object sender, bool e)
    {
        if (e)
            ChangeToScreen(1);
        else
            ChangeToScreen(0);
    }

    protected void NotifyMenuOpen()
    {
        OnMenuOpen?.Invoke(this, EventArgs.Empty);
    }
    protected void NotifyMenuClose()
    {
        OnMenuClose?.Invoke(this, EventArgs.Empty);
    }

    #endregion
}
