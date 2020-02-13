using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour {

    [SerializeField]
    private VanBody vanBody;
    [SerializeField]
    private Vector2 force;
    [SerializeField]
    private float gasDuration;

    [SerializeField]
    private Van van;
    [SerializeField]
    private float motorSpeed;
    [SerializeField]
    private float artificalMaxRPM;
    [SerializeField]
    private float targetRPM;
    [SerializeField]
    private float horizontalTrackingSpeed;

	private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wad"))
        {
            Wad wad = collision.gameObject.GetComponent<Wad>();
            wad.Collect(EntityFactory.Instance.GetPlayer());

            vanBody.AddForceAtPosition(force, collision.contacts[0].point);
            van.ArtificalMaxRPM = artificalMaxRPM; 
            van.Gas(gasDuration);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(collision.gameObject);

            van.ArtificalMaxRPM = Mathf.Infinity;
            van.ReadyWheelsForDrive();
            van.GasUntil(targetRPM);
            van.TargetSpeed = motorSpeed;

            if (EndSceneSequenceManager.Instance.UseDriveSequence)
            {
                TransformTracker.Instance.FixedTarget = null;
                TransformTracker.Instance.Target = vanBody.transform;
                TransformTracker.Instance.HorizontalTrackingSpeed = horizontalTrackingSpeed;
            }
        }
    }
}
