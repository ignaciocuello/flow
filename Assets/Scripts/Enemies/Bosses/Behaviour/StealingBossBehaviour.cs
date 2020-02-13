using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealingBossBehaviour : BehaviourState<Boss> {

    [SerializeField]
    private float attackDelay;
    [SerializeField]
    private float finishDelay;

    private bool attacking;
    private bool finishing;

    public override void BehaviourEnter(Boss entity)
    {
        attacking = false;
        finishing = false;
    }

    public override void BehaviourFixedUpdate(Boss boss)
    {
        if (EntityFactory.Instance.GetPlayer().Inventory.NumWads > 0)
        {
            AIInputGenerator ai = (AIInputGenerator)boss.InputGenerator;
            ai.Reset();

            if (!attacking)
            {
                boss.StartCoroutine(Punch(ai));
            }
        }
        else if (!finishing)
        {
            boss.StartCoroutine(Finish(boss));
        }
    }

    IEnumerator Punch(AIInputGenerator ai)
    {
        attacking = true;
        ai.TapButton("Attack");

        yield return new WaitForSeconds(attackDelay);
        attacking = false;
    }


    IEnumerator Finish(Boss boss)
    {
        finishing = true;
        yield return new WaitForSeconds(finishDelay);

        boss.Escape();
    }
}
