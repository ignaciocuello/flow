using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public abstract class Box {

    /* the ID of this box, can be descriptive or an arbitrary number, used to identify boxes
     * which represent the same box semantically between frames. */
    [SerializeField]
    public String Id;

    /* the AABB data describing the area of effect of the box */
    public BoundsData BoundsData;

    public Box(BoundsData boundsData, string id)
    {
        BoundsData = boundsData;
        Id = id;
    }

    public Box()
    {
    }

    /* called when this box intersects with 'box', owner1 is the owner of this box, owner2
     * is the owner of 'box' */
    public abstract void OnItersect(Box box, Entity owner1, Entity owner2);

    /* checks if this box intersects with any of the given boxes, returns
     * the first one to intersect, null otherwise */
    public Box CheckIntersectionWithFirst(Box[] boxes, Entity owner1, Entity owner2)
    {
        Bounds cb1 = GetRelativeBounds(owner1);

        foreach (Box box in boxes)
        {
            Bounds cb2 = box.GetRelativeBounds(owner2);
            if (cb1.Intersects(cb2) && ValidBoxInteraction(box) && box.ValidBoxInteraction(this) 
                && !BoxManager.Instance.IsExempt(owner1, owner2, this, box))
            {
                return box;
            }
        }

        return null;
    }

    /* Transforms the box bounds from relative coordinates (with respect to the owner) to
     * absolute world coordinates */
    public Bounds GetRelativeBounds(Entity owner)
    {
        return BoundsData.GetRelativeBounds(owner);
    }

    public abstract Color GetDrawColor();

    /* returns true if box is in the set of box types that this box is allowed to interact with */
    public abstract bool ValidBoxInteraction(Box box);

}
