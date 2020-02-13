using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodCoin : Coin {

    public override void AddToPlayerInventory(Inventory inventory)
    {
        inventory.NumBloodCoins++;
    }
}
