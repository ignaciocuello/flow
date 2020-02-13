using Rewired;
using System;
using System.Collections.Generic;
using UnityEngine;

public class InputBuffer : Singleton<InputBuffer> {

    //more recent inputs are at the beginning fo the list
    [SerializeField]
    private int bufferSize;

    private Dictionary<string, 
        Dictionary<string, List<InputData>>> buffer;
    //list  of  input generators in the order that they were added
    private List<IInputGenerator> inputGenerators;

    private PlayerInputGenerator playerOneInputGenerator;
    public PlayerInputGenerator PlayerOneInputGenerator
    {
        get { return playerOneInputGenerator; }
    }

    //false if the InputBuffer is reading input from the user,
    //true if using automated method, such as TAS controller
    public bool Manual;

    public bool IsUsingJoystick
    {
        get; private set;
    }

    protected override void Initialize()
    {
        buffer = new Dictionary<string, Dictionary<string, List<InputData>>>();
        inputGenerators = new List<IInputGenerator>();
    }

    void FixedUpdate()
    {
        if (!Manual)
        {
            foreach (IInputGenerator inputGenerator in inputGenerators)
            {
                foreach (InputAction inputAction in ReInput.mapping.Actions)
                {
                    string name = inputAction.name;
                    if (inputAction.type == InputActionType.Axis)
                    {
                        InputAxis(inputGenerator, name, inputGenerator.GetAxis(name));
                    }
                    else if (inputAction.type == InputActionType.Button)
                    {
                        bool held = inputGenerator.GetButton(name);
                        bool down = inputGenerator.GetButtonDown(name);
                        bool up = inputGenerator.GetButtonUp(name);

                        InputButton(inputGenerator, name, held, down, up);
                    }
                }
            }

            UpdateIsUsingJoyStick();
        }
    }

    //USE THESE AFTER AWAKE IN START OR AFTER
    public void AddInputGenerator(IInputGenerator inputGenerator)
    {
        buffer.Add(inputGenerator.Id, new Dictionary<string, List<InputData>>());

        inputGenerators.Add(inputGenerator);
        InitializeBuffer(inputGenerator);

        if (inputGenerators.Count == 1)
        {
            playerOneInputGenerator = (PlayerInputGenerator)inputGenerators[0];
        }
    }

    void UpdateIsUsingJoyStick()
    {
        IsUsingJoystick = PlayerOneInputGenerator != null ? PlayerOneInputGenerator.IsUsingJoystick() : false;
    }

    void InitializeBuffer(IInputGenerator inputGenerator)
    {
        foreach (InputAction inputAction in ReInput.mapping.Actions)
        {
            string name = inputAction.name;
            buffer[inputGenerator.Id][name] = new List<InputData>();
        }
    }

    List<InputData> UpdateList(List<InputData> inputList, InputData newData)
    {
        inputList.Insert(0, newData);
        if (inputList.Count > bufferSize)
        {
            inputList = inputList.GetRange(0, bufferSize);
        }

        return inputList;
    }

    // Input Button defaulting to player 1
    public void InputButton(string button, bool held, bool down, bool up)
    {
        InputButton(inputGenerators[0], button, held, down, up);
    }

    //down if button pressed this frame, held if button is held, up if button let go off this frame
    //button is the input name.
    public void InputButton(IInputGenerator inputGenerator, string button, bool held, bool down, bool up)
    {
        InputData id = new InputData(1.0f, held, down, up);
        buffer[inputGenerator.Id][button] = UpdateList(buffer[inputGenerator.Id][button], id);
    }

    public bool GetButton(string name)
    {
        return GetButton(name, 0);
    }

    public bool GetButton(IInputGenerator inputGenerator, string name)
    {
        return GetButton(inputGenerator, name, 0);
    }

    public bool GetButton(string name, int frameDelta)
    {
        return inputGenerators.Count > 0 ? GetButton(inputGenerators[0], name, frameDelta) : false;
    }

