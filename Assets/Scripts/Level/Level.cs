using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Level : MonoBehaviour {

    private static readonly Vector2 DEFAULT_REWARD_RELATIVE_SPAWN_POINT = new Vector2(0.0f, -0.25f);

    [SerializeField]
    private string levelName;
    private LevelText levelText;

    [SerializeField]
    private List<LevelRewards> levelRewards;
    //composite actions learnt in previous level, knowledge that will be displayed
    [SerializeField]
    private List<PlayerCompositeActions> learnedActions;
    [SerializeField, Tooltip("index of audio track to play when level is first entered, -1 indicates for music to stop")]
    private int audioTrackIndex;

    [SerializeField]
    private AudioClip levelFinishedClip;

    //the camera size required for this level
    [SerializeField]
    private float cameraSize;
    public float CameraSize
    {
        get { return cameraSize; }
    }

    //where the camera should be centered for the level
    [SerializeField]
    private Vector3 cameraPosition;
    //offset the camera by this amount on level finish
    [SerializeField]
    private Vector3 offsetOnFinish;

    //the rate the camera should snap to its horizontal position
    [SerializeField]
    private float horizontalTrackingSpeed;
    //the rate the camera should snap to its vertical position
    [SerializeField]
    private float verticalTrackingSpeed;
    //the rate the camera should snap to its target size
    [SerializeField]
    private float sizeTrackingSpeed;

    //the camera's transform tracker
    private TransformTracker tracker;

    //true if the player has previously entered this area
    private bool hasBeenEntered;
    //true if the player has finished this level
    private bool hasBeenFinished;
    public bool HasBeenFinished
    {
        get { return hasBeenFinished; }
    }

    private void Awake()
    {
        tracker = Camera.main.GetComponentInParent<TransformTracker>();

        //sort level rewards in increasing order of frame thresholds
        levelRewards.Sort((x, y) => x.TimeThreshold.CompareTo(y.TimeThreshold));
    }

    public void TakeCameraData()
    {
        cameraPosition = Camera.main.gameObject.transform.parent.transform.position;
        cameraSize = Camera.main.orthographicSize;
    }

    public void OnEnterArea()
    {
        if (levelText == null)
        {
            levelText = UserInterface.Instance.Create(UIElemType.LEVEL_TEXT).GetComponent<LevelText>();
            levelText.GetComponent<Text>().text = levelName;
        }

        //comment below to get rid of
        TrackLevel();

        if (!hasBeenEntered)
        {
            OnFirstEnter();
        }

        hasBeenEntered = true;
    }

    public void OnExitArea()
    {
        if (levelText != null)
        {
            levelText.VisibleAnimator.Visible = false;
        }

        if (tracker.FixedTarget == cameraPosition)
        {
            //return to free-roam camera
            tracker.ResetToDefault();
            tracker.Offset = offsetOnFinish;
        }
    }

    public void DisableBeatColliders()
    {
        foreach (LevelBeat beat in GetComponentsInChildren<LevelBeat>())
        {
            beat.DisableCollider();
        }
    }

    private void TrackLevel()
    {
        tracker.FixedTarget = cameraPosition;
        tracker.TargetSize = cameraSize;

        if (horizontalTrackingSpeed > 0.0f)
        {
            tracker.HorizontalTrackingSpeed = horizontalTrackingSpeed;
        }
        if (verticalTrackingSpeed > 0.0f)
        {
            tracker.VerticalTrackingSpeed = verticalTrackingSpeed;
        }
        if (sizeTrackingSpeed > 0.0f)
        {
            tracker.SizeTrackingSpeed = sizeTrackingSpeed;
        }
    }

    private void OnFirstEnter()
    {
        ControllerPromptManager.Instance.KnownActions.UnionWith(learnedActions);
        //start tracking time for level
        StatsTracker.Instance.TimeTracker.StartMeasuring(ToString());

        GameManager.Instance.CrossFade.ClearTargets();
        GameManager.Instance.CrossFade.ResetLerp();
        if (audioTrackIndex == -1)
        {
            GameManager.Instance.StopMusic();
        }
        else
        {
            GameManager.Instance.CrossFade.AddTargetIndex(new TargetIndex(priority: 0.0f, target: audioTrackIndex));
        }
    }

    
    public void LevelFinished()
    {
        //TODO make centralized audio player or something
        UserInterface.Instance.PlayClip(levelFinishedClip);

        hasBeenFinished = true;
        StatsTracker.Instance.TimeTracker.UpdateMeasurementIfExists(ToString());

        GetComponentInChildren<EnterArea>().OnLevelFinish();
        GetComponentInChildren<ExitArea>().OnLevelFinish();

        DisableBeatColliders();
    }

    //return array of reward game objects
    public GameObject[] GiveRewards(Door door)
    {
        Vector2 rewardSpawnPoint = (Vector2)door.transform.position + DEFAULT_REWARD_RELATIVE_SPAWN_POINT  * door.GetComponent<BoxCollider2D>().size;
        return CheckRewards(rewardSpawnPoint);
    }

    private GameObject[] CheckRewards(Vector2 rewardSpawnPoint)
    {
        //if player has not entered level, but somehow finished it, reward them with a blood wad
        float timeTaken = StatsTracker.Instance.TimeTracker.MeasurementExists(ToString()) ? StatsTracker.Instance.TimeTracker.GetMeasurement(ToString()).Duration : 0.0f;
        foreach (LevelRewards reward in levelRewards)
        {
            if (timeTaken <= reward.TimeThreshold)
            {
                return reward.Give(rewardSpawnPoint);
            }
        }

        return new GameObject[] { };
    }
}
