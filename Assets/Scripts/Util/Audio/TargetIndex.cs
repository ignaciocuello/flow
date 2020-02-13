using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TargetIndex {

    public float Priority;
    public float Target;

    public TargetIndex(float priority, float target)
    {
        Priority = priority;
        Target = target;
    }

}
