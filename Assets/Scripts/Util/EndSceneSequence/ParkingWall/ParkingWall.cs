using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingWall : MonoBehaviour {

    public event Action<Van> OnVanCollision;

	private void Awake()
    {
        if (EndSceneSequenceManager.Instance != null)
        {
            EndSceneSequenceManager.Instance.ParkingWall = gameObject;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnVanCollision?.Invoke(collision.gameObject.GetComponentInParent<Van>());
    }
}
