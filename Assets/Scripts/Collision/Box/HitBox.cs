using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class HitBox : Box {

    public static readonly Color DRAW_COLOR = Color.red;

    private static HashSet<string> validBoxInteractions = new HashSet<string> { "HurtBox", "HitBox" };

    public HitData HitData;

    public HitBox()
    {
    }

    public HitBox(Vector2 impact, float angularImpact, HitBox hitBox) : base(hitBox.BoundsData, hitBox.Id)
    {
        HitData = hitBox.HitData;
        HitData.TrajectoryModData.Impact = impact;
        HitData.TrajectoryModData.AngularImpact = angularImpact;
    }

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
        if (box is HitBox)
        {
            //TODO: deal with this appropratiely later
            Debug.Log("CLASH");
            return;
        }

        Vector2 impact = HitData.TrajectoryModData.Impact;
        float angularImpact = HitData.TrajectoryModData.AngularImpact;
        if (!owner1.FacingRight)
        {
            impact.x *= -1.0f;
            angularImpact *= -1.0f;
        }

        HitBox hitBox = new HitBox(impact + HitData.MomentumGainData.GetMomentumGain(owner1.Velocity), angularImpact, this);
        owner1.OnHit(hitBox, (HurtBox)box, owner2);
        owner2.OnHitReceived(hitBox, (HurtBox)box, owner1);
    }
}
