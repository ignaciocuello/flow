using System.Collections;
using UnityEngine;

public class FocusGeneratorTrigger : MonoBehaviour {

    [SerializeField]
    private FocusGenerator focusGenerator;

    /* whether this trigger has been used up */
    private bool triggered;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!triggered && collider.CompareTag("Player"))
        {
            focusGenerator.GenerateFocus();
            triggered = true;
        }
    }
}
