using UnityEngine;

public class Level2Tutorial : Tutorial {

    /* the time the jump prompt reamins on screen after the player has jumped */
    [SerializeField]
    private float jumpPromptDestroyDelayTime;

    public void OnPlayerEnterAreaEvent(Player player)
    {
        ControllerPromptManager.Instance.AssignActionToMainControllerPrompt(PlayerCompositeActions.JUMP.DisplayName());
    }

    public void OnPlayerExitAreaEvent(Player player)
    {
        StartCoroutine(DelayDisableMainControlPrompt(jumpPromptDestroyDelayTime, ControllerPromptManager.Instance.MainPrompt, destroySelf:false));
    }
}
