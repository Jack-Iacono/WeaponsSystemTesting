using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenController : MonoBehaviour
{
    protected UIController parentController;

    public virtual void Initialize(UIController parent) 
    { 
        parentController = parent;
    }

    public virtual void ShowScreen()
    {
        gameObject.SetActive(false);

        parentController.ScreenHideCallback();
    }
    public virtual void HideScreen()
    {
        gameObject.SetActive(true);

        parentController.ScreenShowCallback();
    }
}
