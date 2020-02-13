using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PrefabWrapper
{
    public GameObject Prefab;
    public bool UseWorld;
}

public class UserInterface : Singleton<UserInterface> {

    [SerializeField]
    private PrefabWrapper[] prefabs;

    private Dictionary<string, PrefabWrapper> prefabDict;
    private Dictionary<string, GameObject> uiElemDict;

    [Space(10)]

    [SerializeField]
    private RectTransform overlayCanvas;
    public RectTransform OverlayCanvas
    {
        get { return overlayCanvas; }
    }

    [SerializeField]
    private RectTransform worldCanvas;
    public RectTransform WorldCanvas
    {
        get { return worldCanvas; }
    }

    private AudioSource audioSource;

    public GameObject Create(UIElemType elemType)
    {
        PrefabWrapper prefabWrapper = prefabDict[elemType.DisplayName()];

        GameObject gameObj = InstantiatePrefab(prefabWrapper.Prefab, overlay: !prefabWrapper.UseWorld);
        uiElemDict[elemType.DisplayName()] = gameObj;

        return gameObj;
    }

    public GameObject InstantiatePrefab(GameObject prefab, bool overlay)
    {
        GameObject instance = Instantiate(prefab);
        instance.GetComponent<RectTransform>().SetParent(overlay ? overlayCanvas : worldCanvas, worldPositionStays: false);

        return instance;
    }

    public void Destroy(UIElemType elemType)
    {
        Destroy(Get(elemType));
    }

    public GameObject Get(UIElemType elemType)
    {
        string displayName = elemType.DisplayName();
        if (!uiElemDict.ContainsKey(displayName))
        {
            return null;
        }

        GameObject gameObj = uiElemDict[displayName];
        if (gameObj == null)
        {
            uiElemDict.Remove(displayName);
        }

        return gameObj;
    }

    protected override void Initialize()
    {
        audioSource = GetComponent<AudioSource>();

        prefabDict = new Dictionary<string, PrefabWrapper>();
        foreach (PrefabWrapper prefabWrapper in prefabs)
        {
            prefabDict.Add(prefabWrapper.Prefab.name, prefabWrapper);
        }

        uiElemDict = new Dictionary<string, GameObject>();
    }

    public void PlayClip(AudioClip clip)
    {
        PlayClip(clip, 1.0f);
    }

    public void PlayClip(AudioClip clip, float volume)
    {
        audioSource.volume = volume;
        audioSource.clip = clip;
        audioSource.Play();
    }
}
