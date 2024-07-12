using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object Pool that keeps track of instantiated objects and manages a pool of them.
/// </summary>

public class ObjectPool : MonoBehaviour
{
    private Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();

    public GameObject GetObject(GameObject gameObject) 
    {
        if(objectPool.TryGetValue(gameObject.name, out Queue<GameObject> objectList)) 
        {
            if(objectList.Count == 0) 
            {
                return CreateNewObject(gameObject);
            }
            else 
            {
                GameObject currentObject = objectList.Dequeue();
                currentObject.SetActive(true);
                return currentObject;
            }
        }
        else { return CreateNewObject(gameObject); }
    }

    private GameObject CreateNewObject(GameObject gameObject) 
    {
        GameObject newGameObject = Instantiate(gameObject);
        newGameObject.name = gameObject.name;
        return newGameObject;
    }

    public void ReturnGameObject(GameObject gameObject) 
    {
        if(objectPool.TryGetValue(gameObject.name, out Queue<GameObject> objectList)) 
        {
            objectList.Enqueue(gameObject);
        }
        else 
        {
            Queue<GameObject> newObjectQueue = new Queue<GameObject>();
            newObjectQueue.Enqueue(gameObject);
            objectPool.Add(gameObject.name, newObjectQueue);
        }

        gameObject.SetActive(false);
    }

}
