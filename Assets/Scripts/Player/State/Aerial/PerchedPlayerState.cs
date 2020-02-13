using System.Collections.Generic;
using UnityEngine;

public class PerchedPlayerState : AerialPlayerState
{
    /* the velocity that will be imparted upon the player if they wall jump from a wall to their left
     * i.e. a right wall jump */
    [SerializeField]
    private Vector2 wallJumpVelocity;
    [SerializeField]
    private int wallJumpBuffer = 5;
    /* the reduction applied to the vertical component of the wall jump velocity when repeated jumps are made on
     * the same surface, without first touching another surface */
    [SerializeField]
    private float wallJumpVelocityDamping;

    /* how much time we allow the player to take before they get no boost from their enter velocity, assuming
     * it was faster than the wall jump velocity horizontal component, this value is in frames */
    [SerializeField]
    private int enterVelocityLifeTime;

    /* surface which we're perching on */
    protected GameObject surface;
    public GameObject Surface
    {
        set
        {
            surface = value;
        }
    }

    /* direction in which surface we're perching in is */
    protected Vector2 surfaceDirection;
    public Vector2 SurfaceDirection
    {
        set
        {
            surfaceDirection = value;
        }
    }

    /* velocity of player when they enter this state */
    protected Vector2 enterVelocity;
    public Vector2 EnterVelocity
    {
        set
        {
            enterVelocity = value;
        }
    }

    public override void EnterState(Player player)
    {
        base.EnterState(player);
        player.DebugText.text = "PERCHED";
    }

    public override void StateFixedUpdate(Player player)
    {
        base.StateFixedUpdate(player);

        if (InputBuffer.Instance.GetButtonDownBuffered(
            player.InputGenerator, "Jump", consumeIfDown: true, maxBuffer: wallJumpBuffer))
        {
            WallJump(player);
        }

        player.DebugText.text = "PERCHED";
    }

    public void WallJump(Player player)
    {
        //BREAK HERE
        float velX = (ScaledCurrentFrame  < enterVelocityLifeTime) ? 
            Mathf.Max(wallJumpVelocity.x, Mathf.Abs(enterVelocity.x)) : wallJumpVelocity.x;
        velX = Mathf.Max(velX, Mathf.Abs(player.VelX));

        //IDEA: make y velocity increase with VelX by a factor of (velX / wallJumpVelocity.x) * someDampingFactor
        float velY = wallJumpVelocity.y;

        Vector2 jumpVel = 
            player.LastWallJumpSurface != surface ? 
                new Vector2(velX, velY) : new Vector2(velX, velY * wallJumpVelocityDamping);

        player.LastWallJumpSurface = surface;
        if (surfaceDirection == Vector2.left)
        {
            player.Velocity = jumpVel;
            player.FacingRight = true;
        }
        else if (surfaceDirection == Vector2.right)
        {
            player.Velocity = new Vector2(-jumpVel.x, jumpVel.y);
            player.FacingRight = false;
        }
        else if (surfaceDirection == Vector2.up)
        {
            player.Velocity = new Vector2(0.0f, -jumpVel.y);
        }

        player.WallJump(surface, surfaceDirection);
    }

    public override void CheckState(Player player, 
        Vector2 incomingVel, Dictionary<Vector2, GameObject[]> directionCollisionMap)
    {
        List<Vector2> directions = new List<Vector2>();
        directions.AddRange(directionCollisionMap.Keys);

        if (Mathf.Abs(player.VelY) < EPSILON && directions.Contains(Vector2.down))
        {
            player.VelY = 0.0f;
            player.Stand();
        }
        else if (directions.Contains(Vector2.left))
        {
            if (surfaceDirection != Vector2.left || surface != directionCollisionMap[Vector2.left][0])
            {
                player.Perch(directionCollisionMap[Vector2.left][0], Vector2.left, incomingVel);
            }
        }
        else if (directions.Contains(Vector2.right))
        {
            if (surfaceDirection != Vector2.right || surface != directionCollisionMap[Vector2.right][0])
            {
                player.Perch(directionCollisionMap[Vector2.right][0], Vector2.right, incomingVel);
            }
        }
        else if (directions.Contains(Vector2.up))
        {
            if (surfaceDirection != Vector2.up || surface != directionCollisionMap[Vector2.up][0])
            {
                player.Perch(directionCollisionMap[Vector2.up][0], Vector2.up, incomingVel);
            }
        }
        else
        {
            player.Aerial();
        }
    }

    public override string[] GetAvailableActions()
    {
        return new string[]
        {
            PlayerCompositeActions.DRIFT.DisplayName(),
            PlayerCompositeActions.WALL_JUMP.DisplayName(),
            PlayerCompositeActions.NORMAL_ATTACK.DisplayName(),
            PlayerCompositeActions.DOWN_ATTACK.DisplayName()
        };
    }
}
