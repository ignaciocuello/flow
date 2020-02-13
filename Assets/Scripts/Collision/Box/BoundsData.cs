using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BoundsData {

    public Vector2 Center;
    public Vector2 Size;

    public BoundsData(Vector2 center, Vector2 size)
    {
        Center = center;
        Size = size;
    }

    public BoundsData()
    {
    }

    /* returns the bounds corresponding to this 'BoundsData' relative to 'owner' entity */
    public Bounds GetRelativeBounds(Entity owner)
    {
        Vector2 directedCenter =
            new Vector2((!owner.FacingRight ? 1.0f : -1.0f) * Center.x, Center.y);

        //cast to Vector2 as we only want to take x, y position into account
        return new Bounds(
            (Vector2)owner.transform.TransformPoint(directedCenter),
            Vector2.Scale(Size, owner.transform.localScale));
    }

    public Bounds GetBounds()
    {
        return new Bounds(Center, Size);
    }

}
