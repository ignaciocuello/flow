using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class PlayerEnterEvent : UnityEvent<ProgressChunk, Player, float>
{
}

public class ProgressChunk : MonoBehaviour {

    public PlayerEnterEvent OnPlayerStay;

    [Range(-1, 1)]
    public int Direction;

    private float length;
    public float Length
    {
        get { return length; }
    }

    private float x0;
    private float x1;

    private void Awake()
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();

        x0 = transform.position.x + boxCollider.offset.x - boxCollider.size.x/2.0f;
        x1 = transform.position.x + boxCollider.offset.x + boxCollider.size.x/2.0f;

        length = x1 - x0;
        if (Direction == -1)
        {
            float temp = x0;
            x0 = x1;
            x1 = temp;
        }

        transform.SetParent(StatsTracker.Instance.ProgressTracker.transform);
    }

    public void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            float localProgress = 
                Direction == 0 ? 0.0f : Mathf.Abs(collider.transform.position.x - x0);
            OnPlayerStay.Invoke(this, collider.GetComponent<Player>(), localProgress);
        }
    }
}
