using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Tree : Node
{
    public Tree(string n) : base(n)
    {
        nodeName = n;
    }

    public override Status Check()
    {
        // We return the current status of the running Sequence.
        // By calling its Check(), we also potentially move through
        //  different actions.
        return Children[currentChild].Check();
    }

    public void StartSequence(string name)
    {
        // Search through the collection.
        for (int i = 0; i < Children.Count; i++)
        {
            // Does any child have the name we are looking for?
            if (Children[i].nodeName == name)
            {
                // Fail the currently running sequence.
                Children[currentChild].currentStatus = Status.FAILED;

                // Change currentChild index.
                currentChild = i;

                // Reset the internal child count.
                Children[currentChild].currentChild = 0;

                // Break from the loop.
                break;
            }
        }
    }

    public string GetCurrentSequenceName()
    {
        return Children[currentChild].nodeName;
    }
}
