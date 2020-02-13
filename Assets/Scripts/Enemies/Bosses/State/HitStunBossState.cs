using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStunBossState : HitStunPlayerState {

    public override void EnterState(Player boss)
    {
        base.EnterState(boss);
        boss.DebugText.text = "HITSTUN";
        boss.GravityScale = 1.0f;
        boss.IsAffectedByDrag(false);
        boss.IgnoreBounce = false;
    }

    public override void ExitState(Player boss)
    {
        base.ExitState(boss);
        ((Boss)boss).SmoothGravReset = true;
        boss.IsAffectedByDrag(true);
        boss.IgnoreBounce = true;
    }
}
