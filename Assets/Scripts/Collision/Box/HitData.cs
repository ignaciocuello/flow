using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct HitData {

    /* trajectory modification data */
    public TrajectoryModData TrajectoryModData;

    /* momentum gain data */
    public MomentumGainData MomentumGainData;

    /* the number of frames the receiving entity is in hit stun */
    public int HitStunDuration;

    /* the number of frames both entities are in hit stop */
    public int HitStopDuration;

    /* the power of the hit stop */
    public float HitStopPower;
}
