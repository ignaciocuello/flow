using System.Collections.Generic;
using UnityEngine;

public abstract class GroundedPlayerState : PlayerState {

    /* the upwards velocity applied to the player when they jump while grounded */
    [SerializeField]
    private float groundedJumpSpeed;
    [SerializeField]
    private int jumpBuffer = 5;

    public override void EnterState(Player player)
    {
        //when grounded the player will be affected by drag, by default
        player.IsAffectedByDrag(true);
        //reset wall jump surface
        player.LastWallJumpSurface = null;
    }

    public override void StateFixedUpdate(Player player)
    {
        if (InputBuffer.Instance.GetButtonDownBuffered(
                player.InputGenerator, "Jump", consumeIfDown: true, maxBuffer: jumpBuffer))
        {
            GroundedJump(player);
        }
        else
        {
            CheckForAnyAttacks(player);
        }
    }

    public void ApplyJumpSpeed(Player player)
    {
        player.VelY += groundedJumpSpeed;
    }

    public void GroundedJump(Player player)
    {
        ApplyJumpSpeed(player);
        player.Aerial();
        StatsTracker.Instance.NamedTallier.AddToNamedTally("Jump");
    }

    public override void StateOnAdjacentSurfaceMap(Player player, Dictionary<Vector2, GameObject[]> adjacentSurfaceMap)
    {
        CheckGrounded(player, adjacentSurfaceMap);
    }

    public override void StateOnCollisionExit2D(
        Player player, Collision2D collision, Dictionary<Vector2, GameObject[]> directionCollisionMap)
    {
        CheckGrounded(player, directionCollisionMap);
    }

    public virtual void CheckGrounded(Player player, Dictionary<Vector2, GameObject[]> directionCollisionMap)
    {
        List<Vector2> directions = new List<Vector2>();
        directions.AddRange(directionCollisionMap.Keys);

        //if we have left the ground
        if (!directions.Contains(Vector2.down))
        {
            player.Aerial();
        }
    }

    public override string[] GetAvailableActions()
    {
        return new string[] {
            PlayerCompositeActions.RUN.DisplayName(),
            PlayerCompositeActions.JUMP.DisplayName(),
            PlayerCompositeActions.NORMAL_ATTACK.DisplayName(),
            PlayerCompositeActions.DOWN_ATTACK.DisplayName()
        };
    }
}
