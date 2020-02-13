using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class HurtBox : Box {

    public static readonly Color DRAW_COLOR = Color.green;

    private static HashSet<string> validBoxInteractions = new HashSet<string> { "HitBox" };

    public override Color GetDrawColor()
    {
        return DRAW_COLOR;
    }

    public override bool ValidBoxInteraction(Box box)
    {
        return validBoxInteractions.Contains(box.GetType().Name);
    }

    public override void OnItersect(Box box, Entity owner1, Entity owner2)
    {
    }
}
