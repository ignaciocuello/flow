using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.IO;
using UnityEngine;

[Serializable]
public class SceneLoadMap : SerializableDictionaryBase<string, int>
{
}

public class InterSceneManager : MonoBehaviour {

    public static InterSceneManager Instance;

    /* maps scene names to the amount of times they have been loaded */
    [SerializeField]
    private SceneLoadMap sceneLoadMap;

    /* how many times the user has launched the game, used to keep all inputs in-between launches */
    public int GameCount {
        get; private set;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        string path = TASRecorder.GetInputPath("game_count.txt");
        if (File.Exists(path)) {
            GameCount = int.Parse(File.ReadAllText(path)) + 1;
        }
        else
        {
            GameCount = 1;
        }

        GameManager.Instance.DataWriter.WriteAllText(path, GameCount.ToString());
    }

	public void AddScene(string name)
    {
        if (sceneLoadMap.ContainsKey(name))
        {
            sceneLoadMap[name]++;
        }
        else
        {
            sceneLoadMap.Add(name, 1);
        }
    }

    public int GetSceneLoadAmounts(string name)
    {
        return sceneLoadMap[name];
    }
}
