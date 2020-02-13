using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class FocusEnteredEvent : UnityEvent<Focus> {
}

public class FocusKillerBody : MonoBehaviour {

    [SerializeField]
    private bool boost = true;
    [SerializeField]
    private bool doubleSided;

    [SerializeField]
    private GameObject focusKillerArrowPrefab;

    [SerializeField]
    private float boostHorizontalSpeed;

    [SerializeField]
    private FocusEnteredEvent focusEntered;
    [SerializeField]
    private float playerDragForce;

    /* threshold under which lerp boost speed reduction is applied */
    [SerializeField]
    private float deltaThreshold;

    private List<Animator> focusKillerArrows;

    private void Awake()
    {
        focusKillerArrows = new List<Animator>();
        CreateFocusKillerArrows();

        if (doubleSided)
        {
            FocusKillerSide side = transform.parent.GetComponentInChildren<FocusKillerSide>();

            FocusKillerSide newSide = Instantiate(side.gameObject, transform.parent).GetComponent<FocusKillerSide>();
            newSide.LocalXPos = -side.LocalXPos;
            newSide.transform.localScale = Vector3.Scale(new Vector3(-1.0f, 1.0f, 1.0f), side.transform.localScale);
        }
    }

    private void CreateFocusKillerArrows()
    {
        int numToCreate = (int)(transform.localScale.x / focusKillerArrowPrefab.transform.localScale.x);

        GameObject arrowObj;
        float shiftLeft = ((numToCreate - 1) / 2.0f) * focusKillerArrowPrefab.transform.localScale.x;
        for (int i = 0; i < numToCreate; i++)
        {
            arrowObj = Instantiate(focusKillerArrowPrefab, transform.parent);
            arrowObj.transform.localPosition = Vector2.zero;
            arrowObj.transform.position += new Vector3(-shiftLeft + focusKillerArrowPrefab.transform.localScale.x * i, 0.0f, 0.0f);

            focusKillerArrows.Add(arrowObj.GetComponent<Animator>());
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        OnEnter2D(collision.gameObject);
    }

    //wan't focus killer to work as trigger or dynamic
    public void OnTriggerEnter2D(Collider2D collider)
    {
        OnEnter2D(collider.gameObject);
    }

    private void OnEnter2D(GameObject gameObj)
    {
        if (gameObj.CompareTag("Focus"))
        {
            FocusEntered(gameObj.GetComponent<Focus>());
        }
        else if (gameObj.CompareTag("Player") || gameObj.CompareTag("Boss"))
        {
            Boost(gameObj.GetComponent<Player>());
        }
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        OnStay2D(collision.gameObject);
    }

    public void OnTriggerStay2D(Collider2D collider)
    {
        OnStay2D(collider.gameObject);
    }

    private void OnStay2D(GameObject gameObj)
    {
        if (gameObj.CompareTag("Player") || gameObj.CompareTag("Boss"))
        {
            Boost(gameObj.GetComponent<Player>());
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        OnExit2D(collision.gameObject);
    }

    //wan't focus killer to work as trigger or dynamic
    public void OnTriggerExit2D(Collider2D collider)
    {
        OnExit2D(collider.gameObject);
    }

    private void OnExit2D(GameObject gameObj)
    {
        if (gameObj.CompareTag("Player") || gameObj.CompareTag("Boss"))
        {
            Unboost(gameObj.GetComponent<Player>());
        }
    }

    public void Boost(Player player)
    {
        if (!boost)
        {
            return;
        }

        if (LayerMask.LayerToName(gameObject.layer).Equals("Terrain"))
        {
            float direction = GetDirection();

            float horiz = InputBuffer.Instance.GetAxis(player.InputGenerator, "Horizontal");
            if (horiz != 0.0f && (HorizInDirection(horiz, direction) || doubleSided))
            {
                FlashArrows(true, horiz);
                player.SetHorizontalSpeed(GetBoostHorizontalSpeed(player));
            }
            else
            {
                FlashArrows(false);
                player.ResetHorizontalSpeed();
            }
        }
    }

    public float GetBoostHorizontalSpeed(Player player)
    {
        //TODO: make this work with double sided
        float direction = GetDirection();
        float dif = 0.0f;

        float leftX = transform.position.x - transform.localScale.x / 2.0f;
        float rightX = transform.position.x + transform.localScale.x / 2.0f;

        bool lessThanThresh = false;

        if (!doubleSided)
        {
            dif = (direction == -1) ? player.transform.position.x - leftX : rightX - player.transform.position.x;
            lessThanThresh = dif <= deltaThreshold;
        }

        return lessThanThresh ? player.RunningState.OriginalHorizontalSpeed : boostHorizontalSpeed;
    }

    public void Unboost(Player player)
    {
        if (LayerMask.LayerToName(gameObject.layer).Equals("Terrain"))
        {
            //HAVE A RESET PROPERTY IN RUN ATTRIBUTE?? (this causes bugs)
            player.ResetHorizontalSpeed();
            FlashArrows(false);
        }
    }

    public void FlashArrows(bool flash, float horiz)
    {
        //flip arrows on x-axis if running in non-cannonical direction (for double sided) else reset the arrow positions
        float scaleX = !HorizInDirection(horiz, GetDirection()) ? -1.0f : 1.0f;
        foreach (Animator anim in focusKillerArrows)
        {
            anim.transform.localScale = new Vector3(scaleX, 1.0f, 1.0f);
        }

        FlashArrows(flash);
    }

    public void FlashArrows(bool flash)
    {
        foreach (Animator anim in focusKillerArrows)
        {
            anim.SetBool("Flashing", flash);
        }
    }

    private float GetDirection()
    {
        return (transform.lossyScale.x < 0) ? 1.0f : -1.0f;
    }

    private bool HorizInDirection(float horiz, float direction)
    {
        return ((Mathf.Abs(horiz) / horiz) == direction);
    }

    private void FocusEntered(Focus focus)
    {
        focusEntered.Invoke(focus);

        //kill focus on collision
        focus.Kill();
    }
}
