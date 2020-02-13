using EZCameraShake;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CamShakeData {

    /* camera shake parameters */
    [SerializeField]
    private float magnitude;
    [SerializeField]
    private float roughness;
    [SerializeField]
    private float fadeIn;
    [SerializeField]
    private float fadeOut;

    public CamShakeData(float magnitude, float roughness, float fadeIn, float fadeOut)
    {
        this.magnitude = magnitude;
        this.roughness = roughness;
        this.fadeIn = fadeIn;
        this.fadeOut = fadeOut;
    }

    public void Shake()
    {
        CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeIn, fadeOut);
    }

    public static CamShakeData Lerp(CamShakeData c0, CamShakeData c1, float t)
    {
        return new CamShakeData(
            Mathf.Lerp(c0.magnitude, c1.magnitude, t),
            Mathf.Lerp(c0.roughness, c1.roughness, t),
            Mathf.Lerp(c0.fadeIn, c1.fadeIn, t),
            Mathf.Lerp(c0.fadeOut, c1.fadeOut, t));
    }
}
