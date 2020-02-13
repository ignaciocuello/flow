using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedOpenTargetState : TargetState {

    public override void EnterState(Target target)
    {
        base.EnterState(target);
        target.DebugText.text = "LOCKED(open)";
    }

    public override void HoldFocus(Target target, Focus entered)
    {
        DefaultHoldFocus(target, entered, closed: false);
    }
}
