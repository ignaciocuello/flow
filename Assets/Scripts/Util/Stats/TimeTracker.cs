using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;
using System.IO;

[Serializable]
public class TimeStatsDictionary : SerializableDictionaryBase<string, TimeStats>
{
}

public class TimeTracker : MonoBehaviour {

    private const string PATH = "Statistics";

    [SerializeField]
    private TimeStatsDictionary timeDictionary;

	private void Awake()
    {
        timeDictionary = new TimeStatsDictionary();
    }

    public void StartMeasuring(string name)
    {
        TimeStats stats = new TimeStats();

        if (timeDictionary.ContainsKey(name))
        {
            timeDictionary[name] = stats;
        }
        else
        {
            timeDictionary.Add(name, stats);
        }
    }

    public bool MeasurementExists(string name)
    {
        return timeDictionary.ContainsKey(name);
    }

    public bool UpdateMeasurementIfExists(string name)
    {
        if (MeasurementExists(name))
        {
            UpdateMeasurement(name);
            return true;
        }

        return false;
    }

    public void UpdateMeasurement(string name)
    {
        timeDictionary[name].UpdateStats();
    }

    public TimeStats GetMeasurement(string name)
    {
        return timeDictionary[name];
    }

    public void Write(string fileName)
    {
        string csv = ToCSVString();
        string path = GetPathFromName(fileName);

        GameManager.Instance.DataWriter.WriteAllText(path, csv);
    }

    public string GetPathFromName(string fileName)
    {
        return Path.Combine(Application.streamingAssetsPath, Path.Combine(PATH, fileName));
    }

    private string ToCSVString()
    {
        string csv = "Name, " + TimeStats.GetCSVHeader();

        foreach (string key in timeDictionary.Keys)
        {
            csv += key + ", " + timeDictionary[key].ToCSVString();
        }

        return csv;
    }
}
