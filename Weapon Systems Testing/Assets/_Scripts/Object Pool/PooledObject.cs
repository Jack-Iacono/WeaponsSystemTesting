using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PooledObject", order = 1)]
public class PooledObject : ScriptableObject
{
    [Tooltip("The name that the object will be referenced by in script")]
    public string pooledName;
    [Tooltip("The object that will be spawned")]
    public GameObject obj;
    [Tooltip("The amount of the object to prepare")]
    public int poolCount;
}
