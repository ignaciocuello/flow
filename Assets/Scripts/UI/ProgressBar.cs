using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {

    private Slider slider;
    public Slider Slider
    {
        get { return slider; }
    }

    private Image handle;

    public event Action OnDestroyEvent;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.value = 0.0f;

        handle = slider.handleRect.GetComponent<Image>();
    }

    public void OnDirectionChangeCallback(bool facingRight)
    {
        if (facingRight)
        {
            IconFaceRight();
        }
        else
        {
            IconFaceLeft();
        }
    }

    public void OnStayCallback(float value, bool facingRight)
    {
        OnDirectionChangeCallback(facingRight);

        Slider.value = value;
    }

    public void FaceRight()
    {
        slider.SetDirection(Slider.Direction.LeftToRight, true);
    }

    public void FaceLeft()
    {
        slider.SetDirection(Slider.Direction.RightToLeft, true);
    }

    public void IconFaceRight()
    {
        handle.rectTransform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
    }

    public void IconFaceLeft()
    {
        handle.rectTransform.localScale = Vector3.one;
    }

    public void OnDestroy()
    {
        if (OnDestroyEvent != null)
        {
            OnDestroyEvent();
        }
    }
}
