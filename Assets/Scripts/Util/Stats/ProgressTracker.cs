using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressTracker : MonoBehaviour {

    private float totalLength;
    private float currentLength;

    private ProgressChunk[] progressChunks;

    private Player player;

    //bool is whether player is facing right or not
    public event Action<bool> OnPlayerDirectionChangeEvent;
    //bool same as above, float is percentage of progress range [0,1]
    public event Action<float, bool> OnPlayerStayEvent;

	private void Start () {
        progressChunks = new ProgressChunk[transform.childCount];

        totalLength = 0.0f;
        for (int i = 0; i < transform.childCount; i++)
        {
            ProgressChunk p = progressChunks[i] = transform.GetChild(i).GetComponent<ProgressChunk>();
            p.OnPlayerStay.AddListener(OnPlayerStay);
            totalLength += p.Length;
        }

        if (UserInterface.Instance.Get(UIElemType.PROGRESS_BAR) == null)
        {
            ProgressBar bar = UserInterface.Instance.Create(UIElemType.PROGRESS_BAR).GetComponent<ProgressBar>();

            OnPlayerDirectionChangeEvent += bar.OnDirectionChangeCallback;
            OnPlayerStayEvent += bar.OnStayCallback;

            bar.OnDestroyEvent += () =>
            {
                OnPlayerDirectionChangeEvent -= bar.OnDirectionChangeCallback;
                OnPlayerStayEvent -= bar.OnStayCallback;
            };
        }
    }

    public void Update()
    {
        if (player != null && OnPlayerDirectionChangeEvent != null)
        {
            OnPlayerDirectionChangeEvent(player.FacingRight);
        }
    }

    public void OnPlayerStay(ProgressChunk progressChunk, Player player, float localProgress)
    {
        this.player = player;

        currentLength = 0.0f;

        bool facingRight = true;
        foreach (ProgressChunk p in progressChunks)
        {
            if (p == progressChunk)
            {
                currentLength += localProgress;
                if (p.Direction == -1)
                {
                    facingRight = false;
                }
                break;
            }

            currentLength += p.Length;
        }

        OnPlayerStayEvent?.Invoke(currentLength / totalLength, facingRight);
    }
}
