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
        // Initializes every screen and hides all but the currently active screen
        for (int i = 0; i < screens.Count; i++)
        {
            screens[i].Initialize(this);

            if (i == 0)
                screens[i].ForceShowScreen();
            else
                screens[i].ForceHideScreen();
        }

        screens[0].ShowScreen();

        // Registers this script with the OnGamePause event
        GameController.instance.OnGamePause += OnGamePause;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public void ScreenHideCallback()
    {
        
    }
    public void ScreenShowCallback()
    {
        
    }

    /// <summary>
    /// Changes the currently active UI screen
    /// </summary>
    /// <param name="i">The index of the screen to change to</param>
    public void ChangeToScreen(int i)
    {
        nextScreen = i;

        // Ensures that the current screen isn't null
        if (currentScreen != -1)
        {
            // Hides the previous screen and shows the new screen
            screens[currentScreen].HideScreen();
            screens[nextScreen].ShowScreen();

            // sets the current screen to the new screen
            currentScreen = nextScreen;
            nextScreen = -1;
        }
        else
            ScreenHideCallback();
    }

    #region Events

    protected virtual void OnGamePause(object sender, bool e)
    {
        // Shows the pause menu if paused, else show the hud
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
