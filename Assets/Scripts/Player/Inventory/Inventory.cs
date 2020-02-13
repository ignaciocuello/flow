using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Inventory : MonoBehaviour {

    [SerializeField]
    private bool updateUI;

    [SerializeField]
    private int numWads;
    public int NumWads
    {
        get { return numWads; }
        set
        {
            numWads = value;
            if (updateUI)
            {
                UserInterface.Instance.Get(UIElemType.MONEY_PANEL)?.GetComponent<MoneyPanel>()?.SetNumWads(numWads);
            }
        }
    }

    [SerializeField]
    private int numCoins;
    public int NumCoins
    {
        get { return numCoins; }
        set
        {
            numCoins = value;
            if (updateUI)
            {
                UserInterface.Instance.Get(UIElemType.MONEY_PANEL)?.GetComponent<MoneyPanel>()?.SetNumCoins(numCoins);
            }
        }
    }

    [SerializeField]
    private int numBloodCoins;
    public int NumBloodCoins
    {
        get { return numBloodCoins; }
        set
        {
            numBloodCoins = value;
            if (updateUI)
            {
                UserInterface.Instance.Get(UIElemType.MONEY_PANEL)?.GetComponent<MoneyPanel>()?.SetNumBloodCoins(numBloodCoins);
            }
        }
    }

    private void Start()
    {
        if (UserInterface.Instance.Get(UIElemType.MONEY_PANEL) == null)
        {
            UserInterface.Instance.Create(UIElemType.MONEY_PANEL);
        }
    }

    public void Copy(Inventory inventory)
    {
        NumBloodCoins = inventory.NumBloodCoins;
        NumWads = inventory.NumWads;
        NumCoins = inventory.NumCoins;
    }

    public Score ToScore()
    {
        return new Score(NumBloodCoins, NumWads, NumCoins);
    }
}
