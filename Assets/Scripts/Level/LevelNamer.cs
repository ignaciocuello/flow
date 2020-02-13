using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LevelNamer : MonoBehaviour {

    [SerializeField]
    private string prefix;
    [SerializeField]
    private string remove;

    private Dictionary<string, int> nameCounts;

    private void Update () {
        nameCounts = new Dictionary<string, int>();
        if (prefix != null && prefix.Length > 0)
        {
            RenameChildren(gameObject);
        }
	}

    public void RenameRecursive(GameObject gameObj)
    {
        //if object is not a folder and name doesn't start with prefix
        if (gameObj
            .GetComponents<Component>().Length > 2 && !gameObj.name.StartsWith(prefix))
        {
            RenameGameObject(gameObj);
        }

        RenameChildren(gameObj);
    }

    public void RenameChildren(GameObject gameObj)
    {
        for (int i = 0; i < gameObj.transform.childCount; i++)
        {
            RenameRecursive(gameObj.transform.GetChild(i).gameObject);
        }
    }

    private void RenameGameObject(GameObject gameObj)
    {
        string name = prefix + "_" + gameObj.name;

        if (!nameCounts.ContainsKey(name))
        {
            nameCounts[name] = 0;
        }

        if (remove != null && remove.Length > 0)
        {
            string newName = name.Replace(remove, "");
            if (newName != name)
            {
                int index = newName.LastIndexOf("_");
                if (index != -1)
                {
                    newName = newName.Remove(index, newName.Length - index);
                }

                gameObj.name = newName + "_" + ++nameCounts[name];
                return;
            }
        }

        gameObj.name = name + "_" + ++nameCounts[name];
    }

}
