using System.Collections.Generic;
using UnityEngine;

public class RestingCoinState : CoinState {

    public override void EnterState(Coin coin)
    {
        base.EnterState(coin);
        coin.DebugText.text = "RESTING";
    }
}
