using System.Collections;
using System.Collections.Generic;
using System;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

[Serializable]
public class OutputFrameCounterDictionary : SerializableDictionaryBase<string, string>
{
}

public class FrameCounter : MonoBehaviour {

    /* actual frame counter dictionary, list of all frames which "key" occured */
    private Dictionary<string, List<int>> frameCounterDictionary;

    /* output of frameCounterDictionary to editor in a concise form e.g. converts {1,2,3,4} to the string
     * "1-4" */
    [SerializeField]
    private OutputFrameCounterDictionary outputFrameCounterDictionary;
    
    private void Awake()
    {
        frameCounterDictionary = new Dictionary<string, List<int>>();
        outputFrameCounterDictionary = new OutputFrameCounterDictionary();
    }

    public void AddToFrameCount(string name)
    {
        if (!frameCounterDictionary.ContainsKey(name))
        {
            frameCounterDictionary.Add(name, new List<int>());
        }

        frameCounterDictionary[name].Add(GameManager.Instance.FrameCounter);

        //update editor output
        UpdateCounterOutputDictionaryString(name, frameCounterDictionary[name]);
    }

    private void UpdateCounterOutputDictionaryString(string name, List<int> frameCounter)
    {
        if (!outputFrameCounterDictionary.ContainsKey(name))
        {
            outputFrameCounterDictionary.Add(name, "");
        }

        outputFrameCounterDictionary[name] = ConvertFrameCounterToString(frameCounter);
    }

    /* coverts the list of ints 'frameCounter' into a more concise string output
     * for example [1,2,3,6,7,8,21] would be "1-3,6-8,21" */
    private string ConvertFrameCounterToString(List<int> frameCounter)
    {
        string result = "";

        int prev = -1;
        //true if a range a-b is still open i.e. only a has been written and not (-b)
        bool rangeOpen = false;
        foreach (int f in frameCounter)
        {
            if (f > prev + 1)
            {
                if (prev != -1)
                {
                    result += "-" + prev;
                    rangeOpen = false;
                }

                //prev == -1 when f is the first element of frameCounter
                result += (prev == -1) ? f.ToString() : (", " + f);
            }
            else
            {
                rangeOpen = true;
            }

            prev = f;
        }

        if (rangeOpen)
        {
            result += "-" + prev;
        }

        return result;
    }
}
