using System.Collections.Generic;
using UnityEngine;

public class AerialPlayerState : PlayerState {

    /* the maximum horizontal speed achieved by a strafe */
    [SerializeField]
    protected float airStrafeSpeed;

    /* how quickly the player can strafe in the air */
    [SerializeField]
    protected float airStrafeAcceleration;

    [SerializeField]
    private float fallingGravity = 10.5f;

    [SerializeField]
    private int turnBuffer = 20;

    public override void EnterState(Player player)
    {
        player.IsAffectedByDrag(false);
        player.DebugText.text = "AERIAL";
    }

    public override void StateFixedUpdate(Player player)
    {
        AerialFixedUpdate(player);

        CheckForAnyAttacks(player);
    }

    public void AerialFixedUpdate(Player player)
    {
        if (InputBuffer.Instance.CompareAxisMultipleBuffered(player.InputGenerator, "Horizontal", 2, axis => axis > 0.5f, true, turnBuffer))
        {
            player.FacingRight = true;
        }
        else if (InputBuffer.Instance.CompareAxisMultipleBuffered(player.InputGenerator, "Horizontal", 2, axis => axis < -0.5f, true, turnBuffer))
        {
            player.FacingRight = false;
        }

        float airStrafeValue = InputBuffer.Instance.GetAxis(player.InputGenerator, "Horizontal");
        if (airStrafeValue != 0.0f)
        {
            //the direction the user wants to air strafe in
            float airStrafeInputDirection = airStrafeValue < 0.0f ? -1.0f : 1.0f;
            AirStrafe(player, airStrafeInputDirection, airStrafeAcceleration);
        }
        else
        {
            SetStillText(player);
        }
    }

    public virtual void AirStrafe(Player player, float airStrafeInputDirection, float accel)
    {
        float currentHorizontalSpeed = Mathf.Abs(player.VelX);
        bool changedDirection = airStrafeInputDirection * player.VelX < 0.0f;
        //apply drift acceleration if player has not reached the horizontal
        //speed cap or we're drifting in a different direction
        if (currentHorizontalSpeed < airStrafeSpeed || changedDirection)
        {
            //accelerate the player in the direction they wish to drift
            player.AddForce(
                airStrafeInputDirection * accel * player.Mass * Vector2.right);
            SetAcceleratingText(player);
        }
    }

    public override void ExitState(Player player)
    {
        player.DefaultResetGravityScale();
    }

    public virtual void SetAcceleratingText(Player player)
    {
        player.DebugText.text = "AERIAL(accelerating)";
    }

    public virtual void SetStillText(Player player)
    {
        player.DebugText.text = "AERIAL(still)";
    }

    public override void StateOnAdjacentSurfaceMap(Player player, Dictionary<Vector2, GameObject[]> adjacentSurfaceMap)
    {
        CheckState(player, player.Velocity, adjacentSurfaceMap);
    }

    public override void StateOnCollisionEnter2D(
        Player player, Collision2D collision, Dictionary<Vector2, GameObject[]> directionCollisionMap)
    {
        CheckState(player, -collision.relativeVelocity, directionCollisionMap);
    }

    public override void StateOnCollisionStay2D(Player player, Collision2D collision, Dictionary<Vector2, GameObject[]> directionCollisionMap)
    {
        CheckState(player, -collision.relativeVelocity, directionCollisionMap);
    }

    public override void StateOnCollisionExit2D(Player player, Collision2D collision, Dictionary<Vector2, GameObject[]> directionCollisionMap)
    {
        CheckState(player, -collision.relativeVelocity, directionCollisionMap);
    }

    public virtual void CheckState(Player player, Vector2 incomingVel, Dictionary<Vector2, GameObject[]> directionCollisionMap)
    {
        List<Vector2> directions = new List<Vector2>();
        directions.AddRange(directionCollisionMap.Keys);

        if (Mathf.Abs(player.VelY) < EPSILON && directions.Contains(Vector2.down))
        {
            player.Stand();
        }
        else if (directions.Contains(Vector2.left))
        {
            player.Perch(directionCollisionMap[Vector2.left][0], Vector2.left, incomingVel);
        }
        else if (directions.Contains(Vector2.right))
        {
            player.Perch(directionCollisionMap[Vector2.right][0], Vector2.right, incomingVel);
        }
        else if (directions.Contains(Vector2.up))
        {
            player.Perch(directionCollisionMap[Vector2.up][0], Vector2.up, incomingVel);
        }
    }

    public override string[] GetAvailableActions()
    {
        return new string[] {
            PlayerCompositeActions.DRIFT.DisplayName(),
            PlayerCompositeActions.NORMAL_ATTACK.DisplayName(),
            PlayerCompositeActions.DOWN_ATTACK.DisplayName()
        };
    }
}
