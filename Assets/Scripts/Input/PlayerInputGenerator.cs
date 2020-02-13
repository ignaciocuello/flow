using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputGenerator : IInputGenerator {

    private static List<int> validPlayerIds;

    private Rewired.Player playerInput;

    public readonly int PlayerId;
    public string Id {
        get { return PlayerId.ToString(); }
    }

    public readonly bool Enabled;

    public PlayerInputGenerator(int playerId)
    {
        PlayerId = playerId;

        if (validPlayerIds == null)
        {
            validPlayerIds = new List<int>();
            validPlayerIds.AddRange(ReInput.players.GetPlayerIds());
        }

        if (validPlayerIds.Contains(playerId))
        {
            playerInput = ReInput.players.GetPlayer(playerId);
        }

        Enabled = playerInput != null;
    }

    public float GetAxis(string name)
    {
        return Enabled ? playerInput.GetAxis(name) : 0.0f;
    }

    public bool GetButton(string name)
    {
        return Enabled ? playerInput.GetButton(name) : false;
    }

    public bool GetButtonDown(string name)
    {
        return Enabled ? playerInput.GetButtonDown(name) : false;
    }

    public bool GetButtonUp(string name)
    {
        return Enabled ? playerInput.GetButtonUp(name) : false;
    }

    public void GetElementMapsWithAction(string actionName, bool skipDisabledMaps, List<ActionElementMap> results)
    {
        if (Enabled)
        {
            playerInput.controllers.maps.GetElementMapsWithAction(actionName, skipDisabledMaps, results);
        }
    }

    public bool IsUsingJoystick()
    {
        if (!Enabled)
        {
            return false;
        }

        if (playerInput.controllers.Keyboard.GetLastTimeActive() != ReInput.time.unscaledTime)
        {
            foreach (Joystick joystick in playerInput.controllers.Joysticks)
            {
                if (joystick.GetLastTimeActive() == ReInput.time.unscaledTime)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
