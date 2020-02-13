using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeFade : MonoBehaviour {

    private const float EPS = 0.001f;

    public float TargetVolume;
    public float LerpRate;

    private RealisticEngineSound source;

    private void Awake()
    {
        source = GetComponent<RealisticEngineSound>();
    }

    private void Update()
    {
        float volume = source.masterVolume;
        volume = TruncateVolume(Mathf.Lerp(volume, TargetVolume, LerpRate * Time.unscaledDeltaTime));

        source.masterVolume = volume;
    }

    private float TruncateVolume(float volume)
    {
        return Mathf.Abs(volume - TargetVolume) < EPS ? TargetVolume : volume;
    }
}
