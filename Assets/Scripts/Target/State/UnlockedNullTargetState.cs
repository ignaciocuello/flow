using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockedNullTargetState : TargetState {

    public override void EnterState(Target target)
    {
        base.EnterState(target);
        target.DebugText.text = "UNLOCKED(null)";
    }

    public override void HoldFocus(Target target, Focus entered)
    {
        DefaultHoldFocus(target, entered, closed: false);
    }

    public override void DropFocus(Target target)
    {
        DefaultDropFocus(target);
    }
}
