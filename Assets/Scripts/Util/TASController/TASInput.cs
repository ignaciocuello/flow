using System;

[Serializable]
public class TASInput {

    //name of the input axis/button
    public string Name;

    //value of axis/button
    public float Value;

    //duration of this input value in frames
    public int Duration;

    //amount of frames left in this TASInput
    public int TimeToLive;

    public TASInput(string name, float value, int duration)
    {
        Name = name;
        Value = value;
        Duration = duration;
        TimeToLive = duration;
    }

    public bool IsFirst()
    {
        return TimeToLive == Duration;
    }
}
