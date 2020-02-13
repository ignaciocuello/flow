using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WadState : EntityState<Wad> {

    public override void StateOnHitReceived(Wad wad, HitBox hitBox, HurtBox hurtBox, Entity initiator)
    {
        if (initiator is Player)
        {
            wad.Collect((Player)initiator);
        }
    }

    public override void DeriveState(Wad wad)
    {
        wad.Rest();
    }

    public override EntityState HitStunState(Wad wad)
    {
        throw new UnityException("No wad hit stun state");
    }
}
