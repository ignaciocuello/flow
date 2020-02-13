using RotaryHeart.Lib.SerializableDictionary;
using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class TargetEvent : UnityEvent<Target>
{
}

[Serializable]
public class TargetTypeSpriteMap : SerializableDictionaryBase<TargetType, TargetStates>
{
}

[Serializable]
public class TargetStates
{
    public TargetState LockedState;
    public TargetState UnlockedState;
}

public class Target : Entity
{

    [SerializeField]
    private TargetTypeSpriteMap stateMap;
    [SerializeField]
    private GameObject connectionObj;

    [SerializeField]
    private TargetType targetType;
    public TargetType TargetType
    {
        get { return targetType; }
        set
        {
            targetType = value;
            //use GetComponent here because this is called from OnValidate

            lockedState = stateMap[targetType].LockedState;
            unlockedState = stateMap[targetType].UnlockedState;

            InitStates();

            connectionObj.SetActive(targetType != TargetType.NULL);
        }
    }

    [SerializeField]
    /* amount to scale time when goal completed, usually < 1 */
    private float slowDownFactor;
    public float SlowDownFactor
    {
        get { return slowDownFactor; }
    }

    [SerializeField]
    /* amount to keep time slowed down in seconds */
    private float slowDownDuration;
    /* how sharply the time should normalize */
    [SerializeField]
    private float exponentialGrowthFactor;

    [SerializeField]
    private TargetEvent targetEntered;
    public TargetEvent TargetEntered
    {
        get { return targetEntered; }
    }

    [SerializeField]
    private TargetEvent targetExited;
    public TargetEvent TargetExited
    {
        get { return targetExited; }
    }

    /* with how much force this target attracts focus */
    [SerializeField]
    private float attractionForceMagnitude;
    public float AttractionForceMagnitude
    {
        get { return attractionForceMagnitude; }
    }

    /* animator of child light */
    private Animator lightAnimator;
    public Animator LightAnimator
    {
        get { return lightAnimator; }
    }

    /* focus currently held */
    public Focus Focus;

    [SerializeField]
    private TargetState lockedState;
    [SerializeField]
    private TargetState unlockedState;

    private void OnValidate()
    {
        TargetType = targetType;
    }

    public override void Awake()
    {
        base.Awake();

        lightAnimator = GetComponentInChildren<Animator>();

        Lock();
    }

    public void Lock()
    {
        State = lockedState;
    }

    public void Unlock()
    {
        State = unlockedState;
    }

    public override void InitStates()
    {
        lockedState = Instantiate(lockedState);
        unlockedState = Instantiate(unlockedState);
    }

    public void HoldFocus(Focus entered)
    {
        ((TargetState)State).HoldFocus(this, entered);
    }

    public void DropFocus()
    {
        ((TargetState)State).DropFocus(this);
    }

    public void LetGoOf(Focus old)
    {
        //let go of focus, but don't trigger events
        if (old.State.Descriptors.Contains(StateDescriptor.STATIC))
        {
            old.Rest();
        }

        //break the relationship with the old focus
        old.Target = null;
    }

    public void SlowDown()
    {
        TimeManager.Instance.SetSlowDownFactor(slowDownFactor, slowDownDuration, exponentialGrowthFactor);
    }
}

public enum TargetType
{
    NULL, OPEN, CLOSED
}

