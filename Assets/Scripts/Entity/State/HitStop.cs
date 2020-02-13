using EZCameraShake;
using System;
using UnityEngine;

[Serializable]
public class HitStop : EntityState<Entity> {

    /* baseline camera shake parameters when power = 0*/
    [SerializeField]
    private CamShakeData baseCamShake;
    /* max camera shake paramters when power = 1 */
    [SerializeField]
    private CamShakeData maxCamShake;

    /* baseline sprite jitter parameters when power = 0 */
    [SerializeField]
    private Vector2 baseHitStopMagnitude;
    /* max hit stop magnitude when power = 1 */
    [SerializeField]
    private Vector2 maxHitStopMagnitude;

    /* whether this entity is receiving the hit stop or initiating it */
    [NonSerialized]
    public bool Receiving;
    /* how powerful this hit stop impact is which will determine how the camera shake + victim jitter is scaled*/
    [NonSerialized]
    public float Power;

    private Vector2 hitStopMagnitude;

    /* entity's sprite renderer position before hit stop starts */
    private Vector2 originalPosition;

    public override void EnterState(Entity entity)
    {
        //freeze the entity
        if (entity.EntitySpriteObject != null)
            originalPosition = entity.EntitySpriteObject.transform.position;

        entity.Constraint(RigidbodyConstraints2D.FreezeAll);

        //GET IMPACT HERE TO SCALE CAM SHAKE+HITSTOP
        if (Receiving)
        {
            entity.Flash();

            hitStopMagnitude = Vector2.Lerp(baseHitStopMagnitude, maxHitStopMagnitude, Power);

            CamShakeData.Lerp(baseCamShake, maxCamShake, Power).Shake();
        }
    }

    public override void StateFixedUpdate(Entity entity)
    {
        if (Receiving)
        {
            float damping = 1.0f - ScaledCurrentFrame / Duration;
            float direction = (((int)ScaledCurrentFrame) % 2 == 0 ? -1.0f : 1.0f);


            if (entity.EntitySpriteObject != null)
                entity.EntitySpriteObject.transform.position = originalPosition +
                    Vector2.Scale(damping * direction *  new Vector2(UnityEngine.Random.value, UnityEngine.Random.value), hitStopMagnitude);
        }
    }

    /* on last frame of hit stop the objects will move with physics, but their
    * sprites will still be frozen, this is a bug. This is because the phyiscs
    * update is called after fixed update, which is where exitstate is called from */
    public override void ExitState(Entity entity)
    {
        //unfreeze the entity
        entity.Unconstraint();
        if (entity.EntitySpriteObject != null)
            entity.EntitySpriteObject.transform.localPosition = Vector2.zero;
    }

    public override EntityState HitStunState(Entity entity)
    {
        throw new UnityException("Error no hit stun state in hit stop");
    }

    public override void DeriveState(Entity entity)
    {
        throw new UnityException("Error can't derive state in hit stop");
    }
}
