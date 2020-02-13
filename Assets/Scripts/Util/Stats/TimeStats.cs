using System;
using UnityEngine;

[Serializable]
public class TimeStats  {

    private const float FRAMES_PER_SECOND = 60.0f;

    /* time data */
    public readonly float InitialTime;

    [SerializeField]
    private float currentTime;
    public float CurrentTime
    {
        get { return currentTime; }
        set
        {
            currentTime = value;
            Duration = currentTime - InitialTime;
            FramesDuration = Duration * FRAMES_PER_SECOND;
        }
    }

    /* derived attributes */
    public float Duration;
    public float FramesDuration;

    public TimeStats()
    {
        InitialTime = Time.time;
        CurrentTime = Time.time;
    }

    public virtual void UpdateStats()
    {
        CurrentTime = Time.time;
    }

    public string ToCSVString()
    {
        return string.Format("{0}, {1}, {2}\n", 
            CurrentTime.ToString("F6"), Duration.ToString("F6"), FramesDuration.ToString("F6"));
    }

    public static string GetCSVHeader()
    {
        return "Time, Duration, Frames\n";
    }
}
