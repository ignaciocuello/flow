using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStunStaticFocusState : StaticFocusState {

    [SerializeField]
    private GameObject bloodPrefab;

    public override void EnterState(Focus focus)
    {
        focus.GravityScale = 0.0f;
        focus.Drag = focusDrag;

        focus.DebugText.text = "STATIC HITSTUN";

        GameObject bloodObj = Instantiate(bloodPrefab, focus.transform);
        bloodObj.transform.position = focus.transform.position;
        bloodObj.transform.rotation *= Quaternion.Euler(-Mathf.Atan2(focus.VelY, focus.VelX) * Mathf.Rad2Deg, 0.0f, 0.0f);
    }
}
