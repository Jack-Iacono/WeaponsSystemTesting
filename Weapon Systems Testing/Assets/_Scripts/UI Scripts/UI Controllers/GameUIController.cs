using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIController : UIController
{
    public ScreenController hudScreenController;

    // Start is called before the first frame update
    protected override void Start()
    {
        // Add the screens to the dictionary
        screens.Add(hudScreenController);

        base.Start();

        ChangeToScreen(0);
    }
}
