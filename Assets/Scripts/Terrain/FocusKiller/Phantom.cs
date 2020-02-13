using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phantom : MonoBehaviour {

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void ClearPhantom()
    {
        animator.SetTrigger("Clear");
    }

    public void DestroyPhantom()
    {
        Destroy(gameObject);
    }
}
