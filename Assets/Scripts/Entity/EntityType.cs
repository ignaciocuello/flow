using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum EntityType {
    PLAYER, FOCUS, WAD, BLOOD_WAD, COIN, BLOOD_COIN
}

public static class EntityTypeExtensions
{
    private static Dictionary<EntityType, string> displayNameMap;

    static EntityTypeExtensions()
    {
        displayNameMap = new Dictionary<EntityType, string>();
        displayNameMap[EntityType.PLAYER] = "Player";
        displayNameMap[EntityType.FOCUS] = "Focus";
        displayNameMap[EntityType.WAD] = "Wad";
        displayNameMap[EntityType.BLOOD_WAD] = "BloodWad";
        displayNameMap[EntityType.COIN] = "Coin";
        displayNameMap[EntityType.BLOOD_COIN] = "BloodCoin";
    }

    public static string DisplayName(this EntityType type)
    {
        return displayNameMap[type];
    }
}
