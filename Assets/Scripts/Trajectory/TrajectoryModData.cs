using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TrajectoryModData {

    public int Duration;

    public Vector2 Impact;
    public float AngularImpact;

    public float DragAddition;
    public float MetaDrag;

    public bool Additive;

    public TrajectoryModData(Vector2 impact, float angularImpact, float dragAddition, float metaDrag, int duration, bool additive)
    {
        Impact = impact;
        AngularImpact = angularImpact;
        DragAddition = dragAddition;
        MetaDrag = metaDrag;

        Duration = duration;
        Additive = additive;
    }

    public TrajectoryModData(Vector2 impact, float dragAddition, float metaDrag, int duration, bool additive) : this(impact, 0.0f, dragAddition, metaDrag, duration, additive)
    {
    }

    public TrajectoryModData(Vector2 impact, float dragAddition, float metaDrag, int duration) : this(impact, dragAddition, metaDrag, duration, false)
    {
    }

    public TrajectoryModData(Vector2 impact, TrajectoryModData modData) : this(impact, modData.AngularImpact, modData.DragAddition, modData.MetaDrag, modData.Duration, modData.Additive)
    {
    }

    public TrajectoryModData(Vector2 impact, float angularImpact, TrajectoryModData modData) : this(impact, angularImpact, modData.DragAddition, modData.MetaDrag, modData.Duration, modData.Additive)
    {
    }
}
