using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerPromptPanel : MonoBehaviour {

    private void OnDestroy()
    {
        //unnsub from player state events since there will be no pannel to put controller prompts in
        ControllerPromptManager.Instance.UnsubscribeFromPlayer();
    }
}
