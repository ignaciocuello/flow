using UnityEngine;

public class StaticFocusState : FocusState {

    /* drag to apply to focus when in the static state */
    [SerializeField]
    protected float focusDrag;

    /* when true the focus will derive rest otherwise it will stay static*/
    public bool DeriveRest
    {
        get; set;
    }

    public void OnEnable()
    {
        if (!Descriptors.Contains(StateDescriptor.STATIC))
        {
            Descriptors.Add(StateDescriptor.STATIC);
        }
    }

    public override void EnterState(Focus focus)
    {
        //call before setting drag value as remove trajectory modifier will call End which resets drag
        if (focus.HasTrajectoryModifier())
        {
            focus.RemoveTrajectoryModifier();
        }

        focus.GravityScale = 0.0f;
        focus.Drag = focusDrag;

        focus.DebugText.text = "STATIC";
    }

    public override EntityState HitStunState(Focus focus)
    {
        if (DeriveRest)
        {
            return focus.HitStunState;
        }

        return focus.HitStunStaticState;
    }

    public override void ResetDrag(Focus focus)
    {
        focus.Drag = focusDrag;
    }

    public override void ResetGravityScale(Focus focus)
    {
        focus.GravityScale = 0.0f;
    }

    public override void ExitState(Focus focus)
    {
        focus.DefaultResetGravityScale();
        focus.DefaultResetDrag();
    }

    public override void Kill(Focus focus)
    {
        //DO NOTHING
    }

    public override void DeriveState(Focus focus)
    {
        if (DeriveRest)
        {
            focus.Rest();
        }
        else
        {
            focus.Static(false);
        }
    }
}
