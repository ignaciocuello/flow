using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibleAnimatable : MonoBehaviour {

    [SerializeField]
    private Material targetMaterial;
    [SerializeField]
    private float visibleSpeed = 1.0f;
    [SerializeField]
    private float invisibleSpeed = 1.0f;

    [SerializeField]
    private GameObject visibleAnimatorPrefab;

    public VisibleAnimator VisibleAnimator { get; private set; }

    public virtual void Awake()
    {
        VisibleAnimator = Instantiate(visibleAnimatorPrefab, transform).GetComponent<VisibleAnimator>();

        VisibleAnimator.TargetMaterial = targetMaterial;
        VisibleAnimator.VisibleSpeed = visibleSpeed;
        VisibleAnimator.InvisibleSpeed = invisibleSpeed;
    }
}
