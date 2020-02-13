using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionCompleteTrigger : MonoBehaviour {

    [SerializeField]
    private GameObject endSequenceManagerPrefab;

	public void Trigger()
    {
        GameManager.Instance.InstantiateManager(endSequenceManagerPrefab);
    }
}
