using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CrossFade : MonoBehaviour {

    public const float NO_TARGET = -1.0f;
    private const float EPS = 0.005f;

    [SerializeField]
    private AudioClip[] audioClips;
    private AudioSource[] audioSources;

    [SerializeField]
    private AudioMixerGroup audioMixer;

    /* floating point index of current audio clip */
    private float current;

    private SortedList<float, TargetIndex> targetPriorityQueue;

    /* target volume to aim for */
    public float TargetVolume;

    private float maxVolume;

    private float defaultLerpRate;
    public float LerpRate;
	
    //make sure this script executes before the default time
    private void Awake()
    {
        defaultLerpRate = LerpRate;

        List<AudioSource> sourcesList = new List<AudioSource>();

        AudioSource[] currentSources = GetComponents<AudioSource>();
        sourcesList.AddRange(currentSources);

        while (sourcesList.Count < audioClips.Length)
        {
            sourcesList.Add(gameObject.AddComponent<AudioSource>());
        }

        int i = 0;
        foreach (AudioSource source in sourcesList)
        {
            source.outputAudioMixerGroup = audioMixer;
            source.Stop();
            source.clip = audioClips[i++];
            source.Play();
            source.loop = true;
        }

        audioSources = sourcesList.ToArray();

        targetPriorityQueue = new SortedList<float, TargetIndex>();
        targetPriorityQueue.Add(0.0f, new TargetIndex(0.0f, 0.0f));
    }

	void Update () {
        maxVolume = Mathf.Lerp(maxVolume, TargetVolume, Time.unscaledDeltaTime * LerpRate);

        float targetIndex = GetTargetIndex();
        if (targetIndex != NO_TARGET)
        {
            current = Mathf.Lerp(current, GetTargetIndex(), Time.unscaledDeltaTime * LerpRate);
        }

        //mute all audio sources
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.volume = 0.0f;
        }

        int index = Mathf.FloorToInt(current);

        //t is how much of audio source index + 1 to play, between (0 and 1)
        float t = current - index;
        if (t < EPS)
        {
            t = 0.0f;
        }
        else if ((1.0f - t) < EPS)
        {
            t = 1.0f;
        }

        audioSources[index].volume = TruncateVolume((1.0f - t) * maxVolume);

        if (index+1 < audioSources.Length)
        {
            audioSources[index + 1].volume = TruncateVolume(t * maxVolume);
        }
    }

    //if volume is close enough to zero it will truncate it to zero
    private float TruncateVolume(float volume)
    {
        return volume < EPS ? 0.0f : volume;
    }

    public void AddTargetIndex(TargetIndex targetIndex)
    {
        targetPriorityQueue[targetIndex.Priority] = targetIndex;
    }

    public void RemoveTargetIndex(float priority)
    {
        targetPriorityQueue.Remove(priority);
    }

    public void ClearTargets()
    {
        targetPriorityQueue.Clear();
    }


    public float GetTargetIndex()
    {
        return targetPriorityQueue.Count > 0 ? targetPriorityQueue.Values[targetPriorityQueue.Count - 1].Target : NO_TARGET;
    }

    public void ResetLerp()
    {
        LerpRate = defaultLerpRate;
    }
}
