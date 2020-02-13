using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestingBouncerState : BouncerState {

    public override void EnterState(Bouncer bouncer)
    {
        base.EnterState(bouncer);
        bouncer.DebugText.text = "RESTING";
    }
}
