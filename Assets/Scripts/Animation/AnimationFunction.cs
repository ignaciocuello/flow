using UnityEngine;

public class AnimationFunction {

    //the duration of the animation in seconds i.e the size of the domain of the function
    private float duration;
    //the time at which the animation started
    private float startTime;

    //the value of the variable when the animation started
    private float startValue;
    //the value the variable should have when the animation ends, i.e. when time = startTime + duration
    private float endValue;
    public float EndValue
    {
        get { return endValue; }
    }

    public AnimationFunction(float startTime, float startValue, float duration, float endValue)
    {
        this.startTime = startTime;
        this.startValue = startValue;
        this.duration = duration;
        this.endValue = endValue;
    }

    public float GetValue(float time)
    {
        return (endValue - startValue) * (time - startTime)/duration + startValue;
    }

    public bool Finished(float time)
    {
        return time > startTime + duration;
    } 
}