    /* frame delta is the number of frames in the past from the current frame,
     * we wish to query the input buffer, frameDelta = 0 is this frame */
    public bool GetButton(IInputGenerator inputGenerator, string name, int frameDelta)
    {
        return buffer[inputGenerator.Id][name].Count > 0 ? buffer[inputGenerator.Id][name][frameDelta].Held : false;
    }

    public bool GetButtonDown(string name)
    {
        return GetButtonDown(name, 0);
    }

    public bool GetButtonDown(IInputGenerator inputGenerator, string name)
    {
        return GetButtonDown(inputGenerator, name, 0);
    }

    public bool GetButtonDown(string name, int frameDelta)
    {
        return inputGenerators.Count > 0 ? GetButtonDown(inputGenerators[0], name, frameDelta) : false;
    }

    public bool GetButtonDown(IInputGenerator inputGenerator, string name, int frameDelta)
    {
        return buffer[inputGenerator.Id][name].Count > 0 ? buffer[inputGenerator.Id][name][frameDelta].Down : false;
    }

    public bool GetButtonDownBuffered(string name, bool consumeIfDown)
    {
        return inputGenerators.Count > 0 ? GetButtonDownBuffered(inputGenerators[0], name, consumeIfDown) : false;
    }

    //TODO: see if this messes up TAS recorder
    public bool GetButtonDownBuffered(IInputGenerator inputGenerator, string name, bool consumeIfDown)
    {
        return GetButtonDownBuffered(inputGenerator, name, consumeIfDown, bufferSize);
    }

    public bool GetButtonDownBuffered(IInputGenerator inputGenerator, string name, bool consumeIfDown, int maxBuffer)
    {
        List<InputData> inputData = buffer[inputGenerator.Id][name];
        for (int i = 0; i < maxBuffer && i < inputData.Count; i++)
        {
            InputData input = inputData[i];
            if (input.Down)
            {
                if (consumeIfDown)
                {
                    input.Down = false;
                }
                return true;
            }
        }

        return false;
    }

