using UnityEngine;

public class Level4Tutorial : Tutorial {

    private Player player;

    [SerializeField]
    private Vector3 cameraOffset;

    public void OnPlayerEnteredJumpArea(Player player)
    {
        if (this.player == null)
        {
            this.player = player;

            ControllerPromptManager.Instance.AssignActionToMainControllerPrompt(
                PlayerCompositeActions.JUMP.DisplayName());
            player.OnEnterStateEvent.AddListener(OnEnterState);
        }
    }

    public void OnPlayerExitedJumpArea(Player player)
    {
        StartCoroutine(DelayDisableMainControlPrompt(PromptDestroyDelayTime, ControllerPromptManager.Instance.MainPrompt, destroySelf: false));
    }

    public void OnEnterState(EntityState enState)
    {
        if (enState is AerialPlayerState)
        {
            player.OnEnterStateEvent.RemoveListener(OnEnterState);
        }
    }

    public void OnPlayerEnteredWallJumpArea(Player player) {
        ControllerPromptManager.Instance.AssignActionToMainControllerPrompt(
            PlayerCompositeActions.JUMP.DisplayName(), "(Next to Wall) Wall Jump");

        Camera.main.GetComponentInParent<TransformTracker>().Offset = cameraOffset;
    }

    public void OnPlayerExitedWallJumpArea(Player player)
    {
        ControllerPromptManager.Instance.DisableMainControllerPrompt();
        Camera.main.GetComponentInParent<TransformTracker>().Offset = Vector3.zero;
    }
}
