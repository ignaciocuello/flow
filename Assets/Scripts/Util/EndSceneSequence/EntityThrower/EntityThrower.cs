using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityThrower : MonoBehaviour {

    private const string COLLECT_LAYER = "Collect";

    [SerializeField]
    TrajectoryModData trajectory;

    [SerializeField]
    private Vector2 impactGradientStart;
    [SerializeField]
    private Vector2 impactGradientEnd;

    [SerializeField]
    private float angularImpactGradientStart;
    [SerializeField]
    private float angularImpactGradientEnd;

    public int NumWadsSpawned { get; private set; }

    private int numSpawned;

    private void Awake()
    {
        if (EndSceneSequenceManager.Instance != null)
        {
            EndSceneSequenceManager.Instance.EntityThrower = this;
        }
    }

    public void ThrowWad()
    {
        Wad wad = EntityFactory.Instance.Create<Wad>();
        wad.FacingRight = true;
        wad.GetComponent<Rigidbody2D>().MovePosition(transform.position);
        wad.gameObject.layer = LayerMask.NameToLayer(COLLECT_LAYER);

        TrajectoryModData throwTrajectory = new TrajectoryModData(GetImpactFromGradient(), GetAngularImpactFromGradient(), trajectory);
        wad.Impact(throwTrajectory);

        NumWadsSpawned++;
    }

    public void ThrowPlayer()
    {
        Player player = EntityFactory.Instance.Create<Player>();
        player.PlayerId = ++numSpawned;
        //wad.GetComponent<Rigidbody2D>().MovePosition(transform.position);
        player.transform.position = transform.position;
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        player.FacingRight = true;
        player.gameObject.layer = LayerMask.NameToLayer(COLLECT_LAYER);

        TrajectoryModData throwTrajectory = new TrajectoryModData(GetImpactFromGradient() + new Vector2(-5.0f, 20.0f), GetAngularImpactFromGradient(), trajectory);
        player.Impact(throwTrajectory);
    }

    private Vector2 GetImpactFromGradient()
    {
        return Vector2.Lerp(impactGradientStart, impactGradientEnd, Random.Range(0.0f, 1.0f));
    }
    
    private float GetAngularImpactFromGradient()
    {
        return Mathf.Lerp(angularImpactGradientStart, angularImpactGradientEnd, Random.Range(0.0f, 1.0f));
    }
}
