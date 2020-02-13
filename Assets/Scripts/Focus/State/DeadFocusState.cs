public class DeadFocusState : FocusState {

    public override void EnterState(Focus focus)
    {
        focus.DebugText.text = "DEAD";
    }

    public override void Kill(Focus focus)
    {
        //DO nothing
    }

    public override void DeriveState(Focus focus)
    {
        focus.Dead();
    }
}
