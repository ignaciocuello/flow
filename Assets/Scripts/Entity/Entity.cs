using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class EntityStateEvent : UnityEvent<EntityState>
{
}

[Serializable]
public class EntityHitEvent : UnityEvent<HitBox, HurtBox, Entity>
{
}

public abstract class Entity : MonoBehaviour {

    private const float VERTICAL_TEXT_OFFSET = 1.0f;

    private Text debugText;
    public Text DebugText
    {
        get { return debugText; }
        set { debugText = value; }
    }

    protected Rigidbody2D entityRigidbody;

    [SerializeField]
    private SpriteObject entitySpriteObject;
    public SpriteObject EntitySpriteObject
    {
        get { return entitySpriteObject; }
    }

    /* rigidbody interfaces */
    //velocity cached for unconstraint
    private Vector2 cachedVelocity;
    public Vector2 Velocity
    {
        get { return entityRigidbody.velocity; }
        set
        {
            cachedVelocity = value;
            entityRigidbody.velocity = cachedVelocity;
        }
    }

    public float VelX
    {
        get { return Velocity.x; }
        set { Velocity = new Vector2(value, entityRigidbody.velocity.y); }
    }

    public float VelY
    {
        get { return Velocity.y; }
        set { Velocity = new Vector2(entityRigidbody.velocity.x, value); }
    }

    //velocity cached for unconstraint
    private float cachedAngularVelocity;
    public float AngularVelocity
    {
        get { return entityRigidbody.angularVelocity; }
        set {
            cachedAngularVelocity = value;
            entityRigidbody.angularVelocity = cachedAngularVelocity;
        }
    }

    public float Drag
    {
        get { return entityRigidbody.drag; }
        set { entityRigidbody.drag = value; }
    }

    public float GravityScale
    {
        get { return entityRigidbody.gravityScale; }
        set { entityRigidbody.gravityScale = value; }
    }

    public RigidbodyConstraints2D Constraints
    {
        get { return entityRigidbody.constraints; }
        set { entityRigidbody.constraints = value; }
    }

    public float Mass
    {
        get { return entityRigidbody.mass; }
    }

    public PhysicsMaterial2D PhysMat
    {
        get { return entityRigidbody.sharedMaterial; }
        set { entityRigidbody.sharedMaterial = value; }
    }

    private BoxCollider2D aabb;
    public BoxCollider2D AABB
    {
        get { return aabb; }
    }

    [SerializeField]
    private EntityStateEvent onEnterStateEvent;
    public EntityStateEvent OnEnterStateEvent
    {
        get { return onEnterStateEvent; }
    }

    [SerializeField]
    private EntityStateEvent onExitStateEvent;
    public EntityStateEvent OnExitStateEvent
    {
        get { return onExitStateEvent; }
    }

    [SerializeField]
    private EntityHitEvent onHitEvent;
    public EntityHitEvent OnHitEvent
    {
        get { return onHitEvent; }
    }

    [SerializeField]
    private EntityHitEvent onHitReceivedEvent;
    public EntityHitEvent OnHitReceivedEvent
    {
        get { return onHitReceivedEvent; }
    }
    
    [SerializeField]
    private EntityState state;
    /* throughout the code we always assume State != null */
    public virtual EntityState State
    {
        get { return state; }
        set
        {
            if (state != null)
            {
                EntityState prev = state;
                state = null;

                //exit previous state before entering new one
                prev.BaseExitState(this);
                onExitStateEvent.Invoke(prev);

                //exit if state already set by exit state
                if (state != null)
                {
                    return;
                }
                //derive state if given a null value
                if (value == null)
                {
                    prev.BaseDeriveState(this);
                    return;
                }
            }

            state = value;
            state.BaseEnterState(this);

            onEnterStateEvent.Invoke(value);

            //exit state if finished
            if (state.IsFinished())
            {
                State = null;
            }
        }
    }

    [SerializeField]
    private HitStop hitStopState;
    public HitStop HitStopState
    {
        get { return hitStopState; }
    }

    private HitStop hitStop;
    public HitStop HitStop
    {
        get { return hitStop; }
        set
        {
            if (hitStop != null)
            {
                HitStop prev = hitStop;
                hitStop = null;

                prev.BaseExitState(this);
            }

            hitStop = value;
            if (hitStop != null)
            {
                hitStop.BaseEnterState(this);

                if (hitStop.Duration == 0)
                {
                    HitStop = null;
                }
            }
        }
    }

    public bool IgnoreBounce;

    [Tooltip("Normal impulse must be above this value (but below strong threshold) to register a weak bounce"), SerializeField]
    private float weakNormalImpulseThreshold;
    [SerializeField]
    private TrajectoryModData weakBounceData;

    [Tooltip("Normal impulse must be above this value to register a strong bounce"), SerializeField]
    private float strongNormalImpulseThreshold;
    [SerializeField]
    private TrajectoryModData strongBounceData;

    protected TrajectoryModifier trajectoryModifier;
    public TrajectoryModifier TrajectoryModifier
    {
        get { return trajectoryModifier; }
    }