    public bool GetButtonDownMultipleBuffered(IInputGenerator inputGenerator, string name, int targetCount, int maxBuffer)
    {
        int count = 0;

        List<InputData> inputData = buffer[inputGenerator.Id][name];
        for (int i = 0; i < maxBuffer && i < inputData.Count; i++)
        {
            InputData input = inputData[i];
            if (input.Down)
            {
                count++;
                if (count >= targetCount)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool GetButtonUp(string name)
    {
        return inputGenerators.Count > 0 ? GetButtonUp(inputGenerators[0], name) : false;
    }

    public bool GetButtonUp(IInputGenerator inputGenerator, string name)
    {
        return buffer[inputGenerator.Id][name].Count > 0 ? buffer[inputGenerator.Id][name][0].Up : false;
    }

    public bool GetButtonUp(string name, int frameDelta)
    {
        return inputGenerators.Count > 0 ? GetButtonUp(inputGenerators[0], name, frameDelta) : false;
    }

    public bool GetButtonUp(IInputGenerator inputGenerator, string name, int frameDelta)
    {
        return buffer[inputGenerator.Id][name].Count > 0 ? buffer[inputGenerator.Id][name][frameDelta].Up : false;
    }

    public void InputAxis(string axis, float value)
    {
        InputAxis(inputGenerators[0], axis, value);
    }

    public void InputAxis(IInputGenerator inputGenerator, string axis, float value)
    {
        List<InputData> kbuffer = buffer[inputGenerator.Id][axis];
        bool held = value != 0.0f &&
            (kbuffer.Count > 0 && Mathf.Abs(kbuffer[0].AxisValue) > 0);
        //if axis direction changed this frame
        bool down = value != 0.0f &&
            (kbuffer.Count == 0 || ((kbuffer[0].AxisValue == 0.0f && value != 0.0f) || (kbuffer[0].AxisValue * value < 0.0f)));

        //not implemented yet;
        bool up = false;
        InputData id = new InputData(value, held, down, up);
        buffer[inputGenerator.Id][axis] = UpdateList(buffer[inputGenerator.Id][axis], id);
    }

    public bool CompareAxisBuffered(string axis, Func<float, bool> predicate)
    {
        return inputGenerators.Count > 0 ? CompareAxisBuffered(inputGenerators[0], axis, predicate) : false;
    }

    public bool CompareAxisBuffered(IInputGenerator inputGenerator, string axis, Func<float, bool> predicate)
    {
        return CompareAxisBuffered(inputGenerator, axis, predicate, bufferSize);
    }

    public bool CompareAxisBuffered(IInputGenerator inputGenerator, string axis, Func<float, bool> predicate, int maxBuffer)
    {
        List<InputData> inputData = buffer[inputGenerator.Id][axis];
        for (int i = 0; i < maxBuffer && i < inputData.Count; i++)
        {
            if (predicate(inputData[i].AxisValue))
            {
                return true;
            }
        }

        return false;
    }

    public bool CompareAxisMultipleBuffered(IInputGenerator inputGenerator, string name, int targetCount, 
        Func<float, bool> predicate, bool consumeIfDown, int maxBuffer)
    {
        int count = 0;

        List<int> toConsume = new List<int>();
        List<InputData> inputData = buffer[inputGenerator.Id][name];
        for (int i = 0; i < maxBuffer && i < inputData.Count; i++)
        {
            InputData input = inputData[i];
            if (predicate(inputData[i].AxisValue) && inputData[i].Down)
            {
                count++;
                if (consumeIfDown)
                {
                    toConsume.Add(i);
                }

                if (count >= targetCount)
                {
                    foreach (int j in toConsume)
                    {
                        inputData[j].Down = false;
                    }
                    return true;
                }
            }
        }

        return false;
    }

    public float GetAxis(string name)
    {
        return GetAxis(name, 0);
    }

    public float GetAxis(IInputGenerator inputGenerator, string name)
    {
        return GetAxis(inputGenerator, name, 0);
    }

    public float GetAxis(string name, int frameDelta)
    {
        return inputGenerators.Count > 0 ? GetAxis(inputGenerators[0], name, frameDelta) : 0.0f;
    }

    public float GetAxis(IInputGenerator inputGenerator, string name, int frameDelta)
    {
        return buffer[inputGenerator.Id][name].Count > 0 ? buffer[inputGenerator.Id][name][frameDelta].AxisValue : 0.0f;
    }

    /*
     * gets the axis value but only counts it as non-zero if its been active for more than
     * 1 frame, this is done to prevent an input being read in the opposite direction when
     * the player flicks the stick in one direction then quickly lets go, as the stick is 
     * snapping back it will register a small input on the other direction for 1 frame.
     */
    public float GetAxisNoSnap(string name)
    {
        return GetAxisSignDuration(name) < 2 ? 0.0f : GetAxis(name);
    }

    public float GetAxisNoSnap(IInputGenerator inputGenerator, string name)
    {
        return GetAxisSignDuration(inputGenerator, name) < 2 ? 0.0f : GetAxis(inputGenerator, name);
    }

    /* how many consecutive frames this axis has been on the same sign (positive or negative), useful for
     * preventing snap back when the joystick resets to zero from a flick.
     */
    public int GetAxisSignDuration(string name)
    {
        return inputGenerators.Count > 0 ? GetAxisSignDuration(inputGenerators[0], name) : 0;
    }

    public int GetAxisSignDuration(IInputGenerator inputGenerator, string name)
    {
        if (buffer[inputGenerator.Id][name].Count == 0)
        {
            return 0;
        }

        List<InputData> kbuffer = buffer[inputGenerator.Id][name];

        int signDuration = 1;
        float originalAxisVal = kbuffer[0].AxisValue;
        if (originalAxisVal == 0.0)
        {
            //return zero if axis not active at the moment
            return 0;
        }

        for (int i = 1; i < kbuffer.Count; i++)
        {
            /* if the result of the multiplication is less than equal to zero, then the axis value at
             * this index has a different sign (or is zero) either way stop the iteration*/
            if (kbuffer[i].AxisValue * originalAxisVal <= 0.0f)
            {
                return signDuration;
            }

            signDuration++;
        }

        return signDuration;
    }

    private InputData GetLast(List<InputData> kbuffer)
    {
        return kbuffer[0];
    }
}
