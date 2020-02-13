using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirDashPlayerState : AerialPlayerState {

    /* how long to accelerate, in frames */
    [Tooltip("How long to accelerate, in frames"), SerializeField]
    private int accelerationDuration;
    /* how much to accelerate by */
    [Tooltip("How much to accelerate by"), SerializeField]
    private float accelerationAmount;
    /* how long to sustain velocity, in frames */
    [Tooltip("How long to sustain velocity, in frames"), SerializeField]
    private int sustainDuration;
    /* how much to decelerate by */
    [Tooltip("How much to decelerate by"), SerializeField]
    private float decelerationAmount;

    /* the direction of the air dash */
    private Vector2 direction;

    public override void EnterState(Player player)
    {
        base.EnterState(player);

        player.Velocity = Vector2.zero;
        player.GravityScale = 0.0f;
        player.DebugText.text = "AIR DASH";
        direction = new Vector2(InputBuffer.Instance.GetAxis(player.InputGenerator, "Horizontal"), InputBuffer.Instance.GetAxis(player.InputGenerator, "Vertical")).normalized;
    }

    public override void StateFixedUpdate(Player player)
    {
        if (ScaledCurrentFrame < accelerationDuration)
        {
            player.AddForce(direction * accelerationAmount * player.Mass);
        }
        else if (ScaledCurrentFrame >= accelerationDuration + sustainDuration)
        {
            player.AddForce(-direction * decelerationAmount * player.Mass);
        }
    }

    public override void ExitState(Player player)
    {
        player.ResetGravityScale();
    }

}
