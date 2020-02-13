using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class Focus : Entity
{
    [SerializeField]
    private RestingFocusState restingState;
    [SerializeField]
    private StaticFocusState staticState;
    [SerializeField]
    private DeadFocusState deadState;

    [SerializeField]
    private HitStunFocusState hitStunState;
    public HitStunFocusState HitStunState
    {
        get { return hitStunState; }
    }
    [SerializeField]
    private HitStunStaticFocusState hitStunStaticState;
    public HitStunStaticFocusState HitStunStaticState
    {
        get { return hitStunStaticState; }
    }

    [Space(20)]
    [SerializeField]
    private List<RandClip> metalSounds;

    private ObjectTrail trail;
    public Target Target { get; set; }

    public override void Awake()
    {
        base.Awake();

        trail = GetComponent<ObjectTrail>();
        FacingRight = false;

        Rest();
    }

    /* instantitate states, if this is not done all focuses will share the same states, which 
     * causes issues */
    public override void InitStates()
    {
        base.InitStates();

        restingState = Instantiate(restingState);
        deadState = Instantiate(deadState);
        staticState = Instantiate(staticState);

        hitStunState = Instantiate(hitStunState);
        hitStunStaticState = Instantiate(hitStunStaticState);
    }

    public void Rest()
    {
        State = restingState;
    }

    public void Static(bool deriveRest)
    {
        staticState.DeriveRest = deriveRest;
        State = staticState;
    }

    public void Dead()
    {
        State = deadState;
    }

    public override void OnHitReceived(HitBox hitBox, HurtBox hurtBox, Entity initiator)
    {
        base.OnHitReceived(hitBox, hurtBox, initiator);

        AudioSourcePool.Instance.PlayOneShotClipAt(RandClip.GetRandomClip(metalSounds).AudioClip, transform.position);
    }

    //use kill so on death not invoked when object destroyed through other means outside game control
    public void Kill()
    {
        //State should be an instance of FocusState
        ((FocusState)State).Kill(this);
    }

    public void DestroyGameObject()
    {
        //TODO see if commenting this out causes bugs
        //trail.DestroyTrailChildren();

        if (EntitySpriteObject != null)
            EntitySpriteObject.gameObject.SetActive(false);

        StartCoroutine(DestroyCoroutine());
    }

    /* this is here so OnTriggerExit2D is called when Focus is destroyed */
    private IEnumerator DestroyCoroutine()
    {
        //TODO: remove if this causes issues
        transform.Translate(Vector2.up * 1000.0f);
        yield return new WaitForFixedUpdate();
        Destroy(gameObject);
    }
}
