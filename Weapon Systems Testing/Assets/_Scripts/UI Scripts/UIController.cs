using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIController : MonoBehaviour
{
    protected List<ScreenController> screens = new List<ScreenController>();
    protected int currentScreen;
    protected int nextScreen;

    protected virtual void Start()
    {
        foreach (ScreenController s in screens)
        {
            s.Initialize(this);
        }
    }

    public void ScreenHideCallback()
    {
        screens[nextScreen].ShowScreen();
        currentScreen = nextScreen;
        nextScreen = -1;
    }
    public void ScreenShowCallback()
    {
        // Do nothing for now
    }

    public void ChangeToScreen(int i)
    {
        nextScreen = i;
        if (currentScreen != -1)
            screens[currentScreen].HideScreen();
        else
            ScreenHideCallback();
    }
}
