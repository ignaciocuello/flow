using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBossBehaviour : BehaviourState<Boss> {

    [SerializeField]
    private float durationSeconds;

    public override void BehaviourEnter(Boss boss)
    {
        boss.StartCoroutine(DelayExit(boss));
    }

    IEnumerator DelayExit(Boss boss)
    {
        yield return new WaitForSeconds(durationSeconds);

        boss.Steal();
    }
}
