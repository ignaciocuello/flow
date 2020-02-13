using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MomentumGainData {

    /* momentum gain activated when velcoity components exceed activation vector components */
    public Vector2 ActivationThreshold;

    /* how much to multiply the attacking entity's velocity by when adding it to the receiving
     * entity, component wise */
    public Vector2 MomentumGainFactor;

    public Vector2 GetMomentumGain(Vector2 velocity)
    {
        return Vector2.Scale(velocity, GetVectorToggle(velocity) * MomentumGainFactor);
    }

    /* return for each vector component a 1 if that component in the velocity exceeds the activation
     * threshold */
    private Vector2 GetVectorToggle(Vector2 velocity)
    {
        return new Vector2(
            GetComponentToggle(velocity.x, ActivationThreshold.x),
            GetComponentToggle(velocity.y, ActivationThreshold.y));
    }

    public float GetComponentToggle(float velocityComponent, float threshold)
    {
        return Mathf.Abs(velocityComponent) < threshold ? 0.0f : 1.0f;
    }
}
