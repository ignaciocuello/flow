
using UnityEngine;

public class Level1Tutorial : Tutorial {

    [SerializeField]
    private Vector3 cameraOffset;

    /* the time the run prompt remains on screen after the player has run */
    [SerializeField]
    private float runPromptDelayTime;

    /* true if the player has hit the focus */
    private bool hitFocus;
    /* true if the player has ran */
    private bool playerHasRan;

    /* this game's player */
    private Player player;

    /* camera's transform tracker */
    private TransformTracker tracker;

    private void Awake()
    {
        tracker = Camera.main.GetComponentInParent<TransformTracker>();
    }

    public void OnEnterPlayerState(EntityState playerState)
    {
        if (!playerHasRan)
        {
            if (playerState is RunningPlayerState)
            {
                playerHasRan = true;
                StartCoroutine(DelayDisableMainControlPrompt(runPromptDelayTime, ControllerPromptManager.Instance.MainPrompt, destroySelf: false));
            }
        }
    }

    public void OnHit(HitBox hitBox, HurtBox hurtBox, Entity receiving)
    {
        if (!hitFocus)
        {
            hitFocus = true;
            StartCoroutine(DelayDisableMainControlPrompt(runPromptDelayTime, ControllerPromptManager.Instance.MainPrompt, destroySelf: true));
        }
    }


    public void OnPlayerEnterMainArea(Player player)
    {
        this.player = player;
        player.OnEnterStateEvent.AddListener(OnEnterPlayerState);
        player.OnHitEvent.AddListener(OnHit);

        tracker.TargetSize = GetComponentInParent<Level>().CameraSize;
        tracker.Offset = cameraOffset;

        ControllerPromptManager.Instance.AssignActionToMainControllerPrompt(PlayerCompositeActions.RUN.DisplayName());
    }

    public void OnPlayerExitMainArea(Player player)
    {
        if (player == this.player)
        {
            ControllerPromptManager.Instance.DisableMainControllerPrompt();
            player.OnEnterStateEvent.RemoveListener(OnEnterPlayerState);
            player.OnHitEvent.RemoveListener(OnHit);

            Destroy(gameObject);
        }
    }

    public void OnPlayerEnterArea(Player player)
    {
        tracker.Offset = Vector2.zero;
        if (!hitFocus)
        {
            ControllerPromptManager.Instance.AssignActionToMainControllerPrompt(PlayerCompositeActions.NORMAL_ATTACK.DisplayName());
        }
    }   
}
