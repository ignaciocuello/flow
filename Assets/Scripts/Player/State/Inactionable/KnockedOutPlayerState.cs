using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockedOutPlayerState : StandingPlayerState {

    [SerializeField]
    private Vector2 wadSpawnLocalPosition;

    [SerializeField]
    private int maxHitsForCompletion;

    private int wadsPerHit;

    [NonSerialized]
    public bool Actionable;

    public override void EnterState(Player player)
    {
        base.EnterState(player);

        player.IsAffectedByDrag(false);
        player.Velocity = Vector2.zero;

        wadsPerHit = Mathf.CeilToInt(player.Inventory.NumWads / maxHitsForCompletion);

        Actionable = false;
    }

    public override void StateFixedUpdate(Player player)
    {
        if (Actionable)
        {
            base.StateFixedUpdate(player);
        }
    }

    public override void StateOnHitReceived(Player player, HitBox hitBox, HurtBox hurtBox, Entity initiator)
    {
        ApplyHitStop(player, hitBox.HitData.HitStopDuration, hitBox.HitData.HitStopPower, receiving: true);

        if (player.Inventory.NumWads > 0)
        {
            int taken = Mathf.Min(wadsPerHit, player.Inventory.NumWads);

            player.Inventory.NumWads -= taken;

            for (int i = 0; i < taken; i++)
            {
                Wad wad = EntityFactory.Instance.Create<Wad>();
                wad.transform.position = player.transform.TransformPoint(wadSpawnLocalPosition);
            }
        }
    }

    public override void CheckGrounded(Player player, Dictionary<Vector2, GameObject[]> directionCollisionMap)
    {
        if (Actionable)
        {
            base.CheckGrounded(player, directionCollisionMap);
        }
    }

    public override string[] GetAvailableActions()
    {
        return new string[] { };
    }
}
