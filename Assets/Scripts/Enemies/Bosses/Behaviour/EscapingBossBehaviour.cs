using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapingBossBehaviour : BehaviourState<Boss> {

    public override void BehaviourEnter(Boss entity)
    {
        base.BehaviourEnter(entity);
    }

    public override void BehaviourFixedUpdate(Boss boss)
    {
        AIInputGenerator ai = (AIInputGenerator)boss.InputGenerator;
        ai.Reset();

        if (boss.State is GroundedPlayerState)
        {
            ai.SetAxis("Horizontal", -1.0f);
        }
    }

}
