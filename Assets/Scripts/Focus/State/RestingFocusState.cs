using System.Collections.Generic;
using UnityEngine;

public class RestingFocusState : FocusState
{
    public override void EnterState(Focus focus)
    {
        focus.DebugText.text = "RESTING";
    }
}
