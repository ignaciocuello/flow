using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTrail : MonoBehaviour {

    [SerializeField]
    private GameObject trailChildPrefab;

    [SerializeField]
    private int trailPeriod;
    /* maximum number of trail children rendered */
    [SerializeField]
    private int maxTrailChildren;
   
    /* how many times we have attempted to create a trail child, attempt once per frame */
    private int attemptCounter;

    /* if this is true the starting alpha of the trail will be dependant on object speed */
    [SerializeField]
    private bool speedDependantAlpha;
    /* speed at which the starting alpha is 1 */
    [SerializeField]
    private float fullOpaqueSpeed;
    /* speed at which the starting alpha is 0 */
    [SerializeField]
    private float fullTransparentSpeed;
    [SerializeField]
    private float alphaLerpSpeed;

    /* object containing all the trails */
    private TrailParent trailParent;

    //TODO: remove this later (replace with better option)
    private float startingAlpha;
    [SerializeField, Range(0,1)]
    private float maxStartingAlpha;
    [SerializeField]
    private bool mirrorSpriteRenderer;

    private SpriteObject spriteObject;
    private Rigidbody2D rb;

    private void Awake()
    {
        //get sprite object from body
        spriteObject = GetComponentInChildren<SpriteObject>();
        rb = GetComponent<Rigidbody2D>();

        trailParent = 
            TrailFactory.Instance.CreateTrailParent(
                gameObject.name, mirrorSpriteRenderer, trailChildPrefab).GetComponent<TrailParent>();
    }

    public void FixedUpdate()
    {
        trailParent.UpdateTrailChildren(transform.position, transform.rotation,
            attemptCounter, trailPeriod, maxTrailChildren, GetStartingAlpha(), spriteObject);
        attemptCounter++;
    }

    private float GetStartingAlpha()
    {
        if (speedDependantAlpha)
        {
            float targetStartingAlpha = Mathf.Lerp(0.0f, maxStartingAlpha,
                    (rb.velocity.magnitude - fullTransparentSpeed) / (fullOpaqueSpeed - fullTransparentSpeed));
            startingAlpha = Mathf.Lerp(startingAlpha, targetStartingAlpha, alphaLerpSpeed * Time.fixedDeltaTime);
        }
        else
        {
            startingAlpha = maxStartingAlpha;
        }

        return startingAlpha;
    }

    public void DestroyTrailChildren()
    {
        if (trailParent != null)
        {
            Destroy(trailParent.gameObject);
        }
    }

    public void ClearMaxTrailChildren()
    {
        maxTrailChildren = 0;
        trailParent.ClearAll();
    }

    public void OnDestroy()
    {
        DestroyTrailChildren();
    }
}
