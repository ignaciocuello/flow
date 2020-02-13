using Newtonsoft.Json;
using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class ValidInitFrames
{
    private HashSet<int> frameSet;
    public List<int> FrameCounts;

    public ValidInitFrames()
    {
        frameSet = new HashSet<int>();
        FrameCounts = new List<int>();
    }

    public void AddToFrameSet(int frame)
    {
        frameSet.Add(frame);

        FrameCounts.Clear();
        FrameCounts.AddRange(frameSet);
        FrameCounts.Sort();
    }

    public void CheckFrameStaleness(int oldest)
    {
        frameSet.RemoveWhere(f => f < oldest);

        FrameCounts.Clear();
        FrameCounts.AddRange(frameSet);
        FrameCounts.Sort();
    }
}

/* dictionary from attack names to valid init frames */
[Serializable]
public class ValidInitFramesDictionary : SerializableDictionaryBase<string, ValidInitFrames>
{
}

/* dictionary from frame numbers to hit attempt stats */
[Serializable]
public class HitAttemptDictionary : SerializableDictionaryBase<int, HitAttemptStats>
{
}

public class HitAccuracyTracker : MonoBehaviour {

    private const string FILE_NAME = "HitAccuracy";
    private const string INPUT_PATH = "Statistics/HitAccuracy";

    [SerializeField]
    private GameObject boxRendererPrefab;
    private List<SpriteRenderer> boxRenderers;

    [SerializeField]
    private bool showBoxRenderers;
    public bool ShowBoxRenderers
    {
        set
        {
            showBoxRenderers = value;
            if (boxRenderers != null)
            {
                foreach (SpriteRenderer boxRenderer in boxRenderers)
                {
                    boxRenderer.enabled = showBoxRenderers;
                }
            }
        }
    }

    /* the values below are always compared against the min frame delta */
    /* the maximum frames we allow the tracker to consider a move being early */
    [SerializeField]
    private float maxEarlyThreshold;

    /* the maximum frames we allow the tracker to consider a move being late */
    [SerializeField]
    private float maxLateThreshold;

    /* the player being measured */
    [SerializeField]
    private Player player;

    /* focuses in area of effect */
    [SerializeField]
    private List<Focus> focusesInArea;

    /* list of player attacks being measured */
    [SerializeField]
    private ChargeAttackPlayerState[] playerAttacks;
    /* bounds encapsulating all hit box bounds for each specific attack */
    private BoundsData[] playerAttackBounds;

    /* frames where if you had initiated an attack during said frames, that attack will have connected, indexed
     * by their corresponding attack */
    [SerializeField]
    private ValidInitFramesDictionary validInitFramesDictionary;

    [SerializeField]
    private float accuracy;

    [SerializeField]
    private float hits;
    private float Hits
    {
        get { return hits; }
        set
        {
            hits = value;
            accuracy = hits / attempts;
        }
    }

    [SerializeField]
    private float attempts;
    private float Attempts
    {
        get { return attempts; }
        set
        {
            attempts = value;
            accuracy = hits / attempts;
        }
    }

    [SerializeField]
    private HitAttemptDictionary hitAttemptDictionary;
    /* the frame in which the last hit stats were recorded */
    private int lastHitStatsFrame;

    private void Awake()
    {
        boxRenderers = new List<SpriteRenderer>();

        focusesInArea = new List<Focus>();
        playerAttackBounds = new BoundsData[playerAttacks.Length];
        GeneratePlayerAttackBounds();

        ShowBoxRenderers = showBoxRenderers;
    }

    private void OnValidate()
    {
        ShowBoxRenderers = showBoxRenderers;
    }

    private void GeneratePlayerAttackBounds()
    {
        for (int i = 0; i < playerAttackBounds.Length; i++)
        {
            playerAttackBounds[i] = ExtractPlayerAttackBounds(playerAttacks[i].Charge.Attack);

            boxRenderers.Add(Instantiate(boxRendererPrefab, transform).GetComponent<SpriteRenderer>());
            boxRenderers[i].transform.localScale = playerAttackBounds[i].Size;
            boxRenderers[i].transform.localPosition = playerAttackBounds[i].Center;
        }
    }

