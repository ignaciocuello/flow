using EZCameraShake;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wad : Entity {

    [SerializeField]
    private CamShakeData camShake;

    [SerializeField]
    private GameObject billParticleSystemPrefab;
    [SerializeField]
    private AudioClip collectClip;

    [SerializeField]
    private RestingWadState restingState;

    //this is here because Destroy doesn't make the equality check against null true instantly, to prevent double collection of wad
    private bool collected;

    public override void Awake()
    {
        base.Awake();

        FacingRight = false;

        Rest();
    }

    public override void InitStates()
    {
        base.InitStates();

        restingState = Instantiate(restingState);
    }

    public void Rest()
    {
        State = restingState;
    }

    public override void ApplyHitStop(int hitStopDuration, float hitStopPower, bool receiving)
    {
        //DO NOTHING
    }

    public override void ApplyHitStun(HitData hitData)
    {
        //DO NOTHING
    }

    public void Collect(Player player)
    {
        if (!collected)
        {
            player.Inventory.NumWads++;
            //only want ka-ching to be audible
            player.StopHitAudioSource();
            player.StopThudAudioSource();
            AudioSourcePool.Instance.PlayOneShotClipAt(collectClip, transform.position);

            GameObject billParticleSystem = Instantiate(billParticleSystemPrefab);
            billParticleSystem.transform.position = transform.position;

            camShake.Shake();

            Destroy(gameObject);

            collected = true;
        }
    }
}
