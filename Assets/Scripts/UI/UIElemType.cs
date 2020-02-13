using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIElemType
{
    DEBUG_TEXT, LEVEL_TEXT, PROGRESS_BAR, CONTROLLER_PROMPT_PANEL, MONEY_PANEL,
    MISSION_COMPLETE_PANEL, GREETINGS_PANEL, CREDITS_PANEL
}

public static class UIElemTypeExtensions
{

    private static Dictionary<UIElemType, string> displayNameMap;

    static UIElemTypeExtensions()
    {
        displayNameMap = new Dictionary<UIElemType, string>();
        displayNameMap[UIElemType.DEBUG_TEXT] = "DebugText";
        displayNameMap[UIElemType.LEVEL_TEXT] = "LevelText";
        displayNameMap[UIElemType.PROGRESS_BAR] = "ProgressBar";
        displayNameMap[UIElemType.CONTROLLER_PROMPT_PANEL] = "ControllerPromptPanel";
        displayNameMap[UIElemType.MONEY_PANEL] = "MoneyPanel";
        displayNameMap[UIElemType.MISSION_COMPLETE_PANEL] = "MissionCompletePanel";
        displayNameMap[UIElemType.GREETINGS_PANEL] = "GreetingsPanel";
        displayNameMap[UIElemType.CREDITS_PANEL] = "CreditsPanel";
    }

    public static string DisplayName(this UIElemType type)
    {
        return displayNameMap[type];
    }
}