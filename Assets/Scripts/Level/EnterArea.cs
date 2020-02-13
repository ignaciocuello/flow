using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterArea : MonoBehaviour {

    private Level level;

    private BoxCollider2D boxCollider;

    private void Awake()
    {
        level = GetComponentInParent<Level>();

        boxCollider = GetComponent<BoxCollider2D>();
    }

	public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            level.OnEnterArea();
        }
    }

    public void OnLevelFinish()
    {
        boxCollider.enabled = false;
    }
}
