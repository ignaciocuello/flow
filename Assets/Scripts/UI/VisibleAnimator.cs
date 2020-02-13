using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibleAnimator : MonoBehaviour {

    private Animator animator;

    [Range(0, 1)]
    public float Alpha;

    private bool visible;
    public bool Visible
    {
        get { return visible; }
        set {
            bool changed = visible != value;

            visible = value;
            animator.SetBool("Visible", value);

            if (changed)
            {
                if (visible)
                {
                    InvokeVisible();
                }
                else
                {
                    InvokeInvisible();
                }
            }            
        }
    }

    public float VisibleSpeed
    {
        get { return animator.GetFloat("VisibleSpeed"); }
        set { animator.SetFloat("VisibleSpeed", value); }
    }

    public float InvisibleSpeed
    {
        get { return animator.GetFloat("InvisibleSpeed"); }
        set { animator.SetFloat("InvisibleSpeed", value); }
    }


    //fired when 'Visible' is set to true
    public event Action OnVisible;
    //fired when UI components have an alpha of 1
    public event Action OnFullyVisible;

    //fired when 'Visible' is set to false
    public event Action OnInvisible;
    //fired when UI components have an alpha of 0
    public event Action OnFullyInvisible;

    private Material targetMaterial;
    public Material TargetMaterial
    {
        get { return targetMaterial; }
        set
        {
            targetMaterial = value;
            ChangeMaterialAlpha(Alpha);
        }
    }

    public virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public virtual void Update()
    {
        ChangeMaterialAlpha(Alpha);
    }

    private void ChangeMaterialAlpha(float alpha)
    {
        TargetMaterial.color = TargetMaterial.color.ChangeAlpha(alpha);
    }

    public void InvokeVisible()
    {
        OnVisible?.Invoke();
    }

    public void InvokeFullyVisible()
    {
        OnFullyVisible?.Invoke();
    }

    public void InvokeInvisible()
    {
        OnInvisible?.Invoke();
    }

    public void InvokeFullyInvisible()
    {
        OnFullyInvisible?.Invoke();
    }
}