    protected AdjacentSurfaceMapper adjacentSurfaceMapper;
    public AdjacentSurfaceMapper AdjacentSurfaceMapper
    {
        get { return adjacentSurfaceMapper; }
    }

    public bool AnimateWhenTimeFrozen;

    protected float defaultDrag;
    protected float defaultGravityScale;
    protected Bounds? defaultABBB;
    protected RigidbodyConstraints2D defaultConstraints;

    /* whether the entity is facing right */
    private bool facingRight;
    public bool FacingRight
    {
        get { return facingRight; }
        set
        {
            facingRight = value;
            if (entitySpriteObject != null)
            {
                entitySpriteObject.transform.localScale = new Vector3(facingRight ? -1.0f : 1.0f, 1.0f, 1.0f);
            }
        }
    }

    public virtual void Awake()
    {
        //EntitySpriteRenderer.material = new Material(EntitySpriteRenderer.material);

        entityRigidbody = GetComponent<Rigidbody2D>();
        adjacentSurfaceMapper = GetComponent<AdjacentSurfaceMapper>();
        aabb = GetComponent<BoxCollider2D>();

        defaultDrag = entityRigidbody.drag;
        defaultGravityScale = entityRigidbody.gravityScale;
        if (aabb != null)
        {
            defaultABBB = new Bounds(center: aabb.offset, size: aabb.size);
        }

        defaultConstraints = entityRigidbody.constraints;

        FacingRight = true;

        DebugText = UserInterface.Instance.Create(UIElemType.DEBUG_TEXT).GetComponent<Text>();

        InitStates();
    }

    public virtual void InitStates()
    {
        if (hitStopState != null)
        {
            //hitStopState = Instantiate(hitStopState);
        }
    }

    public virtual void FixedUpdate()
    {
        if (HitStop != null)
        {
            HitStop.BaseStateFixedUpdate(this);

            if (HitStop.IsFinished())
            {
                HitStop = null;
            }
        }
        else
        {
            FixedUpdate(State);

            if (State.IsFinished())
            {
                State = null;
            }
        }  
    }

    public void FixedUpdate(EntityState enState)
    {
        if (HasTrajectoryModifier())
        {
            trajectoryModifier.UpdateTrajectory(this);
            if (trajectoryModifier.IsDone())
            {
                trajectoryModifier.End(this);
                trajectoryModifier = null;
            }
        }

        enState.BaseStateFixedUpdate(this);

        CheckAdjacentSurfaceMap(enState);
    }

    private void CheckAdjacentSurfaceMap(EntityState enState)
    {
        Dictionary<Vector2, GameObject[]> adjSurfaceMap = adjacentSurfaceMapper.CalculateAdjacentSurfaceMap();
        enState.BaseStateOnAdjacentSurfaceMap(this, adjSurfaceMap);
    }

    public virtual void Update()
    {
        Update(State);
    }

    public void Update(EntityState enState)
    {
        DebugText.rectTransform.position = transform.position + Vector3.up * VERTICAL_TEXT_OFFSET;
        enState.BaseStateUpdate(this);
    }

    public virtual void OnHit(HitBox hitBox, HurtBox hurtBox, Entity receiving)
    {
        OnHit(State, hitBox, hurtBox, receiving);
    }

    public void OnHit(EntityState enState, HitBox hitBox, HurtBox hurtBox, Entity receiving)
    {
        onHitEvent.Invoke(hitBox, hurtBox, receiving);
        enState.BaseStateOnHit(this, hitBox, hurtBox, receiving);
    }

    public virtual void OnHitReceived(HitBox hitBox, HurtBox hurtBox, Entity initiator)
    {
        OnHitReceived(State, hitBox, hurtBox, initiator);
    }

    public void OnHitReceived(EntityState enState, HitBox hitBox, HurtBox hurtBox, Entity initiator)
    {
        onHitReceivedEvent.Invoke(hitBox, hurtBox, initiator);
        enState.BaseStateOnHitReceived(this, hitBox, hurtBox, initiator);
    }

