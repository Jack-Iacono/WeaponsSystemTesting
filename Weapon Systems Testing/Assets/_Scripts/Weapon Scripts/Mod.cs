using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mod
{
    public string name;
    public string tooltip;

    public int equipCost;

    public abstract void GetStats();
}
