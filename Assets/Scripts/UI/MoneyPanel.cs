using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyPanel : VisibleAnimatable
{
    [SerializeField]
    private MoneySubPanel wadsPanel;
    [SerializeField]
    private MoneySubPanel coinsPanel;
    [SerializeField]
    private MoneySubPanel bloodCoinsPanel;

    /* how much time the moneyPanel is visible after a money value is changed */
    [SerializeField]
    private float visibleDuration;

    /* the last time which the moneyPanel was made visible */
    private float lastTimeMadeVisible;

    public override void Awake()
    {
        base.Awake();

        VisibleAnimator.OnVisible += () => lastTimeMadeVisible = Time.unscaledTime;

        if (bloodCoinsPanel != null)
        {
            bloodCoinsPanel.gameObject.SetActive(false);
        }
    }

    public void SetNumWads(int numWads)
    {
        wadsPanel.Text = "x" + numWads;
        VisibleAnimator.Visible = true;
    }

    public void SetNumCoins(int numCoins)
    {
        coinsPanel.Text = "x" + numCoins;
        VisibleAnimator.Visible = true;
    }

    public void SetNumBloodCoins(int numBloodCoins)
    {
        bloodCoinsPanel.gameObject.SetActive(true);
        bloodCoinsPanel.Text = "x" + numBloodCoins;

        VisibleAnimator.Visible = true;
    }

    private void Update()
    {
        if (VisibleAnimator.Visible)
        {
            //if visible we know lastTimeMadeVisible is valid
            float timeDif = Time.unscaledTime - lastTimeMadeVisible;
            if (timeDif >= visibleDuration)
            {
                VisibleAnimator.Visible = false;
            }
        }
    }

    public void SetAnimateAdd(bool animate)
    {
        bloodCoinsPanel.AnimateAdd = animate;
        wadsPanel.AnimateAdd = animate;
        coinsPanel.AnimateAdd = animate;
    }
}
