using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneySubPanel : MonoBehaviour {

    private Text textComp;
    private Animator animator;

    [SerializeField]
    private bool animateAdd;
    public bool AnimateAdd
    {
        get { return animateAdd; }
        set { animateAdd = value; }
    }

    public string Text
    {
        get { return textComp.text; }
        set
        {
            textComp.text = value;
            if (animateAdd)
            {
                animator.SetTrigger("Add");
            }
        }
    }

    private void Awake()
    {
        textComp = GetComponentInChildren<Text>();
        animator = GetComponent<Animator>();
    }
}
