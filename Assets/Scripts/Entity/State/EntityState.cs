using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityState<TEntity> : EntityState where TEntity : Entity {

    sealed public override void BaseEnterState(Entity entity)
    {
        EnterState((TEntity)entity);
        base.BaseEnterState(entity);
    }

    public virtual void EnterState(TEntity entity)
    {
    }

    sealed public override void BaseStateFixedUpdate(Entity entity)
    {
        StateFixedUpdate((TEntity)entity);
        base.BaseStateFixedUpdate(entity);
    }

    public virtual void StateFixedUpdate(TEntity entity)
    {
    }

    sealed public override void BaseStateUpdate(Entity entity)
    {
        StateUpdate((TEntity)entity);
        base.BaseStateUpdate(entity);
    }

    public virtual void StateUpdate(TEntity entity)
    {
    }

    sealed public override void BaseStateOnHit(Entity entity, HitBox hitBox, HurtBox hurtBox, Entity receiving)
    {
        StateOnHit((TEntity)entity, hitBox, hurtBox, receiving);
    }

    public virtual void StateOnHit(TEntity entity, HitBox hitBox, HurtBox hurtBox, Entity receiving)
    {
        entity.ApplyHitStop(hitBox.HitData.HitStopDuration, hitBox.HitData.HitStopPower, false);
    }

    sealed public override void BaseStateOnHitReceived(Entity entity, HitBox hitBox, HurtBox hurtBox, Entity initiator)
    {
        StateOnHitReceived((TEntity)entity, hitBox, hurtBox, initiator);
    }

    public virtual void StateOnHitReceived(TEntity entity, HitBox hitBox, HurtBox hurtBox, Entity initiator)
    {
        entity.ApplyHitStun(hitBox.HitData);
    }

    sealed public override void BaseStateOnAdjacentSurfaceMap(Entity entity, Dictionary<Vector2, GameObject[]> adjacentSurfaceMap)
    {
        StateOnAdjacentSurfaceMap((TEntity)entity, adjacentSurfaceMap);
    }

    public virtual void StateOnAdjacentSurfaceMap(TEntity entity, Dictionary<Vector2, GameObject[]> adjacentSurfaceMap)
    {
    }

    sealed public override void BaseStateOnCollisionEnter2D(
        Entity entity, Collision2D collision, Dictionary<Vector2, GameObject[]> directionCollisionMap)
    {
        StateOnCollisionEnter2D((TEntity)entity, collision, directionCollisionMap);
    }

    public virtual void StateOnCollisionEnter2D(
        TEntity entity, Collision2D collision, Dictionary<Vector2, GameObject[]> directionCollisionMap)
    {
    }

    sealed public override void BaseStateOnCollisionStay2D(
        Entity entity, Collision2D collision, Dictionary<Vector2, GameObject[]> directionCollisionMap)
    {
        StateOnCollisionStay2D((TEntity)entity, collision, directionCollisionMap);
    }

    public virtual void StateOnCollisionStay2D(
        TEntity entity, Collision2D collision, Dictionary<Vector2, GameObject[]> directionCollisionMap)
    {
    }

    sealed public override void BaseStateOnCollisionExit2D(
        Entity entity, Collision2D collision, Dictionary<Vector2, GameObject[]> directionCollisionMap)
    {
        StateOnCollisionExit2D((TEntity)entity, collision, directionCollisionMap);
    }

    public virtual void StateOnCollisionExit2D(
        TEntity entity, Collision2D collision, Dictionary<Vector2, GameObject[]> directionCollisionMap)
    {
    }

    public sealed override void BaseStateOnTriggerEnter2D(Entity entity, Collider2D collider)
    {
        StateOnTriggerEnter2D((TEntity)entity, collider);
    }

    public virtual void StateOnTriggerEnter2D(TEntity entity, Collider2D collider)
    {
    }

    public sealed override void BaseStateOnTriggerStay2D(Entity entity, Collider2D collider)
    {
        StateOnTriggerStay2D((TEntity)entity, collider);
    }

    public virtual void StateOnTriggerStay2D(TEntity entity, Collider2D collider)
    {
    }

    public sealed override void BaseStateOnTriggerExit2D(Entity entity, Collider2D collider)
    {
        StateOnTriggerExit2D((TEntity)entity, collider);
    }

    public virtual void StateOnTriggerExit2D(TEntity entity, Collider2D collider)
    {
    }

    sealed public override void BaseApplyHitStop(Entity entity, int hitStopDuration, float hitStopPower, bool receiving)
    {
        ApplyHitStop((TEntity)entity, hitStopDuration, hitStopPower, receiving);
    }

    public virtual void ApplyHitStop(TEntity entity, int hitStopDuration, float hitStopPower, bool receiving)
    {
        entity.HitStopState.Duration = hitStopDuration;
        entity.HitStopState.FrameData = ExtractSingleFrame(entity.State);
        entity.HitStopState.Receiving = receiving;
        entity.HitStopState.Power = hitStopPower;

        entity.HitStop = entity.HitStopState;
    }

    sealed public override void BaseApplyHitStun(Entity entity, HitData hitData)
    {
        ApplyHitStun((TEntity)entity, hitData);
    }

    public virtual void ApplyHitStun(TEntity entity, HitData hitData)
    {
        EntityState hitStunState = HitStunState(entity);
        hitStunState.Duration = hitData.HitStunDuration;

        entity.Impact(hitData.TrajectoryModData);
        entity.State = hitStunState;

        entity.ApplyHitStop(hitData.HitStopDuration, hitData.HitStopPower, receiving: true);
    }

    sealed public override EntityState BaseHitStunState(Entity entity)
    {
        return HitStunState((TEntity)entity);
    }

    public abstract EntityState HitStunState(TEntity entity);

    public sealed override void BaseResetDrag(Entity entity)
    {
        ResetDrag((TEntity)entity);
    }

    public virtual void ResetDrag(TEntity entity)
    {
        entity.DefaultResetDrag();
    }

    public sealed override void BaseResetGravityScale(Entity entity)
    {
        ResetGravityScale((TEntity)entity);
    }

    public virtual void ResetGravityScale(TEntity entity)
    {
        entity.DefaultResetGravityScale();
    }

    sealed public override void BaseDeriveState(Entity entity)
    {
        DeriveState((TEntity)entity);
    }

    public abstract void DeriveState(TEntity entity);

    sealed public override void BaseExitState(Entity entity)
    {
        ExitState((TEntity)entity);
    }

    //NOTE: when calling entity.DeriveState() from ExitState use DeriveState(entity) instead as it is likely
    //that no state is present in the stack at this point resulting in a NullReferenceException
    //NOTE2: just don't call derive state from exit state, trust
    public virtual void ExitState(TEntity entity)
    {
    }
}

