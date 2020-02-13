using UnityEngine;

public abstract class FocusState : EntityState<Focus> {

    public virtual void Kill(Focus focus)
    {
        focus.Dead();
        //remove any hurboxes that have been added to the box manager
        BoxManager.Instance.RemoveBoxes(focus);

        focus.DestroyGameObject();
    }

    public override void DeriveState(Focus focus)
    {
        focus.Rest();
    }

    public override EntityState HitStunState(Focus focus)
    {
        return focus.HitStunState;
    }
}
