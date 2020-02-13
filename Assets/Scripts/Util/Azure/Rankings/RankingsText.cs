using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingsText : MonoBehaviour {

    private Text text;

    private float ranking;
    public float Ranking
    {
        get { return ranking; }
        set
        {
            ranking = value;
            text.text = string.Format(">{0}%", (100.0f * (1.0f - (value-1.0f) / (ScoreManager.Instance.TotalRankingsCount-1.0f))).ToString("F2"));//string.Format("#{0}", value);
            if (ranking < 3.0f)
            {
                text.text = string.Format("#{0}", Mathf.Round(value));
            }
        }
    }

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    public void Shing()
    {
        if (!GetComponent<Animator>().enabled)
        {
            GetComponent<AudioSource>().Play();
            GetComponent<Animator>().enabled = true;
            GetComponent<Animator>().SetTrigger("Shing");
        }
    }
}
