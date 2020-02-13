using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PeriodicString
{
    public string Value;
    public float Duration;
    public bool Full;
    public bool Append;
}

public class PeriodicStringChanger : MonoBehaviour {

    [SerializeField]
    private List<AudioClip> keyboardSounds;

    [SerializeField]
    private List<PeriodicString> periodicStrings;

    private String str;
    public event Action<String> StringChanged;
    public event Action Finished;

    [SerializeField]
    private Text missionText;

    private void Awake()
    {
        if (missionText != null)
        {
            StringChanged += s => missionText.text = s;
        }
    }

    //do this in start, so other components can attach listeners to 'StringChanged' event
    public void Activate()
    {
        StartCoroutine(ChangeStringPeriodically());
        StringChanged += ChangeStr;
    }

    public void ChangeStr(String str)
    {
        this.str = str;

        AudioClip clip = keyboardSounds[UnityEngine.Random.Range(0, keyboardSounds.Count)];
        UserInterface.Instance.PlayClip(clip);
    }

    IEnumerator ChangeStringPeriodically()
    {
        foreach (PeriodicString p in periodicStrings)
        {
            if (p.Full)
            {
                String s = p.Append ? str : "";
                
                foreach (char c in p.Value)
                {
                    s += c;
                    StringChanged?.Invoke(s);
                    yield return new WaitForSecondsRealtime(p.Duration);
                }

                if (p.Append && p.Value.Length == 0)
                {
                    StringChanged?.Invoke(s.Substring(0, s.Length - 1));
                    yield return new WaitForSecondsRealtime(p.Duration);
                }
            }
            else
            {
                StringChanged?.Invoke(p.Value);
                yield return new WaitForSecondsRealtime(p.Duration);
            }
        }

        Finished?.Invoke();
    }
}
