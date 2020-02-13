using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockedClosedTargetState : TargetState {

    public override void EnterState(Target target)
    {
        base.EnterState(target);
        target.DebugText.text = "UNLOCKED(closed)";
    }
}
