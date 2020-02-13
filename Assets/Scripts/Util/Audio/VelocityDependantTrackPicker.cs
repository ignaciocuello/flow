using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityDependantTrackPicker : MonoBehaviour {

    [SerializeField]
    private TargetIndex targetIndex;

    [SerializeField]
    private float horizontalSpeedThreshold;

    private Rigidbody2D rigidbody2d;

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

	private void Update()
    {
        CrossFade crossFade = GameManager.Instance.CrossFade;
        if (Mathf.Abs(rigidbody2d.velocity.x) > horizontalSpeedThreshold)
        {
            crossFade.AddTargetIndex(targetIndex);
        }
        else
        {
            crossFade.RemoveTargetIndex(targetIndex.Priority);
        }
    }
}
