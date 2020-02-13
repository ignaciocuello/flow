using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunnedPlayerState : PlayerState {

    [SerializeField]
    private Vector2 eyesSpawnLocalPosition;
    [SerializeField]
    private GameObject eyesPrefab;

    private GameObject eyes;

	public override void EnterState(Player player)
    {
        base.EnterState(player);

        player.IsAffectedByDrag(true);
        player.Velocity = Vector2.zero;

        eyes = Instantiate(eyesPrefab, player.transform);
        eyes.transform.localPosition = 
            eyesSpawnLocalPosition * new Vector3(player.FacingRight ? 1.0f : -1.0f, 1.0f, 1.0f);
    }

    public override void StateFixedUpdate(Player entity)
    {
        //DON'T DO ANYTHING
    }

    public override void ExitState(Player entity)
    {
        Destroy(eyes);
    }

    public override string[] GetAvailableActions()
    {
        return new string[] { };
    }
}
