using UnityEngine;

public class RunningPlayerState : GroundedPlayerState {

    /* how much we allow the user's speed to be over 'horizontal speed' before applying friction */
    [SerializeField]
    private float speedGive;

    /* the maximum horizontal speed achieved by a run */
    [SerializeField]
    private float horizontalSpeed;
    public float HorziontalSpeed
    {
        get { return horizontalSpeed; }
        set { horizontalSpeed = value; }
    }

    private float originalHorizontalSpeed;
    public float OriginalHorizontalSpeed
    {
        get { return originalHorizontalSpeed; }
    }

    /* how quickly the player accelerates to the maximum horizontal speed */
    [SerializeField]
    private float runningAcceleration;

    public void OnEnable()
    {
        originalHorizontalSpeed = horizontalSpeed;
    }

    public override void StateFixedUpdate(Player player)
    {
        RunningFixedUpdate(player);
        base.StateFixedUpdate(player); 
    }

    public void RunningFixedUpdate(Player player)
    {
        float horizontalValue = InputBuffer.Instance.GetAxisNoSnap(player.InputGenerator, "Horizontal");
        if (horizontalValue != 0.0f)
        {
            //make the player frictionless while running
            player.IsAffectedByDrag(false);

            //the direction the user wants to run in
            float runInputDirection = horizontalValue < 0.0f ? -1.0f : 1.0f;
            player.FacingRight = runInputDirection == 1.0f;

            float currentHorizontalSpeed = Mathf.Abs(player.VelX);
            bool changedDirection = runInputDirection * player.VelX < 0.0f;
            //apply acceleration if player has not reached the horizontal speed
            //cap or we're moving in a different direction
            if (currentHorizontalSpeed < horizontalSpeed || changedDirection)
            {
                if (changedDirection)
                {
                    //stop the player's current movement, so they turn around instantly
                    currentHorizontalSpeed = 0.0f;
                    player.VelX = 0.0f;
                }

                //accelerate the player in the direction they wish to run
                player.AddForce(
                    runInputDirection * runningAcceleration * player.Mass * Vector2.right);
                SetAcceleratingText(player);
            }
            else if (currentHorizontalSpeed < horizontalSpeed + speedGive)
            {
                //we have reached max run velocity
                SetCappedText(player);
                player.IsAffectedByDrag(false);
                player.VelX = Mathf.Clamp(player.VelX, -horizontalSpeed, horizontalSpeed);
            }
            else
            {
                SetDeceleratingText(player);
                player.IsAffectedByDrag(true);
                //Debug.Break();
            }
        }
        else if (Mathf.Abs(player.VelX) > EPSILON)
        {
            SetDeceleratingText(player);
            //make the player be affected by drag while not inputting a run
            player.IsAffectedByDrag(true);
        }
        else
        {
            Stop(player);
        }
    }

    public void ResetHorizontalSpeed()
    {
        horizontalSpeed = originalHorizontalSpeed;
    }

    public virtual void SetAcceleratingText(Player player)
    {
        player.DebugText.text = "RUNNING(accelerating)";
    }

    public virtual void SetCappedText(Player player)
    {
        player.DebugText.text = "RUNNING(capped)";
    }

    public virtual void SetDeceleratingText(Player player)
    {
        player.DebugText.text = "RUNNING(decelerating)";
    }

    public virtual void Stop(Player player)
    {
        //the player is at a still and their state changes to standing
        player.VelX = 0.0f;
        player.Stand();
    }
}
