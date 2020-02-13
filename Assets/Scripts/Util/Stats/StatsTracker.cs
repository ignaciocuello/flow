using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StatsTracker : Singleton<StatsTracker> {

    private TimeTracker timeTracker;
    public TimeTracker TimeTracker
    {
        get { return timeTracker; }
    }

    private DeltaTracker deltaTracker;
    public DeltaTracker DeltaTracker
    {
        get { return deltaTracker; }
    }

    private FrameCounter frameCounter;
    public FrameCounter FrameCounter
    {
        get { return frameCounter; }
    }

    private NamedTallier namedTallier;
    public NamedTallier NamedTallier
    {
        get { return namedTallier; }
    }

    private ProgressTracker progressTracker;
    public ProgressTracker ProgressTracker
    {
        get { return progressTracker; }
    }

    protected override void Initialize()
    {
        timeTracker = GetComponent<TimeTracker>();
        deltaTracker = GetComponent<DeltaTracker>();
        frameCounter = GetComponent<FrameCounter>();
        namedTallier = GetComponent<NamedTallier>();

        progressTracker = GetComponentInChildren<ProgressTracker>();
    }
}
