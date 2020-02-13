using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedClosedTargetState : TargetState {

    public override void EnterState(Target target)
    {
        base.EnterState(target);
        target.DebugText.text = "LOCKED(closed)";
    }

    public override void HoldFocus(Target target, Focus entered)
    {
        DefaultHoldFocus(target, entered, closed: true);
    }
}