public abstract class EntityState : ScriptableObject
{
    public const int UNBOUNDED_DURATION = -1;

    [SerializeField]
    private FrameData frameData;
    public FrameData FrameData
    {
        get { return frameData; }
        set { frameData = value; }
    }

    /* descriptions of the state */
    public List<StateDescriptor> Descriptors;

    /* the current frame of the state, unscaled for playback speed/timescale */
    protected int unscaledCurrentFrame;

    /* frame counter used for frame data with playback speed scaled for playback speed and timescale */
    [NonSerialized]
    public float ScaledCurrentFrame;

    public int Duration = UNBOUNDED_DURATION;

    //used for velocity component comparisons against zero in some of the states.
    public const float EPSILON = 1.0e-3f;

    public virtual void BaseEnterState(Entity entity)
    {
        unscaledCurrentFrame = 0;
        ScaledCurrentFrame = 0.0f;
    }

    /* advances this state by numFrames */
    public void Advance(Entity entity, int numFrames)
    {
        for (int frame = 0; frame < numFrames; frame++)
        {
            BaseStateFixedUpdate(entity);
        }
    }

    public virtual void BaseStateFixedUpdate(Entity entity)
    {
        ScaledFixedUpdate(entity, Time.timeScale);
    }

    private void ScaledFixedUpdate(Entity entity, float timeScale)
    {
        if (FrameData != null)
        {
            if (FrameData.Frames != null && FrameData.Frames.Length > 0)
            {
                ExecuteFrame(entity);
            }

            //Time.timeScale so animation is affected by slow down effect
            ScaledCurrentFrame += (FrameData.PlayBackSpeed * timeScale);
        }

        unscaledCurrentFrame++;
    }

