using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSceneSequenceManager : Singleton<EndSceneSequenceManager> {

    [SerializeField, Tooltip("Camera position while waiting for sequence to start")]
    private Vector3 waitingFixedCameraPosition;
    [SerializeField, Tooltip("Camera position once sequence has started")]
    private Vector3 sequenceFixedCameraPosition;
    [SerializeField]
    private float horizontalTrackingSpeed;

    public EntityThrower EntityThrower;
    public GameObject ParkingWall;
    public Van Van;

    private Inventory target;

    [SerializeField, Tooltip("if better than percentage >= this value get away will be shown")]
    private float betterThanPercentageThreshold;
    [SerializeField, Tooltip("if ranking <= this value get away will be shown")]
    private float rankingThreshold;

    [SerializeField]
    private float missionCompleteVisibleTime;
    [SerializeField]
    private float waitTimeUntilCameraPans;
    [SerializeField]
    private float waitTimeUntilPlayerInVan;
    [SerializeField]
    private float waitTimeUntilCredits;

    private bool useDriveSequence;
    public bool UseDriveSequence
    {
        get { return useDriveSequence; }
    }

    protected override void Initialize()
    {
        target = GetComponent<Inventory>();

        SetUpMissionCompletePanel();

        //get rid of progress bar and controller prompts, also fade money panel out, if its visible
        UserInterface.Instance.Destroy(UIElemType.PROGRESS_BAR);
        UserInterface.Instance.Destroy(UIElemType.CONTROLLER_PROMPT_PANEL);

        UserInterface.Instance.Get(UIElemType.MONEY_PANEL).GetComponent<MoneyPanel>().VisibleAnimator.Visible = false;

        ScoreManager.Instance.OnScoreIncrease += CheckIfUseDriveSequence;
        GameManager.Instance.StopMusic();
    }

    private void CheckIfUseDriveSequence()
    {
        int ranking = ScoreManager.Instance.PlayerRanking;
        int totalRankings = ScoreManager.Instance.TotalRankingsCount;

        float betterThanPercentage = 100.0f * (1.0f - (ranking - 1.0f) / (totalRankings - 1.0f));

        //TODO: set TargetRanking here

        useDriveSequence = betterThanPercentage >= betterThanPercentageThreshold || ranking <= rankingThreshold;
    }

    private void SetUpMissionCompletePanel()
    {
        VisibleAnimatable missionCompletePanel = UserInterface.Instance.Create(UIElemType.MISSION_COMPLETE_PANEL).GetComponent<VisibleAnimatable>();
        missionCompletePanel.VisibleAnimator.OnFullyVisible += OnMissionCompletePanelFullyVisibleCallback;

        SetUpMissionCompleteText();

        missionCompletePanel.VisibleAnimator.Visible = true;
    }

    private void OnMissionCompletePanelFullyVisibleCallback()
    {
        Inventory inventory = EntityFactory.Instance.GetPlayer().Inventory;

        ScoreManager.Instance.AttemptWriteScore(inventory);
        
        //get target values
        target.Copy(inventory);

        UserInterface.Instance.Get(UIElemType.MISSION_COMPLETE_PANEL).GetComponentInChildren<PeriodicStringChanger>().Activate();
    }


    private void SetUpMissionCompleteText()
    {
        VisibleAnimatable completePanel = UserInterface.Instance.Get(UIElemType.MISSION_COMPLETE_PANEL).GetComponent<VisibleAnimatable>();

        completePanel.GetComponentInChildren<PeriodicStringChanger>().Finished += () => StartCoroutine(LoadEndSceneSequenceAndWait());
        completePanel.VisibleAnimator.OnFullyInvisible += () => StartCoroutine(StartGetAwayEndSceneSequence());
    }

    IEnumerator LoadEndSceneSequenceAndWait()
    {
        //unload level and load get away scene
        yield return StartCoroutine(GameManager.Instance.UnloadSceneAsync(GameManager.FIRST_SCENE_INDEX));
        yield return StartCoroutine(GameManager.Instance.LoadSceneAsync(GameManager.END_SCENE_SEQUENCE_INDEX));

        //set camera position to waiting stance
        TransformTracker.Instance.transform.position = waitingFixedCameraPosition;
        TransformTracker.Instance.FixedTarget = waitingFixedCameraPosition;
        TransformTracker.Instance.HorizontalTrackingSpeed = horizontalTrackingSpeed;

        yield return new WaitForSecondsRealtime(missionCompleteVisibleTime);

        UserInterface.Instance.Get(UIElemType.MISSION_COMPLETE_PANEL).GetComponent<VisibleAnimatable>().VisibleAnimator.Visible = false;
    }

    
    IEnumerator StartGetAwayEndSceneSequence()
    {
        //endSceneSequence(scene) guaranteed to be loaded here, see structure of LoadEndSceneSequenceAndWait

        //wait a bit
        yield return new WaitForSecondsRealtime(waitTimeUntilCameraPans);

        BeginGetAwaySequence();
        UserInterface.Instance.Get(UIElemType.MISSION_COMPLETE_PANEL).GetComponent<VisibleAnimatable>().VisibleAnimator.Visible = false;

        Inventory inventory = EntityFactory.Instance.GetPlayer().Inventory;
        while (inventory.NumWads < target.NumWads)
        {
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSecondsRealtime(waitTimeUntilPlayerInVan);

        EndGetAwaySequence();
    }

    private void BeginGetAwaySequence()
    {
        //set up player inventory
        Inventory inventory = EntityFactory.Instance.GetPlayer().Inventory;

        inventory.NumWads = 0;
        inventory.NumCoins = target.NumCoins;

        UserInterface.Instance.Get(UIElemType.MONEY_PANEL).GetComponent<MoneyPanel>().SetAnimateAdd(true);

        //start engine rumble
        Van.GetComponent<AudioSource>().Play();
        TransformTracker.Instance.FixedTarget = sequenceFixedCameraPosition;
    }

    public void EndGetAwaySequence()
    {
        EntityThrower.ThrowPlayer();
        ParkingWall.SetActive(false);

        if (useDriveSequence)
        {
            ParkingWall.GetComponent<ParkingWall>().OnVanCollision += GoToCredits;
        }
        else
        {
            GoToCredits(null);
        }
    }

    private void GoToCredits(Van van)
    {
        StartCoroutine(GoToCreditsDelayed());
    }

    private IEnumerator GoToCreditsDelayed()
    {
        ParkingWall.GetComponent<ParkingWall>().OnVanCollision -= GoToCredits;

        yield return new WaitForSecondsRealtime(waitTimeUntilCredits);

        Van.GetComponent<VolumeFade>().TargetVolume = 0.0f;
        UserInterface.Instance.Create(UIElemType.CREDITS_PANEL).GetComponent<VisibleAnimatable>().VisibleAnimator.Visible = true;
    }

    private void FixedUpdate()
    {
        if (InputBuffer.Instance.GetButtonDown(PlayerAtomicAction.ATTACK.Name))
        {
            if (EntityThrower != null && EntityThrower.NumWadsSpawned < target.NumWads)
            {
                EntityThrower.ThrowWad();
            }
        }
    }
	
	

    
}
