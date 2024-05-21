using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    [SerializeField]private List<PooledObject> _objectsToPool;

    private Dictionary<string, List<GameObject>> _pooledObjects = new Dictionary<string, List<GameObject>>();

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Runs through each of the desired objects to pool
        foreach(PooledObject p in _objectsToPool)
        {
            GameObject parent = new GameObject("Pooled Object: " + p.obj.name);
            parent.transform.parent = transform;

            // Creates an empty list
            List<GameObject> obj = new List<GameObject>();

            // Populates the list with the right amount of gameobjects
            for(int i = 0; i < p.poolCount; i++)
            {
                var inst = InstantiateObject(p.obj, parent.transform);
                inst.name += " " + i;
                obj.Add(inst);
            }

            // Adds the item to the dictionary
            _pooledObjects.Add(p.obj.name, obj);
        }
    }

    /// <summary>
    /// Gets a GameObject from the pool of objects
    /// </summary>
    /// <param name="name">The name of the object that is being requested</param>
    /// <returns>The GameObject if the object is pooled, null otherwise</returns>
    public GameObject GetPooledObject(string name)
    { 
        // Find the first object that is not being used
        for(int i = 0; i < _pooledObjects[name].Count; i++)
        {
            if (!_pooledObjects[name][i].activeInHierarchy)
            {
                return _pooledObjects[name][i];
            }
        }

        // If there are no more objects, instantiate a new one and return it
        _pooledObjects[name].Add(InstantiateObject(GetPoolObject(name).obj, _pooledObjects[name][0].transform.parent));


        return _pooledObjects[name][_pooledObjects.Count - 1];
    }

    /// <summary>
    /// Gets the PooledObject with the given name
    /// </summary>
    /// <param name="name">The name of the PooledObject to find</param>
    /// <returns>The PooledObject, null if the PooledObject is not found</returns>
    private PooledObject GetPoolObject(string name)
    {
        // Run through the list of PooledObjects
        foreach(PooledObject p in _objectsToPool)
        {
            // Check if the pooled object is the one that is being seatched for
            if(p.name == name)
            {
                // Return the object
                return p;
            }
        }

        // Could not find the given object
        Debug.Log("Object " + name + " not found, check name");
        return null;
    }

    /// <summary>
    /// Creates a new GameObject to add to the pool
    /// </summary>
    /// <param name="obj">The gameobject to Instantiate</param>
    /// <param name="parent">The parent to assign to the new object</param>
    /// <returns></returns>
    private GameObject InstantiateObject(GameObject obj, Transform parent)
    {
        // Instantiate the Object
        GameObject g = Instantiate(obj, parent);

        // Run the Initialize function on the new object
        // Look into the performance cost of this later
        g.SendMessage("Initialize", SendMessageOptions.DontRequireReceiver);

        // Set the object to be inactive
        g.SetActive(false);

        // return the object
        return g;
    }
}
