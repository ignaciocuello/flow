using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSourcePool : Singleton<AudioSourcePool> {

    [SerializeField]
    private AudioMixerGroup audioMixer;

    private ObjectPool sourcePool;

    protected override void Initialize()
    {
        sourcePool = GetComponent<ObjectPool>();
    }

    void Update()
    {
        List<GameObject> sourcesInUse = sourcePool.GetObjectsInUse();
        foreach (GameObject sourceInUse in sourcesInUse)
        {
            //destroy the audio sources if they're not playing anything
            if (!sourceInUse.GetComponent<AudioSource>().isPlaying)
            {
                sourcePool.DestroyObject(sourceInUse);
            }
        }
    }

    public void PlayOneShotClipAt(AudioClip clip, Vector3 position)
    {
        PlayOneShotClipAt(clip, position, volumeScale: 1.0f);
    }

    public void PlayOneShotClipAt(AudioClip clip, Vector3 position, float volumeScale)
    {
        GameObject sourceObj = sourcePool.NewObject();
        sourceObj.transform.position = position;

        AudioSource source = sourceObj.GetComponent<AudioSource>();
        source.outputAudioMixerGroup = audioMixer;
        source.clip = clip;
        source.volume = volumeScale;

        source.Play();
    }

}
