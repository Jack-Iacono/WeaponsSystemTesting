using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIController : UIController
{
    public ScreenController hudScreenController;
    public ScreenController pauseScreenController;
    public ScreenController craftScreenController;

    // Start is called before the first frame update
    protected override void Start()
    {
        // Add the screens to the dictionary
        screens.Add(hudScreenController);
        screens.Add(pauseScreenController);
        screens.Add(craftScreenController);

        base.Start();
    }
}
