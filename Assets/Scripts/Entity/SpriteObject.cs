using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpriteRendererDict : SerializableDictionaryBase<string, SpriteRenderer>
{
}

public class SpriteObject : MonoBehaviour {

    [SerializeField]
    private SpriteRendererDict rendererDict;

    [SerializeField]
    private LayeredSprite sprite;
	public LayeredSprite Sprite
    {
        get { return sprite; }
        set
        {
            ClearRendererDict();

            sprite = value;
            if (sprite != null)
            {
                sprite.AssignTo(rendererDict);
            }
        }
    }

    private void ClearRendererDict()
    {
        foreach (SpriteRenderer sr in rendererDict.Values)
        {
            sr.sprite = null;
        }
    }
}
