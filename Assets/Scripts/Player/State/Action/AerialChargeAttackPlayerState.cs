using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialChargeAttackPlayerState : AerialPlayerState {

    [NonSerialized]
    public ChargeAttackPlayerState ChargeAttack;

    public override void EnterState(Player player)
    {
        base.EnterState(player);

        player.DebugText.text = "CHARGE(aerial)";
    }

    public override void StateFixedUpdate(Player player)
    {
        AerialFixedUpdate(player);
    }

    public override void SetAcceleratingText(Player player)
    {
        player.DebugText.text = "CHARGE(aerial, accelerating)";
    }

    public override void SetStillText(Player player)
    {
        player.DebugText.text = "CHARGE(aerial, still)";
    }

    public override void CheckState(Player player, Vector2 incomingVel, Dictionary<Vector2, GameObject[]> directionCollisionMap)
    {
        List<Vector2> directions = new List<Vector2>();
        directions.AddRange(directionCollisionMap.Keys);

        if (Mathf.Abs(player.VelY) < EPSILON && directions.Contains(Vector2.down))
        {
            player.VelY = 0.0f;
            ChargeAttack.Ground();
        }
    }
}
