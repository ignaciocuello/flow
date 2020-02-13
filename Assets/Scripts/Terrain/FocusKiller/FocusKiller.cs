using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusKiller : MonoBehaviour {
    [SerializeField]
    private bool usesOffset;
    [Tooltip("an offset of 0 means the focus killer is at its original position, 1 puts it at endPos and anything in between linearly interpolates")]
    public float Offset;
    [Tooltip("position when Offset is 1 = startPos + offsetValue")]
    public Vector3 offsetValue;
    private Vector3 startPos;

    private void Awake()
    {
        startPos = transform.position;
        GetComponent<Animator>().SetBool("UsesOffset", usesOffset);
    }

    private void FixedUpdate()
    {
        if (usesOffset)
        {
            transform.position = Vector3.Lerp(startPos, startPos + offsetValue, Offset);
        }
    }
}
