using System.Collections.Generic;
using UnityEngine;

public class LevelExtents : MonoBehaviour {

    //composite actions leart in previous knowledge that will be displayed
    [SerializeField]
    private List<PlayerCompositeActions> learnedActions;

    //true if the player has previously entered this area
    private bool hasBeenEntered;

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            //GameManager.Instance.PlayerEnteredLevel(gameObject.scene.buildIndex);
            if (!hasBeenEntered)
            {
                ControllerPromptManager.Instance.KnownActions.UnionWith(learnedActions);
            }

            hasBeenEntered = true;
        }
    }
}
