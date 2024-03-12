using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    public Sequence(string n) : base(n)
    {
        nodeName = n;
    }

    //Once again overriding the Check method to work with this class
    public override Status Check()
    {
        // Get the current child status.
        Status childStatus = Children[currentChild].Check();

        // If the current child is running, return.
        if (childStatus == Status.RUNNING)
        {
            return Status.RUNNING;
        }

        // If the current child has failed, return.
        if (childStatus == Status.FAILED)
        {
            return Status.FAILED;
        }

        // If the current child succeeded, move to the next.
        if (childStatus == Status.SUCCESS)
        {
            // Move to the next child.
            currentChild++;

            // Have we moved through all of the children?
            if (currentChild >= Children.Count)
            {
                currentChild = 0;
                return Status.SUCCESS;
            }
        }

        // Return RUNNING as the default.
        return Status.RUNNING;
    }

    public string GetCurrentActionName()
    {
        return Children[currentChild].nodeName;
    }
}
