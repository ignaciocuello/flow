using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBeat : MonoBehaviour {

    [SerializeField]
    private EntityType[] entityTypes;
    [SerializeField]
    private TargetIndex targetIndex;
    [SerializeField]
    private float enterLerpRate;
    [SerializeField]
    private float exitLerpRate;

    /* maps entity types to how many are in the level beat area */
    private Dictionary<EntityType, int> numEntityTypeInArea;
    private bool beatIsActive;

    private BoxCollider2D collider2d;

    private void Awake()
    {
        collider2d = GetComponent<BoxCollider2D>();

        numEntityTypeInArea = new Dictionary<EntityType, int>();

        foreach (EntityType type in entityTypes)
        {
            numEntityTypeInArea.Add(type, 0);
        }
    }

	private void OnTriggerEnter2D(Collider2D collider)
    {
        CheckArea(collider, enter: true);
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        CheckArea(collider, enter: false);
    }

    private void CheckArea(Collider2D collider, bool enter)
    {
        bool changed = false;

        foreach (EntityType type in entityTypes)
        {
            if (collider.CompareTag(type.DisplayName()))
            {
                if (enter)
                {
                    numEntityTypeInArea[type]++;
                }
                else
                {
                    numEntityTypeInArea[type]--;
                }

                changed = true;

                break;
            }
        }

        if (changed)
        {
            CheckIfBeatIsActive();
            SetBeats();
        }
    }

    private void CheckIfBeatIsActive()
    {
        foreach (EntityType type in entityTypes)
        {
            if (numEntityTypeInArea[type] == 0)
            {
                beatIsActive = false;
                return;
            }
        }

        beatIsActive = true;  
    }

    private void SetBeats()
    {
        if (beatIsActive)
        {
            GameManager.Instance.CrossFade.AddTargetIndex(targetIndex);
            GameManager.Instance.CrossFade.LerpRate = enterLerpRate;
        }
        else
        {
            GameManager.Instance.CrossFade.RemoveTargetIndex(targetIndex.Priority);
            GameManager.Instance.CrossFade.LerpRate = exitLerpRate;
        }
    }

    public void DisableCollider()
    {
        collider2d.enabled = false;
    }
}
