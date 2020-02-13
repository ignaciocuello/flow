using System.Collections;
using UnityEngine;

public class NextLevelTrigger : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            //GameManager.Instance.LoadNextLevel();
        }
    }
}
