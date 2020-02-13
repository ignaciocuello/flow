using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStunFocusState : FocusState {

    [SerializeField]
    private GameObject bloodPrefab;

    public override void EnterState(Focus focus)
    {
        base.EnterState(focus);

        focus.DebugText.text = "HITSTUN";

        GameObject bloodObj = Instantiate(bloodPrefab, focus.transform);
        bloodObj.transform.position = focus.transform.position;
        bloodObj.transform.rotation *= Quaternion.Euler(-Mathf.Atan2(focus.VelY, focus.VelX) * Mathf.Rad2Deg, 0.0f, 0.0f);
    }
}
