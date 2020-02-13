using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : Singleton<ScoreManager> {

    private const string SCORE_TABLE_NAME = "scores";
    private const string NAMES_TABLE_NAME = "names";

    public string ConnectionString = string.Empty;

    [SerializeField]
    private FrameData playerIdle;
    //first should be gold, then silver then bronze (order in list)
    [SerializeField]
    private List<LayeredSprite> crownSprites;

    [SerializeField]
    private float greetingsDuration;

    private CloudStorageAccount storageAccount;

    private CloudTable scoresTable;
    private CloudTable namesTable;

    //dict mapping machine names to player names
    private Dictionary<string, string> machineToNameMap;
    //list containing all ranking entities
    private List<ScoreEntity> rankings;

    private int totalRankingsCount;
    public int TotalRankingsCount
    {
        get { return Mathf.Max(totalRankingsCount, machineToNameMap.Count); }
        private set { totalRankingsCount = value; }
    }

    public int PlayerRanking
    {
        get; private set;
    }

    public event Action OnScoreIncrease;

    private Score maxScore;

    private void Start()
    {
        machineToNameMap = new Dictionary<string, string>();

        storageAccount = CloudStorageAccount.Parse(ConnectionString);
        CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

        scoresTable = tableClient.GetTableReference(SCORE_TABLE_NAME);
        namesTable = tableClient.GetTableReference(NAMES_TABLE_NAME);

        SetUpAndPopulate();
    }

    private async void SetUpAndPopulate()
    {
        await SetUpTables();

        await PopulateMachineToNameMap();

        PopulateScoreEntities();
    }

    private async Task SetUpTables()
    {
        try
        {
            await scoresTable.CreateIfNotExistsAsync();
        }
        catch (StorageException ex)
        {
            //TODO: handle
        }

        try
        {
            await namesTable.CreateIfNotExistsAsync();
        }
        catch (StorageException ex)
        {
            //TODO: handle
        }
    }
    
    private async Task PopulateMachineToNameMap()
    {
        List<NameEntity> nameEntities = await GetAllNameEntities();
        foreach (NameEntity n in nameEntities)
        {
            machineToNameMap[n.MachineName] = n.PlayerName;
        }
    }

    private async void PopulateScoreEntities()
    {
        rankings = await GetAllScoreEntities();
        foreach (ScoreEntity s in rankings)
        {
            if (s.MachineId == SystemInfo.deviceUniqueIdentifier)
            {
                maxScore = s.ToScore();
                break;
            }
        }

        SortRankings();

        playerIdle.Sprites[0].RemoveChild("Crown");

        int ranking = -1;

        //SHOW RANNKINNGS
        int num = 1;
        foreach (ScoreEntity s in rankings)
        {
            if (machineToNameMap.ContainsKey(s.MachineName))
            {
                if (s.MachineName == SystemInfo.deviceName)
                {
                    playerIdle.Sprites[0].AddChild(crownSprites[num - 1]);
                    ranking = num;
                }

                num++;
            }
        }

        if (machineToNameMap.ContainsKey(SystemInfo.deviceName))
        {
            VisibleAnimatable greetingsPanel = UserInterface.Instance.Create(UIElemType.GREETINGS_PANEL).GetComponent<VisibleAnimatable>();
            greetingsPanel.VisibleAnimator.Alpha = 0.0f;

            greetingsPanel.GetComponentInChildren<Text>().text = string.Format("Hey {0}! {1}", machineToNameMap[SystemInfo.deviceName], ranking != -1 ? "(ranked #" + ranking + ")" : "(unranked)");
            greetingsPanel.VisibleAnimator.Visible = true;
            greetingsPanel.VisibleAnimator.OnFullyVisible += () => StartCoroutine(DelayGreetingsPanelDisappear(greetingsPanel));
        }
    }

    private IEnumerator DelayGreetingsPanelDisappear(VisibleAnimatable greetingsPanel)
    {
        yield return new WaitForSecondsRealtime(greetingsDuration);

        greetingsPanel.VisibleAnimator.Visible = false;
        greetingsPanel.VisibleAnimator.OnFullyInvisible += () => Destroy(greetingsPanel.gameObject);
    }

    public async void AttemptWriteScore(Inventory inventory)
    {
        Score currentScore = inventory.ToScore();

        ScoreEntity scoreEntity = new ScoreEntity(SystemInfo.deviceUniqueIdentifier, SystemInfo.deviceName);
        scoreEntity.FromScore(currentScore);

        if (currentScore > maxScore)
        {
            Task<ScoreEntity> write = WriteScore(scoresTable, scoreEntity);

            UpdateLocalTable(scoreEntity);
            if (machineToNameMap.ContainsKey(SystemInfo.deviceName))
            {
                OnScoreIncrease?.Invoke();
            }

            await write;
        }
    }

    private void UpdateLocalTable(ScoreEntity current)
    {
        //TODO make this more efficient
        int oldIndex = rankings.IndexOf(new ScoreEntity(SystemInfo.deviceUniqueIdentifier, SystemInfo.deviceName));
        if (oldIndex >= 0)
        {
            rankings.RemoveAt(oldIndex);
        }

        rankings.Add(current);
        SortRankings();

        PlayerRanking = rankings.IndexOf(current) + 1;
        TotalRankingsCount = rankings.Count;
    }

    private void SortRankings()
    {
        //sort rankings in descending order
        rankings.Sort((s1, s2) => -s1.ToScore().CompareTo(s2.ToScore()));
    }

    private async Task<ScoreEntity> WriteScore(CloudTable cloudTable, ScoreEntity score)
    {
        TableOperation insertOrMerge = TableOperation.InsertOrMerge(score);

        TableResult result = await scoresTable.ExecuteAsync(insertOrMerge);
        ScoreEntity insertedRanking = result.Result as ScoreEntity;

        return insertedRanking;
    }

    public async Task<List<ScoreEntity>> GetAllScoreEntities()
    {
        TableQuery<ScoreEntity> allQuery = new TableQuery<ScoreEntity>();

        TableContinuationToken token = null;
        List<ScoreEntity> rankingEntities = new List<ScoreEntity>();

        do
        {
            TableQuerySegment<ScoreEntity> segment = await scoresTable.ExecuteQuerySegmentedAsync(allQuery, token);
            token = segment.ContinuationToken;

            rankingEntities.AddRange(segment);
        }
        while (token != null);

        return rankingEntities;
    }

    public async Task<List<NameEntity>> GetAllNameEntities()
    {
        TableQuery<NameEntity> allQuery = new TableQuery<NameEntity>();

        TableContinuationToken token = null;
        List<NameEntity> nameEntities = new List<NameEntity>();

        do
        {
            TableQuerySegment<NameEntity> segment = await namesTable.ExecuteQuerySegmentedAsync(allQuery, token);
            token = segment.ContinuationToken;

            nameEntities.AddRange(segment);
        }
        while (token != null);

        return nameEntities;
    }
}

