using System.Collections;
using System;
using UnityEngine;

[Serializable]
public class DeltaStats : TimeStats {

    /* main data */
    public object MeasuredObject;
    /* calling AttributeGetterFunction(MeasuredObject) should return the measured attribute */
    public Func<object, object> AttributeGetterFunction;

    /* attribute as it was initially */
    public object InitialAttribute;

    private object currentAttribute;
    /* attribute as it is now */
    public object CurrentAttribute
    {
        get { return currentAttribute; }
        set
        {
            currentAttribute = value;
            Debug.Log(currentAttribute);
            CurrentAttributeStr = ToStringFunction(currentAttribute);
            AttributeDifferenceStr = ToStringFunction(DifferenceFunction(currentAttribute, InitialAttribute));
        }
    }

    /* function used to calculate difference between initial attribute and current */
    public Func<object, object, object> DifferenceFunction;
    /* function to use to convert the attributes to strings */
    public Func<object, string> ToStringFunction;

    /* store this only so that its displayed on the editor */
    public string InitialAtributeStr;

    /* store this only so that its displayed on editor */
    public string CurrentAttributeStr;

    /* string storing the difference between current attribute and initial attribute */
    public string AttributeDifferenceStr;

    public DeltaStats(
        object measuredObject,
        Func<object, object> attributeGetterFunction, 
        Func<object, object, object> differenceFunction, 
        Func<object, string> toStringFunction)
    {
        MeasuredObject = measuredObject;
        AttributeGetterFunction = attributeGetterFunction;

        InitialAttribute = AttributeGetterFunction(MeasuredObject);
        currentAttribute = InitialAttribute;

        DifferenceFunction = differenceFunction;
        ToStringFunction = toStringFunction;

        InitialAtributeStr = InitialAttribute.ToString();
        CurrentAttributeStr = InitialAtributeStr;

        AttributeDifferenceStr = ToStringFunction(differenceFunction(currentAttribute, InitialAttribute));
    }

    public override void UpdateStats()
    {
        base.UpdateStats();
        CurrentAttribute = AttributeGetterFunction(MeasuredObject);
    }
}
