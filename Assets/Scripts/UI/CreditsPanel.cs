using System.Collections;
using UnityEngine;

public class CreditsPanel : VisibleAnimatable {

    [SerializeField]
    private AudioClip cheeringAudioClip;

    [SerializeField]
    private Animator exitTextAnimator;

    [SerializeField]
    private float exitTextAppearDelay;

    public override void Awake()
    {
        base.Awake();
        VisibleAnimator.TargetMaterial.color = VisibleAnimator.TargetMaterial.color.ChangeAlpha(0.0f);

        VisibleAnimator.OnFullyVisible += () => {
            StartCoroutine(StartExitTextFade());
            UserInterface.Instance.PlayClip(cheeringAudioClip);
        };
    }

    private IEnumerator StartExitTextFade()
    {
        yield return new WaitForSecondsRealtime(exitTextAppearDelay);
        exitTextAnimator.SetBool("Fade", true);
    }
}
