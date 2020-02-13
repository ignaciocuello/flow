using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitArea : MonoBehaviour {

    /* value to change offset of collider when player finishes the level */
    [SerializeField]
    private Vector2 finishedOffset;

    private Level level;

    private BoxCollider2D boxCollider;

    private void Awake()
    {
        level = GetComponentInParent<Level>();

        boxCollider = GetComponent<BoxCollider2D>();
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            level.OnExitArea();
        }
    }

    public void OnLevelFinish()
    {
        //player will exit level  area
        boxCollider.offset = Vector2.up * 10000;
    }
}
