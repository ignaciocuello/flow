using Newtonsoft.Json;
using Rewired;
using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class TASMap : SerializableDictionaryBase<int, TASInputList>
{
}

[Serializable]
public class TASInputList
{
    public List<TASInput> Inputs;

    public TASInputList()
    {
        Inputs = new List<TASInput>();
    }
}

[Serializable]
public class ActiveMap : SerializableDictionaryBase<string, TASInput>
{
}

//TODO FIX BUGS
public class TASController : MonoBehaviour {

    /* the inputs file corresponding to this game count will be read */
    [SerializeField]
    private int targetGameCount;
    /* true if TASController should read inputs from a file automatically */
    [SerializeField]
    private bool autoReadInputsFromFile;

    [SerializeField]
    private TASMap inputs;
    [SerializeField]
    private ActiveMap activeInputs;

    private void Awake()
    {
        if (enabled)
        {
            InputBuffer.Instance.Manual = true;

            if (autoReadInputsFromFile)
            {
                string path = TASRecorder.GetInputPath(targetGameCount);
                if (File.Exists(path))
                {
                    inputs = JsonConvert.DeserializeObject<TASMap>(File.ReadAllText(path));
                }
            }
        }
    }

    private void FixedUpdate()
    {
        UpdateActiveInputs();

        foreach (InputAction inputAction in ReInput.mapping.Actions)
        {
            string name = inputAction.name;
            if (inputAction.type == InputActionType.Axis)
            {
                float value;
                if (activeInputs.ContainsKey(name))
                {
                    TASInput input = activeInputs[name];
                    value = input.Value;
                    input.TimeToLive--;
                }
                else
                {
                    value = 0.0f;
                }
                InputBuffer.Instance.InputAxis(name, value);
            }
            else if (inputAction.type == InputActionType.Button)
            {
                bool held, down, up;
                if (activeInputs.ContainsKey(name))
                {
                    TASInput input = activeInputs[name];

                    held = true;
                    down = input.IsFirst();
                    up = (input.TimeToLive == 1);

                    input.TimeToLive--;
                }
                else
                {
                    held = false;
                    down = false;
                    up = false;
                }

                InputBuffer.Instance.InputButton(name, held, down, up);
            }
        }
    }

    private void UpdateActiveInputs()
    {
        RemoveDeadInputs();

        if (inputs.ContainsKey(GameManager.Instance.FrameCounter))
        {
            List<TASInput> newInputs = inputs[GameManager.Instance.FrameCounter].Inputs;
            foreach (TASInput input in newInputs)
            {
                if (input.TimeToLive > 0)
                {
                    activeInputs.Add(input.Name, input);
                }
            }
        }
    }

    private void RemoveDeadInputs()
    {
        List<string> toRemove = new List<string>();

        foreach (string name in activeInputs.Keys)
        {
            if (activeInputs[name].TimeToLive == 0)
            {
                toRemove.Add(name);
            }
        }

        //must use two different for loops to avoid concurrent modification exception
        foreach (string name in toRemove)
        {
            activeInputs.Remove(name);
        }
    }
}
