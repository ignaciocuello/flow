using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FocusKillerSide : MonoBehaviour {

    [SerializeField]
    private GameObject body;

    [Range(-0.5f, 0.5f)]
    public float LocalXPos;

    public void Update()
    {
        transform.localPosition = new Vector3(Mathf.Abs(body.transform.localScale.x) * LocalXPos, 0.0f, 0.0f);

        Vector3 pos = transform.position;
        float shiftFactor = (transform.parent.lossyScale.x < 0.0f ? -1.0f : 1.0f) * (LocalXPos < 0.0f ? -1.0f : 1.0f);
        transform.position -= Vector3.right * shiftFactor * Mathf.Abs(transform.localScale.x) / 2.0f;
    }
}
