using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

//The colon indicates inheritance
public class Action : Node
{
    // Define a kind of function able
    //  to be passed based on a type named Task.
    public delegate Status Task();

    // Use the named type to save a method to run.
    public Task methodToRun = null;

    public Action(string n, Task m) : base(n)
    {
        nodeName = n;
        methodToRun = m;
    }

    //Using the virtual word in Node allows for this method to be ovewritten
    public override Status Check()
    {
        //If there is a method, run it. Else return the FAILED status.
        if (methodToRun != null)
        {
            return methodToRun();
        }
        else
        {
            return Status.FAILED;
        }

    }
}
