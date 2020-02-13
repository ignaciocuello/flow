using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestingWadState : WadState {

    public override void EnterState(Wad wad)
    {
        base.EnterState(wad);
        wad.DebugText.text = "RESTING";
    }
}