    private void ExecuteFrame(Entity entity)
    {
        //wrap around the frames
        BoxManager.Instance.PutBoxArray(entity, FrameData.Frames[(int)ScaledCurrentFrame % FrameData.Frames.Length].Boxes);
        if (FrameData.Sprites.Length > 0)
        {
            entity.EntitySpriteObject.Sprite = FrameData.Sprites[(int)ScaledCurrentFrame % FrameData.Sprites.Length];
        }
        entity.ResetAABB();
    }

    private float accumulatedTime;

    public virtual void BaseStateUpdate(Entity entity)
    {
        if (entity.AnimateWhenTimeFrozen && Time.timeScale == 0.0f)
        {
            accumulatedTime += Time.unscaledDeltaTime;

            while (accumulatedTime > 1 / 60.0f)
            {
                ScaledFixedUpdate(entity, 1.0f);

                accumulatedTime -= 1 / 60.0f;
            }
        }
    }

    public abstract void BaseStateOnHit(Entity entity, HitBox hitBox, HurtBox hurtBox, Entity receiving);

    public abstract void BaseStateOnHitReceived(Entity entity, HitBox hitBox, HurtBox hurtBox, Entity initiator);

    public abstract void BaseStateOnAdjacentSurfaceMap(Entity entity, Dictionary<Vector2, GameObject[]> adjacentSurfaceMap);

    public abstract void BaseStateOnCollisionEnter2D(
        Entity entity, Collision2D collision, Dictionary<Vector2, GameObject[]> directionCollisionMap);

    public abstract void BaseStateOnCollisionStay2D(
        Entity entity, Collision2D collision, Dictionary<Vector2, GameObject[]> directionCollisionMap);

    public abstract void BaseStateOnCollisionExit2D(
        Entity entity, Collision2D collision, Dictionary<Vector2, GameObject[]> directionCollisionMap);

    public abstract void BaseStateOnTriggerEnter2D(Entity entity, Collider2D collider);

    public abstract void BaseStateOnTriggerStay2D(Entity entity, Collider2D collider);

    public abstract void BaseStateOnTriggerExit2D(Entity entity, Collider2D collider);

    public abstract void BaseDeriveState(Entity entity);

    public abstract void BaseResetDrag(Entity entity);

    public abstract void BaseResetGravityScale(Entity entity);

    /* applies hit stop to this entity for 'duration' frames */
    public abstract void BaseApplyHitStop(Entity entity, int hitStopDuration, float hitStopPower, bool receiving);

    public abstract void BaseApplyHitStun(Entity entity, HitData hitData);

    /* return the hit stun state also do anything you need to do to entity before hit stun is applied */
    public abstract EntityState BaseHitStunState(Entity entity);

    public abstract void BaseExitState(Entity entity);

    /* return an array of the name of possible actions the entity may do from this state */
    public virtual string[] GetAvailableActions()
    {
        return new string[] { };
    }

    /* return true if the state is finished, by default this is defined to be
     * when the current frame is equal to the duration, some actions may have
     different defenitions for when they are finished, which is why the method is
     virtual. */
    public virtual bool IsFinished()
    {
        return Duration != UNBOUNDED_DURATION && (int)ScaledCurrentFrame >= Duration;
    }

    public static FrameData ExtractSingleFrame(EntityState state)
    {
        FrameData newFrameData = CreateInstance<FrameData>();

        float modCurrentFrame = state.ScaledCurrentFrame;

        Frame[] frames = new Frame[]
        {
            state.FrameData.Frames[(int)modCurrentFrame % state.FrameData.Frames.Length]
        };

        Sprite[] spriteList = new Sprite[]
        {
            state.FrameData.SpriteList[(int)modCurrentFrame % state.FrameData.SpriteList.Length]
        };

        LayeredSprite[] sprites = new LayeredSprite[0];

        newFrameData.Frames = frames;
        newFrameData.SpriteList = spriteList;
        newFrameData.Sprites = sprites;
        newFrameData.PlayBackSpeed = 1.0f;

        return newFrameData;
    }
}