public class ScoreEntity : TableEntity
{
    public ScoreEntity() { }

    public ScoreEntity(string machineId, string machineName)
    {
        PartitionKey = machineId;
        RowKey = machineName;
    }

    [IgnoreProperty]
    public string MachineId
    {
        get { return PartitionKey; }
    }
    [IgnoreProperty]
    public string MachineName
    {
        get { return RowKey; }
    }

    public int NumBloodCoins { get; set; }
    public int NumWads { get; set; }
    public int NumCoins { get; set; }

    public void FromScore(Score score)
    {
        NumBloodCoins = score.NumBloodCoins;
        NumWads = score.NumWads;
        NumCoins = score.NumCoins;
    }

    public Score ToScore()
    {
        return new Score(NumBloodCoins, NumWads, NumCoins);
    }

    public override bool Equals(object obj)
    {
        var ranking = obj as ScoreEntity;
        return obj != null && MachineId == ranking.MachineId;
    }

    public override int GetHashCode()
    {
        return -304207509 + EqualityComparer<string>.Default.GetHashCode(MachineId);
    }
}

public class NameEntity : TableEntity
{
    public NameEntity() { }

    public NameEntity(string machineName, string playerName)
    {
        PartitionKey = machineName;
        RowKey = playerName;
    }

    [IgnoreProperty]
    public string MachineName
    {
        get { return PartitionKey; }
    }
    [IgnoreProperty]
    public string PlayerName
    {
        get { return RowKey; }
    }
}
