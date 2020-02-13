using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Player {

    [SerializeField]
    private float punchDelay;

    private BehaviourState<Boss> behaviour;
    public BehaviourState<Boss> Behaviour
    {
        get { return behaviour; }
        set
        {
            if (behaviour != null)
            {
                BehaviourState<Boss> prev = behaviour;
                behaviour = null;

                prev.BehaviourExit(this);
            }

            behaviour = value;
            if (behaviour != null)
            {
                behaviour.BehaviourEnter(this);
            }
        }
    }

    [SerializeField, Tooltip("how quickly this boss' gravity changes whenn we decide to change it smoothly")]
    private float gravChangeSpeed;
    public bool SmoothGravReset;

    [SerializeField]
    private IdleBossBehaviour idleBehaviour;
    [SerializeField]
    private StealingBossBehaviour stealingBehaviour;
    [SerializeField]
    private EscapingBossBehaviour escapingBehaviour;

    private bool stunOnHit;

    public override void Awake()
    {
        OnEnterStateEvent.AddListener(state =>
        {
            if (state is GroundedPlayerState)
            {
                stunOnHit = true;
            }
        });

        OnExitStateEvent.AddListener(state =>
        {
            if (!(state is HitStunBossState))
            {
                SmoothGravReset = false;
                DefaultResetGravityScale();
            }
        });

        base.Awake();  
    }

    public override void Start()
    {
        inputGenerator = new AIInputGenerator("AI");
        InputBuffer.Instance.AddInputGenerator(inputGenerator);
    }

    public override void OnHitReceived(HitBox hitBox, HurtBox hurtBox, Entity initiator)
    {
        if (stunOnHit)
        {
            Stun();
            stunOnHit = false;
        }
        else
        {
            base.OnHitReceived(hitBox, hurtBox, initiator);
        }
    }

    public override void FixedUpdate()
    {
        if (behaviour != null)
        {
            behaviour.BehaviourFixedUpdate(this);
        }

        if (SmoothGravReset)
        {
            if (GravityScale != defaultGravityScale)
            {
                if (GravityScale < defaultGravityScale)
                {
                    GravityScale = Mathf.Min(GravityScale + gravChangeSpeed * Time.fixedDeltaTime, defaultGravityScale);
                }
                else
                {
                    GravityScale = Mathf.Max(GravityScale - gravChangeSpeed * Time.fixedDeltaTime, defaultGravityScale);
                }
            }
        }

        base.FixedUpdate();
    }

    public void Idle()
    {
        Behaviour = idleBehaviour;
    }

    public void Steal()
    {
        Behaviour = stealingBehaviour;
    }

    public void Escape()
    {
        Behaviour = escapingBehaviour;
    }
}
