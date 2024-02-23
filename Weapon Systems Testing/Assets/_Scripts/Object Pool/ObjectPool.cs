using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    [SerializeField]private List<PooledObject> _objectsToPool;

    private Dictionary<string, List<GameObject>> _pooledObjects = new Dictionary<string, List<GameObject>>();

    // Start is called before the first frame update
    void Start()
    {
        // Create a Singleton
        if(instance != null && instance != this)
            Destroy(this);
        else
            instance = this;

        // Runs through each of the desired objects to pool
        foreach(PooledObject p in _objectsToPool)
        {
            GameObject parent = new GameObject("Pooled Object: " + p.pooledName);
            parent.transform.parent = transform;

            // Creates an empty list
            List<GameObject> obj = new List<GameObject>();

            // Populates the list with the right amount of gameobjects
            for(int i = 0; i < p.poolCount; i++)
            {
                obj.Add(InstantiateObject(p.obj, parent.transform));
            }

            // Adds the item to the dictionary
            _pooledObjects.Add(p.pooledName, obj);
        }
    }

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

    // Gets the original pooled object scriptable object
    private PooledObject GetPoolObject(string name)
    {
        foreach(PooledObject p in _objectsToPool)
        {
            if(p.name == name)
            {
                return p;
            }
        }

        Debug.Log("Object not found, check name");
        return null;
    }

    private GameObject InstantiateObject(GameObject obj, Transform parent)
    {
        GameObject g = Instantiate(obj, parent);
        g.SendMessage("Initialize", SendMessageOptions.DontRequireReceiver);
        g.SetActive(false);
        return g;
    }
}
