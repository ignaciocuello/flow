using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

[Serializable]
public class DeltaStatsDictionary : SerializableDictionaryBase<string, DeltaStats>
{
}

public class DeltaTracker : MonoBehaviour {

    private const string FORMAT_STRING = "F8";

    public static readonly Func<object, object, object> VECTOR3_DELTA_FUNC
        = (o1, o2) => (Vector3)o1 - (Vector3)o2;
    public static readonly Func<object, object, object> FLOAT_DELTA_FUNC
        = (o1, o2) => (float)o1 - (float)o2;

    public static readonly Func<object, string> VECTOR3_TO_STR_FUNC
        = o => ((Vector3)o).ToString(FORMAT_STRING);
    public static readonly Func<object, string> FLOAT_TO_STR_FUNC
        = o => ((float)o).ToString(FORMAT_STRING);

    [SerializeField]
    private DeltaStatsDictionary deltaDictionary;

    private void Awake()
    {
        deltaDictionary = new DeltaStatsDictionary();
    }

    public void StartMeasuring(string name,
        object measuredObject, Func<object, object> attributeGetterFunction,
        Func<object, object, object> differenceFunction, Func<object, string> toStringFunction)
    {
        DeltaStats stats = 
            new DeltaStats(measuredObject, attributeGetterFunction, differenceFunction, toStringFunction);

        //have to do this so it displays properly in editor
        if (deltaDictionary.ContainsKey(name))
        {
            deltaDictionary[name] = stats;
        }
        else
        {
            deltaDictionary.Add(name, stats);
        }
    }

    public void StartMeasuringVector3(string name,
        object measuredObject, Func<object, object> attributeGetterFunction)
    {
        StartMeasuring(name, measuredObject, attributeGetterFunction, VECTOR3_DELTA_FUNC, VECTOR3_TO_STR_FUNC);
    }

    public void StartMeasuringFloat(string name,
        object measuredObject, Func<object, object> attributeGetterFunction)
    {
        StartMeasuring(name, measuredObject, attributeGetterFunction, FLOAT_DELTA_FUNC, FLOAT_TO_STR_FUNC);
    }

    public bool RemoveMeasurementIfExists(string name)
    {
        if (deltaDictionary.ContainsKey(name))
        {
            RemoveMeasurment(name);
            return true;
        }

        return false;
    }

    public void RemoveMeasurment(string name)
    {
        deltaDictionary.Remove(name);
    }

    /* only update the measurement if it exists, otherwise silently do nothing, return whether measurement exists */
    public bool UpdateMeasurementIfExists(string name)
    {
        if (deltaDictionary.ContainsKey(name))
        {
            UpdateMeasurement(name);
            return true;
        }

        return false;
    }

    public void UpdateMeasurement(string name)
    {
        //NOTE: this might cause issues
        deltaDictionary[name].UpdateStats();
        //DeltaStats stats = deltaDictionary[name];
        //stats.Update();
        //deltaDictionary[name] = stats;
    }

    public DeltaStats GetMeasurement(string name)
    {
        return deltaDictionary[name];
    }

    public bool MeasurementExists(string name)
    {
        return deltaDictionary.ContainsKey(name);
    }

}
