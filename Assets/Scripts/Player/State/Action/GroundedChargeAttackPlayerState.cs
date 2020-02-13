using System;
using System.Collections.Generic;
using UnityEngine;

public class GroundedChargeAttackPlayerState : RunningPlayerState {

    [NonSerialized]
    public ChargeAttackPlayerState ChargeAttack;

    public override void EnterState(Player player)
    {
        base.EnterState(player);

        //first frame player is unaffected by drag
        player.IsAffectedByDrag(false);
        player.DebugText.text = "CHARGE(grounded)";
    }

    public override void StateFixedUpdate(Player player)
    {
        RunningFixedUpdate(player);

        if (InputBuffer.Instance.GetButtonDownBuffered(
            player.InputGenerator, "Jump", consumeIfDown: true, maxBuffer: DownAttackBuffer))
        {
            ApplyJumpSpeed(player);
            ChargeAttack.Aerial();
        }
    }

    public override void SetAcceleratingText(Player player)
    {
        player.DebugText.text = "CHARGE(grounded, accelerating)";
    }

    public override void SetCappedText(Player player)
    {
        player.DebugText.text = "CHARGE(grounded, capped)";
    }

    public override void SetDeceleratingText(Player player)
    {
        player.DebugText.text = "CHARGE(grounded, decelerating)";
    }

    public override void Stop(Player player)
    {
        player.IsAffectedByDrag(true);
        player.DebugText.text = "CHARGE(grounded, still)";
    }

    public override void CheckGrounded(Player player, Dictionary<Vector2, GameObject[]> directionCollisionMap)
    {
        //if we have left the ground and have a non neglible velocity in the y direction
        //change the state to aerial
        if (Mathf.Abs(player.VelY) > EPSILON && !directionCollisionMap.ContainsKey(Vector2.down))
        {
            ChargeAttack.Aerial();
        }
    }
}
