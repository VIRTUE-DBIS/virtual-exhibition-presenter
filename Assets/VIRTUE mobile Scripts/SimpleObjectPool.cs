using UnityEngine;
using System.Collections.Generic;

// A very simple object pooling class
public class SimpleObjectPool : MonoBehaviour
{
    // The prefab that this object pool returns instances of
    public GameObject prefab;
    // Collection of currently inactive instances of the prefab
    private Stack<GameObject> inactiveInstances = new Stack<GameObject>();

    // Returns an instance of the prefab
    public GameObject GetObject() 
    {
        GameObject spawnedGameObject;

        // If there is an inactive instance of the prefab ready to return, return that
        if (inactiveInstances.Count > 0) 
        {
            // Removes the instance from the collection of inactive instances
            spawnedGameObject = inactiveInstances.Pop();
        }
        // Otherwise, create a new instance
        else 
        {
            spawnedGameObject = (GameObject)GameObject.Instantiate(prefab);

            // Add the PooledObject component to the prefab so we know it came from this pool
            PooledObject pooledObject = spawnedGameObject.AddComponent<PooledObject>();
            pooledObject.pool = this;
        }

        // Put the instance in the root of the scene and enable it
        spawnedGameObject.transform.SetParent(null);
        spawnedGameObject.SetActive(true);

        // Return a reference to the instance
        return spawnedGameObject;
    }

    // Return an instance of the prefab to the pool
    public void ReturnObject(GameObject toReturn) 
    {
        PooledObject pooledObject = toReturn.GetComponent<PooledObject>();

        // If the instance came from this pool, return it to the pool
        if(pooledObject != null && pooledObject.pool == this)
        {
            // Make the instance a child of this and disable it
            toReturn.transform.SetParent(transform);
            toReturn.SetActive(false);

            // Add the instance to the collection of inactive instances
            inactiveInstances.Push(toReturn);
        }
        // Otherwise, just destroy it
        else
        {
            Debug.LogWarning(toReturn.name + " was returned to a pool it wasn't spawned from! Destroying.");
            Destroy(toReturn);
        }
    }
}

// A component that simply identifies the pool that a GameObject came from
public class PooledObject : MonoBehaviour
{
    public SimpleObjectPool pool;
}