using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : Entity
{
    [SerializeField]
    private PhysicsMaterial2D frictionlessPhysMat;
    [SerializeField]
    private PhysicsMaterial2D normalPhysMat;

    /* grounded states */
    [SerializeField]
    private StandingPlayerState standingState;
    [SerializeField]
    private RunningPlayerState runningState;
    public RunningPlayerState RunningState
    {
        get { return runningState; }
    }

    [Space(10)]

    /* aerial states */
    [SerializeField]
    private AerialPlayerState aerialState;
    [SerializeField]
    private PerchedPlayerState perchedState;
    [SerializeField]
    private WallJumpingPlayerState wallJumpState;

    /* inactionable states */
    [Space(10)]
    [SerializeField]
    private StunnedPlayerState stunnedState;
    [SerializeField]
    private KnockedOutPlayerState knockedOutState;

    [Space(10)]

    /* attacks */
    [SerializeField]
    private ChargeAttackPlayerState chargeNormalAttack;
    //[SerializeField]
    //private ChargeAttackPlayerState chargeUpAttack;
    [SerializeField]
    private ChargeAttackPlayerState chargeDownAttack;

    [SerializeField]
    private HitStunPlayerState hitStunState;
    public HitStunPlayerState HitStunState
    {
        get { return hitStunState; }
    }

    [SerializeField]
    private AudioSource hitAudioSource;
    [SerializeField]
    private AudioSource thudAudioSource;

    //the last frame in which a hit was connected
    private int lastHitFrame;

    public int ConsecutiveHits { get; private set; }

    /* the last surface that was used to wall jump */
    public GameObject LastWallJumpSurface
    {
        get; set;
    }

    private Inventory inventory;
    public Inventory Inventory
    {
        get { return inventory; }
    }

    protected IInputGenerator inputGenerator;
    public IInputGenerator  InputGenerator
    {
        get { return inputGenerator; }
    }

    [SerializeField]
    private int playerId;
    public int PlayerId
    {
        get { return playerId; }
        set { playerId = value; }
    }

    public override void Awake()
    {
        base.Awake();

        inventory = GetComponent<Inventory>();

        Stand();
    }

    public virtual void Start()
    {
        inputGenerator = new PlayerInputGenerator(playerId);
        InputBuffer.Instance.AddInputGenerator(inputGenerator);
    }

    public override void InitStates()
    {
        base.InitStates();

        standingState = Instantiate(standingState);
        runningState = Instantiate(runningState);

        aerialState = Instantiate(aerialState);
        perchedState = Instantiate(perchedState);
        wallJumpState = Instantiate(wallJumpState);

        stunnedState = Instantiate(stunnedState);
        knockedOutState = Instantiate(knockedOutState);

        chargeNormalAttack = Instantiate(chargeNormalAttack);
        chargeNormalAttack.InitStates();
        /*chargeUpAttack = Instantiate(chargeUpAttack);
        chargeUpAttack.InitStates();*/
        chargeDownAttack = Instantiate(chargeDownAttack);
        chargeDownAttack.InitStates();

        hitStunState = Instantiate(hitStunState);
    }

    //TODO: move elsewhere
    public void OnHitThreshold(int frameThreshold)
    {
        int currentFrame = GameManager.Instance.FrameCounter;
        if (currentFrame - lastHitFrame <= frameThreshold)
        {
            ConsecutiveHits++;
        }
        else
        {
            ConsecutiveHits = 0;
        }
        lastHitFrame = currentFrame;
    }

    public void Stand()
    {
        State = standingState;
    }

    public void Run()
    {
        State = runningState;
    }

    public void ResetHorizontalSpeed()
    {
        runningState.ResetHorizontalSpeed();
        chargeNormalAttack.GroundedState.ResetHorizontalSpeed();
    }

    public void SetHorizontalSpeed(float horizontalSpeed)
    {
        runningState.HorziontalSpeed = horizontalSpeed;
        chargeNormalAttack.GroundedState.HorziontalSpeed = horizontalSpeed;
    }

    public void Aerial()
    {
        State = aerialState;
    }

    public void Perch(GameObject surface, Vector2 surfaceDirection, Vector2 enterVelocity)
    {
        perchedState.Surface = surface;
        perchedState.SurfaceDirection = surfaceDirection;
        perchedState.EnterVelocity = enterVelocity;

        State = perchedState;
    }

    public void WallJump(GameObject surface, Vector2 surfaceDirection)
    {
        wallJumpState.Surface = surface;
        wallJumpState.SurfaceDirection = surfaceDirection;

        State = wallJumpState;
    }

    public void NormalAttack()
    {
        ChargeAttack(chargeNormalAttack);
    }

    public void UpAttack()
    {
        //ChargeAttack(chargeUpAttack);
    }

    public void DownAttack()
    {
        ChargeAttack(chargeDownAttack);
    }

    private void ChargeAttack(ChargeAttackPlayerState attack)
    {
        attack.Charge.InputCheckFunc =
            () => InputBuffer.Instance.GetButton(inputGenerator, PlayerAtomicAction.ATTACK.Name);
        State = attack;
    }

    public void Stun()
    {
        State = stunnedState;
    }

    public void KnockOut()
    {
        State = knockedOutState;
    }

    public void ActivateKnockout()
    {
        knockedOutState.Actionable = true;
    }

    public void IsAffectedByDrag(bool isAffectedByDrag, float drag)
    {
        if (isAffectedByDrag)
        {
            PhysMat = normalPhysMat;
            Drag = drag;
        }
        else
        {
            PhysMat = frictionlessPhysMat;
            Drag = 0.0f;
        }
    }

    public void IsAffectedByDrag(bool isAffectedByDrag)
    {
        IsAffectedByDrag(isAffectedByDrag, defaultDrag);
    }

    public void PlayHitClip(AudioClip hitClip)
    {
        PlayHitClip(hitClip, volumeScale: 1.0f);
    }

    public void PlayHitClip(AudioClip hitClip, float volumeScale)
    {
        PlayClip(hitAudioSource, hitClip, volumeScale);
    }

    public void StopHitAudioSource()
    {
        hitAudioSource.Stop();
    }

    public void PlayThudClip(AudioClip thudClip)
    {
        PlayThudClip(thudClip, volumeScale: 1.0f);
    }

    public void PlayThudClip(AudioClip thudClip, float volumeScale)
    {
        PlayClip(thudAudioSource, thudClip, volumeScale);
    }

    public void StopThudAudioSource()
    {
        thudAudioSource.Stop();
    }

    private void PlayClip(AudioSource source, AudioClip clip, float volumeScale)
    {
        source.volume = volumeScale;
        source.clip = clip;
        source.Play();
    }
}