    private BoundsData ExtractPlayerAttackBounds(AttackPlayerState playerAttack)
    {
        Bounds bounds = new Bounds();

        foreach (Frame frame in playerAttack.FrameData.Frames)
        {
            foreach (Box box in frame.Boxes)
            {
                if (box is HitBox)
                {
                    bounds.Encapsulate(box.BoundsData.GetBounds());
                }
            }
        }

        return new BoundsData(bounds.center, bounds.size);
    }

    public void OnPlayerEnteredState(EntityState state)
    {
        if (state is AttackPlayerState)
        {
            AttackPlayerState attack = (AttackPlayerState)state;
            attack.name = attack.name.Replace("(Clone)", "").Trim();

            AddAttempt(attack);

            attack.OnHitEvent.AddListener(OnPlayerAttackHit);
            attack.OnEndEvent.AddListener(OnPlayerAttackEnded);
        }
    }

    public void OnPlayerAttackHit(Player player, AttackPlayerState attack)
    {
        AddHit(attack);
    }

    public void OnPlayerAttackEnded(Player player, AttackPlayerState attack)
    {
        HitAttemptStats stats = hitAttemptDictionary[lastHitStatsFrame];
        if ((stats.ResultType != ResultType.SUCCESS) && validInitFramesDictionary.ContainsKey(attack.name))
        {
            CalculateLateStats(stats, attack);
        }

        RemoveListeners(attack);
    }

    private void CalculateLateStats(HitAttemptStats stats, AttackPlayerState attack)
    {
        CalculateDeltaStats(stats, attack, maxLateThreshold);
    }

    private void CalculateEarlyStats(HitAttemptStats stats, AttackPlayerState attack)
    {
        CalculateDeltaStats(stats, attack, maxEarlyThreshold);
    }

    private void CalculateDeltaStats(HitAttemptStats stats, AttackPlayerState attack, float maxThreshold)
    {
        List<int> validInitFrames = validInitFramesDictionary[attack.name].FrameCounts;
        if (validInitFrames.Count > 0)
        {
            foreach (int frame in validInitFrames)
            {
                int frameDelta = lastHitStatsFrame - frame;

                //if delta within maximum alotted threshold
                if (Mathf.Abs(frameDelta) < maxThreshold)
                {
                    stats.AddToHitFrameDeltaSet(frameDelta);
                }
            }
        }
    }

    private float CalculateAverageHitFrameDelta(List<int> validInitFrames)
    {
        float avgHitFrameDelta = 0.0f;
        foreach (int validInitFrame in validInitFrames)
        {
            avgHitFrameDelta += (validInitFrame - lastHitStatsFrame);
        }
        avgHitFrameDelta /= validInitFrames.Count;

        return avgHitFrameDelta;
    }

    private void AddAttempt(AttackPlayerState attack)
    {
        HitAttemptStats stats = new HitAttemptStats(attack.name);

        hitAttemptDictionary.Add(GameManager.Instance.FrameCounter, stats);
        lastHitStatsFrame = GameManager.Instance.FrameCounter;

        Attempts++;
    }
    
    private void AddHit(AttackPlayerState attack)
    {
        HitAttemptStats stats = hitAttemptDictionary[lastHitStatsFrame];
        if (!stats.AttackName.Equals(attack.name))
        {
            Debug.LogError("Invalid hit add");
            return;
        }

        //only count hits once for multi-hit moves
        if ((stats.ResultType != ResultType.SUCCESS))
        {
            Hits++;
        }

        stats.ResultType = ResultType.SUCCESS;
        stats.MinHitFrameDelta = 0;
    }

    private void RemoveListeners(AttackPlayerState attack)
    {
        attack.OnHitEvent.RemoveListener(OnPlayerAttackHit);
        attack.OnEndEvent.RemoveListener(OnPlayerAttackEnded);
    }

	private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Focus"))
        {
            Focus focus = collider.GetComponent<Focus>();

            focusesInArea.Add(focus);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Focus"))
        {
            Focus focus = collider.GetComponent<Focus>();
            focusesInArea.Remove(focus);
        }
    }


