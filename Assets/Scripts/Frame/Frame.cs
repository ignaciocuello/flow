using System;
using System.Collections.Generic;

[Serializable]
public class Frame {

    private Box[] boxes;
    public Box[] Boxes
    {
        get
        {
            if (boxes == null)
            {
                List<Box> boxList = new List<Box>();
                if (HurtBoxes != null) {
                    boxList.AddRange(HurtBoxes);
                }
                if (HitBoxes != null)
                {
                    boxList.AddRange(HitBoxes);
                }

                boxes = boxList.ToArray();
            }

            return boxes;
        }
    }

    public HurtBox[] HurtBoxes;
    public HitBox[] HitBoxes;

    /* axis aligned bounding box for entity during this frame, if size is (0,0) the default will be used */
    public BoundsData AABB;
}
