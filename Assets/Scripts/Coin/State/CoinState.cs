using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CoinState : EntityState<Coin> {

    public override void DeriveState(Coin coin)
    {
        coin.Rest();
    }

    public override EntityState HitStunState(Coin entity)
    {
        throw new UnityException("No hitstun");
    }

}
