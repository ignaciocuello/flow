using Rewired;
using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class KeyMap : SerializableDictionaryBase<KeyboardKeyCode, Sprite>
{
}

public class KBControllerImageMap : ScriptableObject {

    [SerializeField]
    private KeyMap keyMap;

    public bool TryGetValue(PlayerAtomicAction atomicAction, out List<Sprite> sprites)
    {
        List<Sprite> keySprites = new List<Sprite>();

        List<ActionElementMap> actionMaps = new List<ActionElementMap>();
        //get base atomic action e.g. if argument is "Down" base is "Vertical"
        InputBuffer.Instance.PlayerOneInputGenerator.GetElementMapsWithAction(
            atomicAction.Name, true, actionMaps);

        foreach (ActionElementMap actionMap in actionMaps)
        {
            if (atomicAction.Matches(actionMap))
            {
                if (actionMap.keyboardKeyCode != KeyboardKeyCode.None)
                {
                    Sprite keySprite;
                    if (keyMap.TryGetValue(actionMap.keyboardKeyCode, out keySprite))
                    {
                        keySprites.Add(keySprite);
                    }
                }
            }
        }

        if (keySprites.Count == 0)
        {
            sprites = null;
            return false;
        }
        else
        {
            sprites = keySprites;
            return true;
        }
    }
}
