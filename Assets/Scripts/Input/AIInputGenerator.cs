using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInputGenerator : IInputGenerator {

    private Dictionary<string, InputData> inputs;
    private Dictionary<string, InputData> heldButtons;

    private string id;
    public string Id
    {
        get { return id; }
    }

    public AIInputGenerator(string id)
    {
        this.id = id;

        inputs = new Dictionary<string, InputData>();
        heldButtons = new Dictionary<string, InputData>();
    }

    public void Reset()
    {
        inputs.Clear();

        //do this to avoid OutOfSyncException
        List<string> keys = new List<string>();
        keys.AddRange(heldButtons.Keys);

        //var keys = heldButtons.Keys;
        foreach (string name in keys)
        {
            heldButtons[name] = new InputData(axisValue: 1.0f, held: true, down: false, up: false);
            inputs[name] = heldButtons[name];
        }
    }

    public float GetAxis(string name)
    {
        return inputs.ContainsKey(name) ? inputs[name].AxisValue : 0.0f;
    }

    public void SetAxis(string name, float value)
    {
        inputs[name] = new InputData(value, false, false, false);
    }

    public void TapButton(string name)
    {
        inputs.Add(name, new InputData(1.0f, held: true, down: true, up: true));
    }

    public void PressButton(string name)
    {
        heldButtons.Add(name, new InputData(1.0f, held: true, down: true, up: false));
        inputs.Add(name, heldButtons[name]);
    }

    public void ReleaseButton(string name)
    {
        //not using Add here because replacing an entry in the dictioanry is fine
        inputs[name] = new InputData(1.0f, held: false, down: false, up: true);
        heldButtons.Remove(name);
    }

    public bool GetButton(string name)
    {
        return inputs.ContainsKey(name) ? inputs[name].Held : false;
    }

    public bool GetButtonDown(string name)
    {
        return inputs.ContainsKey(name) ? inputs[name].Down : false;
    }

    public bool GetButtonUp(string name)
    {
        return inputs.ContainsKey(name) ? inputs[name].Up : false;
    }

    public bool IsUsingJoystick()
    {
        return false;
    }
}
