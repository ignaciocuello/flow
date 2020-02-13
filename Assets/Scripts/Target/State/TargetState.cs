using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetState : EntityState<Target>
{
    [SerializeField, Tooltip("if focus is less than this distance to target then force will decrease linearly with distance")]
    private float epsDistance = 0.25f;

    public override void DeriveState(Target target)
    {
        target.Lock();
    }

    public override EntityState HitStunState(Target target)
    {
        throw new UnityException("No hitstun");
    }

    public override void StateFixedUpdate(Target target)
    {
        if (target.Focus != null && (target.Focus.State.Descriptors.Contains(StateDescriptor.STATIC)))
        {
            Vector2 distanceVector = target.transform.position - target.Focus.transform.position;
            float forceMag = target.AttractionForceMagnitude;

            if (distanceVector.magnitude < epsDistance)
            {
                forceMag = target.AttractionForceMagnitude * (distanceVector.magnitude / epsDistance);
            }

            target.Focus.AddForce(forceMag * distanceVector.normalized);
        }
    }

    public override void StateOnTriggerEnter2D(Target target, Collider2D collider)
    {
        if (collider.CompareTag("Focus"))
        {
            Focus entered = collider.GetComponent<Focus>();
            //prevents holding the same focus when it oscillates in and out
            //of the target
            if (entered != target.Focus && !(entered.State is DeadFocusState))
            {
                HoldFocus(target, collider.GetComponent<Focus>());
            }
        }
    }

    public override void StateOnTriggerExit2D(Target target, Collider2D collider)
    {
        if (collider.CompareTag("Focus"))
        {
            Focus exited = collider.GetComponent<Focus>();
            //only drop focus if the focus being held is the one dropped.
            //this is to prevent the scenario where there is another focus
            //in the target and it exits the target, it would otherwise
            //cause the target to drop its focus, drop only if focus resting
            //this is done so the sheer momentum of the ball doesn't make it be
            //dropped when it falls out of the target (as it is in static state)
            //however if  the player hits the focus (it channges to resting)
            if (exited == target.Focus && !exited.State.Descriptors.Contains(StateDescriptor.STATIC))
            {
                DropFocus(target);
            }
        }
    }

    public virtual void HoldFocus(Target target, Focus entered)
    {
    }

    public void DefaultHoldFocus(Target target, Focus entered, bool closed)
    {
        //if the new focus already belongs to a target, make that target drop
        //the focus, as we only want a one to one relationship between
        //targets and focuses
        if (entered.Target != null)
        {
            if (entered.Target.TargetType == TargetType.CLOSED)
            {
                return;
            }

            entered.Target.DropFocus();
        }

        bool unlock = false;
        if (target.Focus != null)
        {
            target.LetGoOf(target.Focus);
        }
        else
        {
            unlock = true;
            target.TargetEntered.Invoke(target);
        }

        target.Focus = entered;
        target.Focus.Target = target;

        //we only want the focus to rest on hit, if it should drop from the target on exit
        target.Focus.Static(deriveRest: !closed);

        target.LightAnimator.SetBool("HoldsFocus", true);

        if (target.SlowDownFactor != 0.0f)
        {
            target.SlowDown();
        }

        if (unlock)
        {
            target.Unlock();
        }
    }

    public virtual void DropFocus(Target target)
    {
    }

    public void DefaultDropFocus(Target target)
    {
        if (target.Focus != null)
        {
            target.TargetExited.Invoke(target);

            target.LightAnimator.SetBool("HoldsFocus", false);

            target.LetGoOf(target.Focus);
            target.Focus = null;

            target.Lock();
        }
    }
}
