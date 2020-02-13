using UnityEngine;

public class StandingPlayerState : GroundedPlayerState {

    public override void EnterState(Player player)
    {
        base.EnterState(player);
        player.IsAffectedByDrag(true);
    }

    public override void StateFixedUpdate(Player player)
    {
        if (Mathf.Abs(player.VelX) > EPSILON)
        {
            //the player is still decelerating
            player.DebugText.text = "STANDING(moving)";
        }
        else
        {
            //the player is now standing still
            player.VelX = 0.0f;
            player.DebugText.text = "STANDING(still)";
        }

        //comparison with zero is fine here, since 'Input' has dead zone to cover this
        float horizontalValue = InputBuffer.Instance.GetAxisNoSnap(player.InputGenerator, "Horizontal");
        
        if (horizontalValue != 0.0f)
        {
            player.Run();
            return;
        }

        base.StateFixedUpdate(player);
    }
}
