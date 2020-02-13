using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputGenerator {

    float GetAxis(string name);

    bool GetButton(string name);

    bool GetButtonDown(string name);

    bool GetButtonUp(string name);

    bool IsUsingJoystick();

    string Id
    {
        get;
    }
}
