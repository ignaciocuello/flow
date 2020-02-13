using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {

    [SerializeField]
    private GameObject objectPrefab;

    //list of objects still alive, where all object after index 'objectsInuse' are
    //inactive
    private List<GameObject> objects;
    //number of objects in 'objects' list that are active
    private int objectsInUse;

    private void Awake()
    {
        objects = new List<GameObject>();
    }

    public GameObject NewObject()
    {
        GameObject newObject;
        if (objectsInUse == objects.Count)
        {
            newObject = Instantiate(objectPrefab, transform);
            objects.Add(newObject);
        }
        else
        {
            newObject = objects[objectsInUse];
            newObject.SetActive(true);
        }

        objectsInUse++;

        return newObject;
    }

    public void DestroyObject(GameObject obj)
    {
        //if object was part of the pool
        if (objects.Remove(obj))
        {
            obj.SetActive(false);
            //add object at the end
            objects.Add(obj);

            objectsInUse--;
        }
    }

    public void DestroyAllObjects()
    {
        objectsInUse = 0;

        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }
    }

    public List<GameObject> GetObjectsInUse()
    {
        return objects.GetRange(0, objectsInUse);
    }
}
