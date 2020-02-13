using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : Entity {

    [SerializeField]
    private RestingBouncerState restingState;

    public override void Awake()
    {
        base.Awake();

        Rest();
    }

    public override void OnTriggerExit2D(Collider2D collider)
    {
        Entity entity = collider.GetComponent<Entity>();
        if (entity != null)
        {
            BoxManager.Instance.ClearFromExemptOnFixedUpdateEnd(entity);
        }
    }

    public override void InitStates()
    {
        base.InitStates();

        restingState = Instantiate(restingState);
    }

    public void Rest()
    {
        State = restingState;
    }
}
