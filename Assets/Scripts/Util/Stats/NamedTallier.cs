using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NamedTallierDictionary : SerializableDictionaryBase<string, int>
{
}

public class NamedTallier : MonoBehaviour {

    [SerializeField]
    private NamedTallierDictionary namedTallierDictionary;

    private void Awake()
    {
        namedTallierDictionary = new NamedTallierDictionary();
    }

    public void AddToNamedTally(string name)
    {
        if (namedTallierDictionary.ContainsKey(name))
        {
            namedTallierDictionary[name]++;
        }
        else
        {
            namedTallierDictionary.Add(name, 1);
        }
    }

    /* -1 if name not present in dict */
    public int GetNamedTallyFor(string name)
    {
        return namedTallierDictionary.ContainsKey(name) ? namedTallierDictionary[name] : -1;
    }
}
