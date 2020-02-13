using System.Collections.Generic;
using UnityEngine;

public class BoxManager : Singleton<BoxManager> {

    [SerializeField]
    private bool drawHitBoxes;
    [SerializeField]
    private bool drawHurtBoxes;

    /* maps entities to the set of boxes they own */
    private Dictionary<Entity, BoxData> ownerBoxDict;

    private List<EntityExemption> exemptions;

    private ObjectPool objectPool;
    private Dictionary<Entity, BoxData> drawDict;

    /* entities waiting to be cleared from exempt on fixed update end */
    private List<Entity> clearWaitList;

    protected override void Initialize()
    {
        ownerBoxDict = new Dictionary<Entity, BoxData>();
        exemptions = new List<EntityExemption>();
        clearWaitList = new List<Entity>();

        //drawDict continually initialized in FixedUpdate
        objectPool = GetComponent<ObjectPool>();
    }

    public void PutBoxArray(Entity owner, Box[] boxes)
    {
        if (boxes.Length == 0)
        {
            return;
        }

        ownerBoxDict[owner] = new BoxData(boxes);
    }

    public void RemoveBoxes(Entity entity)
    {
        ownerBoxDict.Remove(entity);
        ClearFromExempt(entity);

        //don't need to render boxes we've removed
        drawDict.Remove(entity);
    }

    public void ClearFromExempt(Entity entity)
    {
        foreach (EntityExemption ex in new List<EntityExemption>(exemptions))
        {
            if (ex.EntityMatchesAny(entity))
            {
                exemptions.Remove(ex);
            }
        }
    }

    public void ClearFromExemptOnFixedUpdateEnd(Entity entity)
    {
        clearWaitList.Add(entity);
    }

    void FixedUpdate()
    {
        Entity[] owners = new Entity[ownerBoxDict.Keys.Count];
        ownerBoxDict.Keys.CopyTo(owners, 0);

        Entity owner1, owner2;
        for (int i = 0; i < owners.Length; i++)
        {
            owner1 = owners[i];
            //extent of aggregated boxes of owner1
            Bounds ctb1 = ownerBoxDict[owner1].totalBounds.GetRelativeBounds(owner1);
            for (int j = i+1; j < owners.Length; j++)
            {
                owner2 = owners[j];
                //extent of aggregated boxes of owner 2
                Bounds ctb2 = ownerBoxDict[owner2].totalBounds.GetRelativeBounds(owner2);
                if (ctb1.Intersects(ctb2))
                {
                    CheckForBoxIntersection(owner1, owner2);
                }
            }
        }

        foreach (Entity entity in clearWaitList)
        {
            ClearFromExempt(entity);
        }
        clearWaitList.Clear();

        //add boxes to the draw dictionary
        drawDict = new Dictionary<Entity, BoxData>(ownerBoxDict);
        //clear the hit boxes every frame
        ownerBoxDict.Clear();
    }

    void Update()
    {
        DrawAllBoxes();
    }

    public bool IsExempt(Entity owner1, Entity owner2, Box box1, Box box2)
    {
        foreach (EntityExemption ex in exemptions)
        {
            if (ex.IsExempt(owner1, owner2, box1, box2))
            {
                return true;
            }
        }

        return false;
    }

    private void CheckForBoxIntersection(Entity owner1, Entity owner2)
    {
        foreach (Box box in ownerBoxDict[owner1].boxes)
        {
            Box intersectingBox;
            if ((intersectingBox = box.CheckIntersectionWithFirst(ownerBoxDict[owner2].boxes, owner1, owner2)) != null)
            {
                //hit box should be called first
                if (box is HitBox)
                {
                    box.OnItersect(intersectingBox, owner1, owner2);
                    intersectingBox.OnItersect(box, owner2, owner1);
                }
                else
                {
                    intersectingBox.OnItersect(box, owner2, owner1);
                    box.OnItersect(intersectingBox, owner1, owner2);
                }

                exemptions.Add(new EntityExemption(owner1, owner2, box.Id));
                exemptions.Add(new EntityExemption(owner2, owner1, intersectingBox.Id));

                return;
            }
        }
    }

    private void DrawAllBoxes()
    {
        objectPool.DestroyAllObjects();

        if (drawDict != null)
        {
            foreach (Entity owner in drawDict.Keys)
            {
                if (owner != null)
                {
                    foreach (Box box in drawDict[owner].boxes)
                    {
                        if ((drawHitBoxes && (box is HitBox)) ||
                            (drawHurtBoxes && (box is HurtBox)))
                        {
                            DrawBox(box, owner);
                        }
                    }
                }
            }
        }
    }

    private void DrawBox(Box box, Entity owner)
    {
        SpriteRenderer sr = objectPool.NewObject().GetComponent<SpriteRenderer>();

        Bounds b = box.GetRelativeBounds(owner);
        sr.enabled = true;
        //keep z component as we want to render boxes above other items
        sr.transform.position = new Vector3(b.center.x, b.center.y, sr.transform.position.z);
        sr.transform.localScale = new Vector2(b.size.x, b.size.y);

        Color c = box.GetDrawColor();
        sr.color = new Color(c.r, c.g, c.b, 0.35f);
    }

    private class BoxData
    {
        internal BoundsData totalBounds;
        internal Box[] boxes;

        internal BoxData(Box[] boxes)
        {
            this.boxes = boxes;
            totalBounds = DeriveTotalBounds(boxes);
        }

        private BoundsData DeriveTotalBounds(Box[] boxes)
        {
            Bounds totalBounds = boxes[0].BoundsData.GetBounds();
            for (int i = 1; i < boxes.Length; i++)
            {
                totalBounds.Encapsulate(boxes[i].BoundsData.GetBounds());
            }

            return new BoundsData(totalBounds.center, totalBounds.size);
        }
    }
}
