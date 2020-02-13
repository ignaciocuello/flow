using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelText : VisibleAnimatable {

    public override void Awake()
    {
        base.Awake();

        VisibleAnimator.Visible = true;
        VisibleAnimator.OnFullyInvisible += () => Destroy(gameObject);
    }
}
