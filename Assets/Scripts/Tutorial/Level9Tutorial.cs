using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level9Tutorial : Tutorial {

    [SerializeField]
    private float targetIndex;
    [SerializeField]
    private float lerpRate;

    public void OnPlayerEnteredDownAttackArea(Player player)
    {
        ControllerPromptManager.Instance.AssignActionToMainControllerPrompt(
            PlayerCompositeActions.DOWN_ATTACK.DisplayName());
    }

    public void OnPlayerEnteredMusicResetAreaOnce(Player player)
    {
        GameManager.Instance.CrossFade.ClearTargets();
        GameManager.Instance.CrossFade.LerpRate = lerpRate;
        GameManager.Instance.CrossFade.AddTargetIndex(new TargetIndex(priority: 0.0f, target: targetIndex));
    }

    //should be destroyed when we down attack into desired target
    public void OnTargetEntered(Target target)
    {
        ControllerPromptManager.Instance.KnownActions.UnionWith(new PlayerCompositeActions[] { PlayerCompositeActions.DOWN_ATTACK });
        StartCoroutine(DelayDisableMainControlPrompt(PromptDestroyDelayTime, ControllerPromptManager.Instance.MainPrompt, destroySelf: true));
    } 
}
