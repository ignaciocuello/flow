using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScriptedSequence : MonoBehaviour {

    [SerializeField]
    private GameObject blackScreen;
    [SerializeField]
    private Boss boss;

    [SerializeField]
    private float targetSize;
    [SerializeField]
    private float sizeTrackingSpeed;

    [SerializeField]
    private AudioClip slapClip;

    [SerializeField, Tooltip("wait time until slap sound is played, this must be less than \'blackScreenDuration\'")]
    private float slapWaitTime;

    [SerializeField, Tooltip("how long the black screen is active for")]
    private float blackScreenDuration;

	public void OnPlayerEnteredAreaOnce(Player player)
    {
        StartCoroutine(PlayScriptedSequence(player));
    }

    public void OnBossEnteredArea(Boss boss)
    {
        EntityFactory.Instance.GetPlayer().ActivateKnockout();
    }

    IEnumerator PlayScriptedSequence(Player player)
    {
        GameObject screen = UserInterface.Instance.InstantiatePrefab(blackScreen, overlay: true);

        player.KnockOut();
        //TODO: remove this later, but this is to prevent exitStates that call derive state overridingn knock out
        player.KnockOut();
        player.FacingRight = false;

        boss.gameObject.SetActive(true);
        GetComponentInParent<Level>().LevelFinished();

        yield return new WaitForSecondsRealtime(slapWaitTime);

        TransformTracker.Instance.TargetSize = targetSize;
        TransformTracker.Instance.SizeTrackingSpeed = sizeTrackingSpeed;

        UserInterface.Instance.PlayClip(slapClip);

        float remainderWait = blackScreenDuration - slapWaitTime;
        if (remainderWait < 0)
        {
            throw new UnityException("blackScreenDuration < slapWaitTime");
        }

        yield return new WaitForSecondsRealtime(remainderWait);

        boss.Idle();

        //bring back screen to normal
        Destroy(screen.gameObject);
    }
}
