using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperScripts
{
    public static float ClampAngle(float f)
    {
        if (f > 360)
        {
            return 360 % f;
        }
        else if (f < 0)
        {
            return 360 + (360 % f);
        }
        else
        {
            return f;
        }
    }
}
