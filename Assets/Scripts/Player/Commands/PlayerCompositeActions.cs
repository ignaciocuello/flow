using System;
using System.Collections.Generic;

[Serializable]
public enum PlayerCompositeActions {
    RUN, DRIFT, JUMP, WALL_JUMP, JUMP_GAIN, NORMAL_ATTACK, DOWN_ATTACK, RECALL
}

public static class PlayerActionsExtensions
{
    private static Dictionary<PlayerCompositeActions, string> displayNameMap;

    static PlayerActionsExtensions()
    {
        displayNameMap = new Dictionary<PlayerCompositeActions, string>();
        displayNameMap[PlayerCompositeActions.RUN] = "Run";
        displayNameMap[PlayerCompositeActions.DRIFT] = "Drift";
        displayNameMap[PlayerCompositeActions.JUMP] = "Jump";
        displayNameMap[PlayerCompositeActions.WALL_JUMP] = "Wall Jump";
        displayNameMap[PlayerCompositeActions.JUMP_GAIN] = "Jump Gain";
        displayNameMap[PlayerCompositeActions.NORMAL_ATTACK] = "Normal Attack";
        displayNameMap[PlayerCompositeActions.DOWN_ATTACK] = "Down Attack";
        displayNameMap[PlayerCompositeActions.RECALL] = "Recall";
    }

    public static string DisplayName(this PlayerCompositeActions action)
    {
        return displayNameMap[action];
    }
}
