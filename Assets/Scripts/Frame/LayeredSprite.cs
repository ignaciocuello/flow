using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayeredSprite : ScriptableObject {

    /* the name of the current layer */
    public string Name;
    /* the sprite of the current layer */
    public Sprite Sprite;

    /* child layers */
    public List<LayeredSprite> children;

    // assigns sprite layers to a hirearchical set of sprite renderers whenever the sprite layer's name
    // matches the dictionary key 
    public void AssignTo(IDictionary<string, SpriteRenderer> rendererDict)
    {
        rendererDict[Name].sprite = Sprite;
        if (children != null) {
            foreach (LayeredSprite child in children)
            {
                child.AssignTo(rendererDict);
            }
        }
    }

    public void AddChild(LayeredSprite child)
    {
        children.Add(child);
    }

    public void RemoveChild(string name)
    {
        List<LayeredSprite> toRemove = new List<LayeredSprite>();

        foreach (LayeredSprite child in children)
        {
            if (child.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                toRemove.Add(child);
            }
        }

        foreach (LayeredSprite rem in toRemove)
        {
            children.Remove(rem);
        }
    }

}
