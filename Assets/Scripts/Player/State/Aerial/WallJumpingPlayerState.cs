using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumpingPlayerState : AerialPlayerState {
    
    /* direction in which surface we wall jumped from */
    protected Vector2 surfaceDirection;
    public Vector2 SurfaceDirection
    {
        set
        {
            surfaceDirection = value;
        }
    }

    /* surface which we're perching on */
    protected GameObject surface;
    public GameObject Surface
    {
        set
        {
            surface = value;
        }
    }

    public override void EnterState(Player player)
    {
        base.EnterState(player);
        //Debug.Break();
        player.DebugText.text = "WALL JUMPING";
    }

    public override void AirStrafe(Player player, float airStrafeInputDirection, float accel)
    {
        float newAccel = (airStrafeInputDirection * surfaceDirection.x > 0.0f ? /*(ScaledCurrentFrame / Duration)*/ 0.0f : 1.0f) * airStrafeAcceleration;

        base.AirStrafe(player, airStrafeInputDirection, newAccel);
        player.DebugText.text = "WALL JUMPING";
    }

    public override void CheckState(Player player, Vector2 incomingVel, Dictionary<Vector2, GameObject[]> directionCollisionMap)
    {
        List<Vector2> directions = new List<Vector2>();
        directions.AddRange(directionCollisionMap.Keys);

        if (Mathf.Abs(player.VelY) < EPSILON && directions.Contains(Vector2.down))
        {
            player.Stand();
        }
        else if (directions.Contains(Vector2.left) && directionCollisionMap[Vector2.left][0] != surface)
        {
            player.Perch(directionCollisionMap[Vector2.left][0], Vector2.left, incomingVel);
        }
        else if (directions.Contains(Vector2.right) && directionCollisionMap[Vector2.right][0] != surface)
        {
            player.Perch(directionCollisionMap[Vector2.right][0], Vector2.right, incomingVel);
        }
        else if (directions.Contains(Vector2.up) && directionCollisionMap[Vector2.up][0] != surface)
        {
            player.Perch(directionCollisionMap[Vector2.up][0], Vector2.up, incomingVel);
        }
    }
}
