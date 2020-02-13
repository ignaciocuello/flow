using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {

    public float PromptDestroyDelayTime = 1.0f;

    public IEnumerator DelayDisableMainControlPrompt(float delay, ControllerPrompt main, bool destroySelf)
    {
        yield return new WaitForSecondsRealtime(delay);

        if (main == ControllerPromptManager.Instance.MainPrompt)
        {
            ControllerPromptManager.Instance.DisableMainControllerPrompt();   
        }

        if (destroySelf)
        {
            Destroy(gameObject);
        }
    }
}
