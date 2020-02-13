using Newtonsoft.Json;
using Rewired;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TASRecorder : MonoBehaviour {

    private const string INPUT_PATH = "Inputs";

    [SerializeField]
    private TASMap inputs;

    [SerializeField]
    private ActiveMap activeInputs;

	private void FixedUpdate()
    {
        foreach (InputAction inputAction in ReInput.mapping.Actions)
        {
            string name = inputAction.name;
            if (inputAction.type == InputActionType.Axis)
            {
                CheckAxisInputs(name);
            }
            else if (inputAction.type == InputActionType.Button)
            {
                CheckButtonInputs(name);
            }
        }
    }

    private void CheckAxisInputs(string name)
    {
        float axis = InputBuffer.Instance.GetAxis(name);

        //active will be non-null if input was active (non-zero) last frame
        TASInput active;
        activeInputs.TryGetValue(name, out active);

        if (axis != 0.0f)
        {
            if (active != null)
            {
                //if axis is still active and has the same value, increase its duration and time to live
                if (active.Value == axis)
                {
                    active.Duration++;
                    active.TimeToLive++;
                    return;
                }
                //no need to remove input from active dictionary as it will be replaced next
            }

            //create a new input with a duration of 1 frame
            active = new TASInput(name, axis, 1);
            activeInputs[name] = active;

            //add the input to the input list for this frame
            GetInputListForThisFrame().Inputs.Add(active);
        }
        else if (active != null)
        {
            activeInputs.Remove(name);
        }
    }

    private void CheckButtonInputs(string name)
    {
        bool held = InputBuffer.Instance.GetButton(name);
        bool down = InputBuffer.Instance.GetButtonDown(name);
        bool up = InputBuffer.Instance.GetButtonUp(name);

        //active will be non-null if input was active (non-zero) last frame
        TASInput active;
        activeInputs.TryGetValue(name, out active);

        if (held)
        {
            if (down)
            {
                //create a new input with a duration of 1 frame
                active = new TASInput(name, value:1.0f, duration:1);
                activeInputs[name] = active;

                //add the input to the input list for this frame
                GetInputListForThisFrame().Inputs.Add(active);
            }
            else
            {
                if (active == null)
                {
                    //create a new input with a duration of 1 frame
                    active = new TASInput(name, value: 1.0f, duration: 1);

                    //TODO CHANGE (this is a hacky way of doing this)
                    //increase the duration by 1, so that there is no first input (no down input)
                    //but it is still counted as held
                    active.Duration++;
                    activeInputs[name] = active;

                    //add the input to the input list for this frame
                    GetInputListForThisFrame().Inputs.Add(active);
                }
                else
                {
                    active.Duration++;
                    active.TimeToLive++;
                }
            }
        }
        else if (active != null)
        {
            activeInputs.Remove(name);
        }
    }

    private TASInputList GetInputListForThisFrame()
    {
        //if an input list already exists for this current frame use it, otherwise
        //create a new one
        TASInputList inputList;
        if (!inputs.TryGetValue(GameManager.Instance.FrameCounter, out inputList))
        {
            inputs[GameManager.Instance.FrameCounter] = inputList = new TASInputList();
        }

        return inputList;
    }

    private void OnDisable()
    {
        string path = GetInputPath(GameManager.Instance.GameCount);

        JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented
        };

        string json = JsonConvert.SerializeObject(inputs, settings);
        GameManager.Instance.DataWriter.WriteAllText(path, json);
    }

    public static string GetInputPath(int gameCount)
    {
        return GetInputPath(gameCount + ".json");
    }

    public static string GetInputPath(string append)
    {
        return Path.Combine(Application.streamingAssetsPath, Path.Combine(INPUT_PATH, append));
    }
}
