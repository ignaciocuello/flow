using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Charge {

    /*the maximum time that the attack can be charged for
   after this amount of time(in frames) has elapsed the
   attack will come out automatically and fully charged*/

    /*how much charge will affect the hit boxes in this FrameData
    if ChargeVector = (cx, cy) then if we're k% charged the hit boxes
    velocity will be ((1 + cx * k/100), (1 + cy * k/100)), a charge
    vector of (0, 0) will mean that the hitboxes are unaffected by charge
    . A charge vector of either cx < 0 or cy < 0 will make charging decrease
    knockback */
    [SerializeField]
    private Vector2 chargeVector;
    [SerializeField]
    private AttackPlayerState attack;
    public AttackPlayerState Attack
    {
        get { return attack; }
    }

    //TODO replace with gradient
    //color of entity on empty charge
    public Color NoChargeColor;
    //color of entity on mid charge
    public Color MidChargeColor;
    //color of entity on full charge
    public Color FullChargeColor;

    /* returns true if the input for the attack is currently active */
    [NonSerialized]
    public Func<bool> InputCheckFunc;

    [NonSerialized]
    public bool InputReleased;

    [NonSerialized]
    public float ChargeFraction;

    public void ChangePlayerChargeColor(Player player)
    {
        Color chargeColor;
        if (ChargeFraction < 0.5f)
        {
            //normalize charge fraction on 0.5f
            chargeColor = Color.Lerp(NoChargeColor, MidChargeColor, ChargeFraction / 0.5f);
        }
        else
        {
            //normalize charge fraction on 0.5f, shift by 0.5f left
            chargeColor = Color.Lerp(MidChargeColor, FullChargeColor, (ChargeFraction - 0.5f) / 0.5f);
        }
    }

    public void PerformChargedAttack(Player player)
    {
        Frame[] frames = GetChargedFrames();

        FrameData frameData = ScriptableObject.CreateInstance<FrameData>();
        frameData.Frames = frames;
        frameData.SpriteList = attack.FrameData.SpriteList;
        frameData.Sprites = attack.FrameData.Sprites;
        frameData.PlayBackSpeed = attack.FrameData.PlayBackSpeed;

        AttackPlayerState chargedAttack = UnityEngine.Object.Instantiate(attack);
        chargedAttack.FrameData = frameData;

        player.State = chargedAttack;
    }

    public Frame[] GetChargedFrames()
    {
        //dynamically generates a new frame data for the charged attack
        Frame[] chargedFrames = new Frame[attack.FrameData.Frames.Length];
        for (int i = 0; i < attack.FrameData.Frames.Length; i++)
        {
            chargedFrames[i] = new Frame();

            HitBox[] chargedHitBoxes = new HitBox[attack.FrameData.Frames[i].HitBoxes.Length];
            for (int j = 0; j < attack.FrameData.Frames[i].HitBoxes.Length; j++)
            {
                HitBox original = attack.FrameData.Frames[i].HitBoxes[j];
                chargedHitBoxes[j] = GetChargedBox(original);
            }

            chargedFrames[i].HurtBoxes = attack.FrameData.Frames[i].HurtBoxes;
            chargedFrames[i].HitBoxes = chargedHitBoxes;
        }

        return chargedFrames;
    }

    public HitBox GetChargedBox(HitBox uncharged)
    {
        HitBox chargedHitBox = new HitBox(
            ChargeVector(uncharged.HitData.TrajectoryModData.Impact, chargeVector),
            uncharged.HitData.TrajectoryModData.AngularImpact, uncharged)
        {
            Id = uncharged.Id
        };

        return chargedHitBox;
    }

    public Vector2 ChargeVector(Vector2 original, Vector2 chargeVector)
    {
        return new Vector2(
            ChargeVectorComponent(original.x, chargeVector.x),
            ChargeVectorComponent(original.y, chargeVector.y));
    }

    //component is x or y vector component
    public float ChargeVectorComponent(float component, float chargeFactor)
    {
        return (1.0f + chargeFactor * ChargeFraction) * component;
    }
}
