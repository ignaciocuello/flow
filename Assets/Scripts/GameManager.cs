using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {

    public const int ROOT_BUILD_INDEX = 0;
    public const int END_SCENE_SEQUENCE_INDEX = 1;
    public const int FIRST_SCENE_INDEX = 2;
    public const int SECOND_SCENE_INDEX = 3;

    [SerializeField]
    private GameObject managerFolder;

    [SerializeField]
    private GameObject audioSourcePoolPrefab;

    private DataWriter dataWriter;
    public DataWriter DataWriter
    {
        get { return dataWriter; }
    }

    private CrossFade crossFade;
    public CrossFade CrossFade
    {
        get { return crossFade; }
    }

    /* whether first room is loaded on awake */
    [SerializeField]
    private bool loadFirstRoom;

    /* number of times fixed update has been called since beginning of level */
    [SerializeField]
    private int frameCounter;
    public int FrameCounter
    {
        get
        {
            return frameCounter;
        }
    }

    /* how many times the user has launched the game, used to keep all inputs in-between launches */
    public int GameCount
    {
        get; private set;
    }

    private AudioSource audioSource;

    private void Start()
    {
        TransformTracker.Instance.TrackPlayer();
    }

    protected override void Initialize()
    {
        dataWriter = GetComponent<DataWriter>();
        audioSource = GetComponent<AudioSource>();
        crossFade = GetComponent<CrossFade>();

        Instantiate(audioSourcePoolPrefab, transform);

        frameCounter = 0;
        ReadGameCount();

        if (loadFirstRoom)
        {
            //load first level from root
            StartCoroutine(LoadSceneAsync(FIRST_SCENE_INDEX));
        }
    }

    private void ReadGameCount()
    {
        string path = TASRecorder.GetInputPath("game_count.txt");
        if (File.Exists(path))
        {
            GameCount = int.Parse(File.ReadAllText(path)) + 1;
        }
        else
        {
            GameCount = 1;
        }

        DataWriter.WriteAllText(path, GameCount.ToString());
    }

    private void FixedUpdate()
    {
        frameCounter++;

        if (InputBuffer.Instance.GetButton("Exit"))
        {
            Application.Quit();
        }
        if (InputBuffer.Instance.GetButton("Reset"))
        {
            ResetRoom();
        }
    }

    public void StopMusic()
    {
        crossFade.TargetVolume = 0.0f;
    }

    public GameObject InstantiateManager(GameObject managerPrefab)
    {
        return Instantiate(managerPrefab, managerFolder.transform);
    }

    public void ResetRoom()
    {
        TransformTracker.Instance.PrepareForReset();
        SceneManager.LoadScene(ROOT_BUILD_INDEX, LoadSceneMode.Single);
    }

    public IEnumerator LoadSceneSingle(int index)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public IEnumerator LoadSceneAsync(int index)
    {
        //don't bother loading if already loaded
        if (!SceneManager.GetSceneByBuildIndex(index).isLoaded)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
            //make scene active as soon as it is loaded
            asyncLoad.completed += a =>
                SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(index));

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
    }

    public IEnumerator UnloadSceneAsync(int index)
    {
        if (SceneManager.GetSceneByBuildIndex(index).isLoaded)
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(index);
            while (!asyncUnload.isDone)
            {
                yield return null;
            }
        }
    }
}
