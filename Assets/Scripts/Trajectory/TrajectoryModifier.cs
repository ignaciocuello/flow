using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryModifier {

    /* how long the trajectory will be modified for */
    private int duration;

    /* initial velocity to apply to the entity */
    private Vector2 impact;
    public Vector2 Impact
    {
        get { return impact; }
    }

    /* initial drag to add to the entity's current drag */
    public float DragAddition;
    /* determines how fast drag is decreased for this entity */
    public float MetaDrag;

    /* the entity's original drag */
    private float originalDrag;

    /* store copy of drag that is consistent with internal trajectory
     * modification, as entity drag may be modified externally, which
     * breaks the process */
    private float storedDrag;
    public float StoredDrag
    {
        get { return storedDrag; }
    }

    /* current frame scaled for time */
    private float scaledCurrentFrame;

    public TrajectoryModifier(Entity entity, TrajectoryModData tmod)
    {
        impact = tmod.Impact;
        DragAddition = tmod.DragAddition;
        MetaDrag = tmod.MetaDrag;
        duration = tmod.Duration;

        //need entity's base drag
        entity.ResetDrag();

        /* set the entity's initial velocity to the impact */
        entity.Velocity = tmod.Additive ? entity.Velocity + impact : impact;
        entity.AngularVelocity += tmod.AngularImpact;

        originalDrag = entity.Drag;

        entity.Drag = tmod.DragAddition;
        storedDrag = tmod.DragAddition;
    }

    public void UpdateTrajectory(Entity entity)
    {
        storedDrag = Mathf.Clamp(storedDrag - MetaDrag * Time.fixedDeltaTime, originalDrag, float.PositiveInfinity);
        entity.Drag = storedDrag;

        scaledCurrentFrame += Time.timeScale;
    }

    public bool IsDone()
    {
        return scaledCurrentFrame >= duration;
    }

    public void End(Entity entity)
    {
        entity.ResetDrag();
    }
}
