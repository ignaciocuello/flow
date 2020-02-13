using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncerState : EntityState<Bouncer> {

    public override void DeriveState(Bouncer bouncer)
    {
        bouncer.Rest();
    }

    public override EntityState HitStunState(Bouncer entity)
    {
        throw new System.NotImplementedException();
    }
}
