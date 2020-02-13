using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class PlayerAttackEvent : UnityEvent<Player, AttackPlayerState>
{
}

[Serializable]
public class AttackPlayerState : PlayerState {

    public PlayerAttackEvent OnHitEvent;
    public PlayerAttackEvent OnJumpGainEvent;
    public PlayerAttackEvent OnEndEvent;

    [SerializeField]
    private float jumpGain;

    [SerializeField]
    private List<RandClip> whooshClips;
    [SerializeField, Tooltip("How many frames we wait before we play the whoosh sound")]
    private int whooshFrameDelay;

    [Space(20)]
    //TODO REMOVE
    [SerializeField]
    private List<RandClip> hitSounds;
    [SerializeField]
    private List<RandClip> thudSounds;

    [SerializeField]
    private List<TrajectoryModData> trajectoryMods;

    private bool hit;

    public override void EnterState(Player player)
    {
        base.EnterState(player);

        player.DebugText.text = "ATTACK";
        //not affected by drag during attack
        player.IsAffectedByDrag(false);

        //want boxes to be added as soon as this state(action) is entered
        BaseStateFixedUpdate(player);

        Duration = FrameData.Frames.Length;

        hit = false;
    }

    public override void StateFixedUpdate(Player player)
    {
        if (!hit && ScaledCurrentFrame == whooshFrameDelay)
        {
            player.PlayHitClip(RandClip.GetRandomClip(whooshClips).AudioClip);
        }

        ApplyTrajectoryMod(player);

        if (InputBuffer.Instance.GetButtonDownBuffered(player.InputGenerator, PlayerCompositeActions.JUMP.DisplayName(), consumeIfDown: true, maxBuffer: 1) && (Grounded(player) || hit))
        {
            JumpCancel(player);
        }
    }

    private void JumpCancel(Player player)
    {
        player.VelY = jumpGain;
        player.Aerial();
    }

    private void ApplyTrajectoryMod(Player player)
    {
        if (trajectoryMods.Count > 0 && !player.HasTrajectoryModifier())
        {
            player.Impact(trajectoryMods[0]);
            trajectoryMods.RemoveAt(0);
        }
    }

    public override void StateOnHit(Player player, HitBox hitBox, HurtBox hurtBox, Entity receiving)
    {
        base.StateOnHit(player, hitBox, hurtBox, receiving);

        RandClip hitSound = hitSounds[player.ConsecutiveHits % hitSounds.Count];

        player.PlayThudClip(RandClip.GetRandomClip(thudSounds).AudioClip);
        player.PlayHitClip(RandClip.GetRandomClip(hitSounds).AudioClip);

        hit = true;

        //jump if holding jump
        if (InputBuffer.Instance.GetButton(player.InputGenerator, PlayerCompositeActions.JUMP.DisplayName()))
        {
            player.VelY = jumpGain;
            StatsTracker.Instance.NamedTallier.AddToNamedTally("Jump Gain");

            OnJumpGainEvent.Invoke(player, this);
        }

        OnHitEvent.Invoke(player, this);
    }

    public override void ExitState(Player player)
    {
        BoxManager.Instance.ClearFromExemptOnFixedUpdateEnd(player);

        OnEndEvent.Invoke(player, this);
    }
}
