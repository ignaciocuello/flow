using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailParent : MonoBehaviour {

    private List<GameObject> trailChildren;

    public GameObject TrailChildPrefab;
    /* whether the trailChild's sprite should mirror the sprite renderer of the object's whose trail is being made */
    public bool MirrorSpriteRenderer;

    private void Awake()
    {
        trailChildren = new List<GameObject>();
    }


    public void UpdateTrailChildren(Vector3 pos, Quaternion rotation,
        int attemptCounter, int trailPeriod, int maxTrailChildren, float startingAlpha, SpriteObject spriteObject)
    {
        if (maxTrailChildren == 0)
        {
            return;
        }

        if (attemptCounter % trailPeriod == 0)
        {
            GameObject trailChild;
            if (trailChildren.Count < maxTrailChildren)
            {
                trailChild = Instantiate(TrailChildPrefab, transform);
            }
            else
            {
                trailChild = trailChildren[0];
                trailChildren.RemoveAt(0);
            }

            trailChild.transform.position = pos;
            trailChild.transform.rotation = rotation;
            
            SpriteObject childSpriteObject = trailChild.GetComponent<SpriteObject>();
            if (MirrorSpriteRenderer)
            {
                childSpriteObject.Sprite = spriteObject.Sprite;
            }
            //make sure trail child is facing the right direction
            childSpriteObject.transform.localScale = spriteObject.transform.localScale;

            trailChildren.Add(trailChild);
        }

        SetAlphaGradient(startingAlpha);
    }

    private void SetAlphaGradient(float startingAlpha)
    {
        //lower transparency as trail children get older so the newest one has alpha of startingAlpha
        //and oldest one has alpha of 1/trailChildren.Count (or 0) (in between are linearly interpolated)

        float alpha = startingAlpha;
        float step = startingAlpha / trailChildren.Count;

        for (int i = trailChildren.Count - 1; i >= 0; i--)
        {
            GameObject tr = trailChildren[i];
            foreach (SpriteRenderer sr in tr.GetComponentsInChildren<SpriteRenderer>())
            {
                Color c = sr.color;
                sr.color = new Color(c.r, c.g, c.b, alpha);
                sr.sortingOrder = i;   
            }

            alpha = Mathf.Max(0.0f, alpha - step);
        }
    }

    public void ClearAll()
    {
        foreach (GameObject trailChild in trailChildren)
        {
            Destroy(trailChild);
        }

        trailChildren.Clear();
    }
}
