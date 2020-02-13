using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RandClip {

    /* the name of the audio clip in audio repository */
    [SerializeField]
    private AudioClip audioClip;
    public AudioClip AudioClip
    {
        get { return audioClip; }
    }

    /* probability of this sound being played */
    [SerializeField, Range(0, 1)]
    private float probability;
    public float Probability
    {
        get { return probability; }
    }

    public static RandClip GetRandomClip(List<RandClip> hitSounds)
    {
        float rand = UnityEngine.Random.Range(0.0f, 1.0f);

        int i = 0;
        float cumul = 0.0f;
        while (cumul < rand && i < hitSounds.Count)
        {
            cumul += hitSounds[i].Probability;
            i++;
        }

        //return first sound if probabilites add to less than 1
        if (i >= hitSounds.Count)
        {
            return hitSounds[0];
        }

        return hitSounds[i];
    }
}
