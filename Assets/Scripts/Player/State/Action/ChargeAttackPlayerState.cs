using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAttackPlayerState : PlayerState {

    [SerializeField]
    private Charge charge;
    public Charge Charge
    {
        get { return charge; }
    }

    [SerializeField]
    private AerialChargeAttackPlayerState aerial;
    public AerialChargeAttackPlayerState AerialState
    {
        get { return aerial; }
    }

    [SerializeField]
    private GroundedChargeAttackPlayerState grounded;
    public GroundedChargeAttackPlayerState GroundedState
    {
        get { return grounded; }
    }

    [Tooltip("If true will allow user to change direction of normal attack while in charge"), SerializeField]
    private bool changeAttackDirection;

    private PlayerState currentState;

    public void InitStates()
    {
        aerial = Instantiate(aerial);
        aerial.ChargeAttack = this;

        grounded = Instantiate(grounded);
        grounded.ChargeAttack = this;
    }

    public void Ground()
    {
        currentState = grounded;
    }

    public void Aerial()
    {
        currentState = aerial;
    }

    public override void EnterState(Player player)
    {
        Dictionary<Vector2, GameObject[]> adj = player.AdjacentSurfaceMapper.CalculateAdjacentSurfaceMap();
        if (adj.ContainsKey(Vector2.down))
        {
            Ground();
        }
        else
        {
            Aerial();
        }

        charge.InputReleased = false;
        charge.ChargeFraction = 0.0f;

        currentState.EnterState(player);
    }

    public override void StateFixedUpdate(Player player)
    {
        if (!Charge.InputCheckFunc())
        {
            Charge.InputReleased = true;
        }
        else
        {
            Charge.ChargeFraction = ScaledCurrentFrame / Duration;
            Charge.ChangePlayerChargeColor(player);

            currentState.StateFixedUpdate(player);

            if (changeAttackDirection && InputBuffer.Instance.CompareAxisBuffered(player.InputGenerator, "Vertical", axis => axis < -0.5f, DownAttackBuffer))
            {
                player.DownAttack();
                //unscaledCurrentFrame + 1 to account for the fact that the value of unscaledCurrentFrame is incremented
                //after StateFixedUpdate is called
                player.State.Advance(player, numFrames: (unscaledCurrentFrame+1));
            }
        }
    }

    public override void DeriveState(Player player)
    {
        currentState.DeriveState(player);
    }

    public override EntityState HitStunState(Player player)
    {
        return currentState.HitStunState(player);
    }

    public override void StateOnCollisionEnter2D(
        Player player, Collision2D collision, Dictionary<Vector2, GameObject[]> directionCollisionMap)
    {
        currentState.StateOnCollisionEnter2D(player, collision, directionCollisionMap);
    }

    public override void StateOnCollisionStay2D(Player player, Collision2D collision, Dictionary<Vector2, GameObject[]> directionCollisionMap)
    {
        currentState.StateOnCollisionStay2D(player, collision, directionCollisionMap);
    }

    public override void StateOnCollisionExit2D(Player player, Collision2D collision, Dictionary<Vector2, GameObject[]> directionCollisionMap)
    {
        currentState.StateOnCollisionExit2D(player, collision, directionCollisionMap);
    }

    public override string[] GetAvailableActions()
    {
        return currentState.GetAvailableActions();
    }

    public override void ExitState(Player player)
    {
        if (IsFinished())
        {
            Charge.ChargeFraction = Duration != 0 ? ScaledCurrentFrame / Duration : 0.0f;
            Charge.PerformChargedAttack(player);
        }
    }

    public override bool IsFinished()
    {
        return base.IsFinished() || Charge.InputReleased;
    }
}
