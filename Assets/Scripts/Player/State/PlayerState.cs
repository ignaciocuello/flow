using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState : EntityState<Player> {

    [SerializeField]
    private int downAttackBuffer = 10;
    public int DownAttackBuffer
    {
        get { return downAttackBuffer; }
    }

    //TODO: insert player hitstun/stop states here

    /* checks to see if any attack has been triggered */
    public void CheckForAnyAttacks(Player player)
    {
        if (InputBuffer.Instance.GetButtonDown(player.InputGenerator, PlayerAtomicAction.ATTACK.Name))
        {
            if (InputBuffer.Instance.CompareAxisBuffered(
                player.InputGenerator, "Vertical", axis => axis > 0.5f, maxBuffer: downAttackBuffer))
            {
                player.UpAttack();
            }
            else if (InputBuffer.Instance.CompareAxisBuffered(
                player.InputGenerator, "Vertical", axis => axis < -0.5f, maxBuffer: downAttackBuffer))
            {
                player.DownAttack();
            }
            else
            {
                player.NormalAttack();
            } 
        }
    }

    public override void DeriveState(Player player)
    {
        //set the state to standing if there is a surface beneath the player, otherwise set their
        //state to aerial.
        if (Grounded(player))
        {
            player.Stand();
        }
        else
        {
            player.Aerial();
        }
    }

    public bool Grounded(Player player)
    {
        List<Vector2> directions = new List<Vector2>();
        directions.AddRange(player.AdjacentSurfaceMapper.CalculateAdjacentSurfaceMap().Keys);

        return directions.Contains(Vector2.down);
    }

    public override EntityState HitStunState(Player player)
    {
        return player.HitStunState;
    }
}
