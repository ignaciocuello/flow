using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttackPlayerState : AttackPlayerState {

    public override void StateFixedUpdate(Player player)
    {
        if (InputBuffer.Instance.GetAxis("Vertical") < -0.5f)
        {
            player.DownAttack();
            return;
        }

        base.StateFixedUpdate(player);
    }
}