    private void FixedUpdate()
    {
        int oldest = GameManager.Instance.FrameCounter - (int)Mathf.Max(maxEarlyThreshold, maxLateThreshold);
        foreach (string attackName in validInitFramesDictionary.Keys)
        {
            validInitFramesDictionary[attackName].CheckFrameStaleness(oldest);
        }

        if (!(player.State is AttackPlayerState))
        {
            foreach (Focus focus in focusesInArea)
            {
                Frame focusFrame =
                        focus.State.FrameData.Frames[(int)focus.State.ScaledCurrentFrame % focus.State.FrameData.Frames.Length];
                foreach (Box box in focusFrame.Boxes)
                {
                    if (box is HurtBox)
                    {
                        //for each attack check if the hurt box intersects with the approximate encapsulating
                        //bounds, if so perform intersection checks with individual frame hit box bounds
                        for (int i = 0; i < playerAttackBounds.Length; i++)
                        {
                            if (box.GetRelativeBounds(focus).Intersects(playerAttackBounds[i].GetRelativeBounds(player)))
                            {
                                PerAttackFrameIntersectionCheck(
                                    box.GetRelativeBounds(focus), playerAttacks[i].Charge.Attack);
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
    
    private void PerAttackFrameIntersectionCheck(Bounds centeredFocusBounds, AttackPlayerState attack)
    {
        //List<int> validInitFrames;
        ValidInitFrames validInitFrames;
        if (validInitFramesDictionary.ContainsKey(attack.name))
        {
            validInitFrames = validInitFramesDictionary[attack.name];
        }
        else
        {
            validInitFrames = new ValidInitFrames();
            validInitFramesDictionary.Add(attack.name, validInitFrames);
        }
        //get old behaviour by clearing the frameCounter here
        //reset valid init frames
        //validInitFrames.Clear();

        int position = 1;
        for (float normalizedCurrentFrame = 0.0f; 
                normalizedCurrentFrame < attack.FrameData.Frames.Length; 
                    normalizedCurrentFrame += Time.timeScale * attack.FrameData.PlayBackSpeed)
        {
            Frame frame = attack.FrameData.Frames[(int)normalizedCurrentFrame];
            foreach (Box box in frame.Boxes)
            {
                if (box is HitBox && 
                    (centeredFocusBounds.Intersects(box.GetRelativeBounds(player))))
                {
                    validInitFrames.AddToFrameSet(GameManager.Instance.FrameCounter - position);
                    break;
                }
            }

            position++;
        }

        //sort in ascending order

        /* make sure stats for last hit are present, this may not be the case if e.g. the player
         * hasn't attacked yet */
        if (hitAttemptDictionary.ContainsKey(lastHitStatsFrame))
        {
            HitAttemptStats stats = hitAttemptDictionary[lastHitStatsFrame];
            if ((stats.ResultType != ResultType.SUCCESS) && validInitFramesDictionary.ContainsKey(attack.name))
            {
                CalculateEarlyStats(stats, attack);
            }
        }
    }

    private void OnDisable()
    {
        string path = UniqueFileNameFinder.Find(GetPathFromName, FILE_NAME, ".csv");

        JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented
        };

        string json = JsonConvert.SerializeObject(hitAttemptDictionary, settings);
        GameManager.Instance.DataWriter.WriteAllText(GetPathFromName(path.Replace("csv", "json")), json);

        WriteToCSV(path);
    }

    public void WriteToCSV(string filename)
    {
        GameManager.Instance.DataWriter.WriteAllText(GetPathFromName(filename), ToCSVString());
    }

    private string ToCSVString()
    {
        string csv = "FrameCounter, " + HitAttemptStats.GetCSVHeader();
        
        foreach (int frameCount in hitAttemptDictionary.Keys)
        {
            csv += frameCount + ", " + hitAttemptDictionary[frameCount].ToCSVString();
        }

        return csv;
    }

    public static string GetPathFromGameCount(int gameCount)
    {
        return GetPathFromName(gameCount + ".json");
    }

    public static string GetPathFromName(string fileName)
    {
        return Path.Combine(Application.streamingAssetsPath, Path.Combine(INPUT_PATH, fileName));
    }
}