    public virtual void OnAdjacentSurfaceMap(Dictionary<Vector2, GameObject[]> adjSurfaceMap)
    {

    }

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionEnter2D(State, collision);
    }

    public void OnCollisionEnter2D(EntityState enState, Collision2D collision)
    {
        Dictionary<Vector2, GameObject[]> adjSurfaceMap = adjacentSurfaceMapper.CalculateAdjacentSurfaceMap();
        ResolveBounce(collision);
        enState.BaseStateOnCollisionEnter2D(this, collision, adjSurfaceMap);
    }

    public virtual void OnCollisionStay2D(Collision2D collision)
    {
        OnCollisionStay2D(State, collision);
    }

    public void OnCollisionStay2D(EntityState enState, Collision2D collision)
    {
        Dictionary<Vector2, GameObject[]> adjSurfaceMap = adjacentSurfaceMapper.CalculateAdjacentSurfaceMap();
        ResolveBounce(collision);
        enState.BaseStateOnCollisionStay2D(this, collision, adjSurfaceMap);
    }

    public virtual bool ResolveBounce(Collision2D collision)
    {
        if (IgnoreBounce)
        {
            return false;
        }

        //10 is arbitrary value
        ContactPoint2D[] contacts = new ContactPoint2D[10];
        int numContacts = collision.GetContacts(contacts);

        if (numContacts > 0)
        {
            bool bounce = false;
            TrajectoryModData bounceData = new TrajectoryModData();
            
            if (contacts[0].normalImpulse > strongNormalImpulseThreshold)
            {
                bounceData = strongBounceData;
                bounce = true;
            }
            else if (contacts[0].normalImpulse > weakNormalImpulseThreshold)
            {
                bounceData = weakBounceData;
                bounce = true;
            }

            if (bounce)
            {
                ResetDrag();

                //only if relative velocity is negative i.e. we're moving towards the object
                //velocity before collision
                Vector2 incomingVel = collision.otherCollider.gameObject == gameObject ? -contacts[0].relativeVelocity : contacts[0].relativeVelocity;
                Vector2 dir = incomingVel.normalized;

                Vector2 norm = contacts[0].normal;
                Vector2 outputDir = (dir - 2.0f * (Vector2.Dot(dir, norm)) * norm).normalized;

                Vector2 bounceImpact = contacts[0].normalImpulse * outputDir * bounceData.Impact;

                Impact(new TrajectoryModData(bounceImpact, bounceData));
            }

            return bounce;
        }

        return false;
    }

    public virtual void OnCollisionExit2D(Collision2D collision)
    {
        OnCollisionExit2D(State, collision);
    }

    public void OnCollisionExit2D(EntityState enState, Collision2D collision)
    {
        Dictionary<Vector2, GameObject[]> adjSurfaceMap = adjacentSurfaceMapper.CalculateAdjacentSurfaceMap();
        //this will generally be called on the wrong state
        //as state transitions will occur before physics updates which is when
        //these are called
        enState.BaseStateOnCollisionExit2D(this, collision, adjSurfaceMap);
    }

    public virtual void OnTriggerEnter2D(Collider2D collider)
    {
        State.BaseStateOnTriggerEnter2D(this, collider);
    }

    public virtual void OnTriggerStay2D(Collider2D collider)
    {
        State.BaseStateOnTriggerStay2D(this, collider);
    }

    public virtual void OnTriggerExit2D(Collider2D collider)
    {
        State.BaseStateOnTriggerExit2D(this, collider);
    }

    /* applies hit stop to this entity for 'duration' frames */
    public virtual void ApplyHitStop(int hitStopDuration, float hitStopPower, bool receiving)
    {
        State.BaseApplyHitStop(this, hitStopDuration, hitStopPower, receiving);
    }

    public virtual void ApplyHitStun(HitData hitData)
    {
        State.BaseApplyHitStun(this, hitData);
    }

    public virtual void DeriveState()
    {
        DeriveState(State);
    }

    public void DeriveState(EntityState enState)
    {
        enState.BaseDeriveState(this);
    }

    public void Impact(TrajectoryModData tmod)
    {
        trajectoryModifier = new TrajectoryModifier(this, tmod);
    }

    public bool HasTrajectoryModifier()
    {
        return trajectoryModifier != null;
    }

    public void RemoveTrajectoryModifier()
    {
        trajectoryModifier.End(this);
        trajectoryModifier = null;
    }

    public void AddForce(Vector2 force)
    {
        entityRigidbody.AddForce(force);
    }

    public void ResetAABB()
    {
        if (defaultABBB != null)
        {
            SetAABB((Bounds)defaultABBB);
        }
    }

    public void SetAABB(Bounds bounds)
    {
        aabb.offset = bounds.center;
        aabb.size = bounds.size;
    }

    public void DefaultResetDrag()
    {
        entityRigidbody.drag = defaultDrag;
    }

    public void ResetDrag()
    {
        State.BaseResetDrag(this);
    }

    public void DefaultResetGravityScale()
    {
        entityRigidbody.gravityScale = defaultGravityScale; entityRigidbody.gravityScale = defaultGravityScale;
    }

    public void ResetGravityScale()
    {
        State.BaseResetGravityScale(this);
    }

    public void Constraint(RigidbodyConstraints2D constraints)
    {
        cachedVelocity = Velocity;
        cachedAngularVelocity = AngularVelocity;

        entityRigidbody.constraints = constraints;
    }

    public void Unconstraint()
    {
        if (HasTrajectoryModifier())
        {
            //NOT SURE WHY WE DO THIS
            entityRigidbody.drag = trajectoryModifier.StoredDrag;
        }

        entityRigidbody.constraints = defaultConstraints;

        Velocity = cachedVelocity;
        AngularVelocity = cachedAngularVelocity;
    }

    public void Flash()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Flash");
        }
    }

    public virtual void OnDestroy()
    {
        if (DebugText != null)
        {
            Destroy(DebugText.gameObject);
        }
    }
}
