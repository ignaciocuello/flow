using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level8Tutorial : Tutorial {

    [SerializeField]
    private FocusGenerator focusGenerator;
    // true if main control prompt is about to be disabled
    private bool disabling;

    private void Awake()
    {
        focusGenerator.OnFocusEnteredStateEventProxy.AddListener(OnFocusEnteredState);
    }

	public void OnPlayerEntered(Player player)
    {
        if (!disabling)
        {
            ControllerPromptManager.Instance.AssignActionToMainControllerPrompt(
                PlayerCompositeActions.JUMP_GAIN.DisplayName(), "(hold during hit)\nPogo");
        }
    }

    public void OnFocusEnteredState(EntityState state)
    {
        if (!disabling && state.Descriptors.Contains(StateDescriptor.HITSTUN))
        {
            disabling = true;
            StartCoroutine(DelayDisableMainControlPrompt(PromptDestroyDelayTime, ControllerPromptManager.Instance.MainPrompt, destroySelf: true));
        }
    }

    public void OnPlayerExited(Player player)
    {
        if (!disabling)
        {
            ControllerPromptManager.Instance.DisableMainControllerPrompt();
            Destroy(gameObject);
        }
    }
}
