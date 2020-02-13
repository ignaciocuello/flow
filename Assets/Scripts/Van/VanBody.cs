using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanBody : MonoBehaviour {

    [SerializeField]
    private Vector2 force;
    [SerializeField]
    private Vector2 localPos;

    [SerializeField]
    private Vector2 engineForce;
    [SerializeField]
    private Vector2 engineLocalPos;
    [SerializeField]
    private float enginePeriod;

    private float engineForceDir = 1.0f;
    private bool canApplyEngineForce = true;

	private void FixedUpdate()
    {
        if (canApplyEngineForce)
        {
            StartCoroutine(ApplyEngineForce());
        }
    }

    IEnumerator ApplyEngineForce()
    {
        engineForce *= new Vector2(engineForceDir, engineForceDir);
        engineForceDir *= -1.0f;

        AddForceAtLocalPosition(engineForce, engineLocalPos);
        canApplyEngineForce = false;

        yield return new WaitForSeconds(enginePeriod);
        canApplyEngineForce = true;
    }

    public void AddForceAtLocalPosition(Vector2 force, Vector2 localForce)
    {
        AddForceAtPosition(force, transform.TransformPoint(localPos));
    }

    public void AddForceAtPosition(Vector2 force, Vector2 position)
    {
        GetComponent<Rigidbody2D>().AddForceAtPosition(force, position, ForceMode2D.Impulse);
    }
}
