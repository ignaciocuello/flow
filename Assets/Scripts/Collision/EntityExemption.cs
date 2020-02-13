using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EntityExemption {

    public Entity IdOwner;
    public Entity Other;

    public string Id;

    public EntityExemption(Entity idOwner, Entity other, string id)
    {
        IdOwner = idOwner;
        Other = other;
        Id = id;
    }

    public bool EntityMatchesAny(Entity entity)
    {
        return IdOwner == entity || Other == entity;
    }

    public bool IsExempt(Entity owner1, Entity owner2, Box box1, Box box2)
    {
        if (IdOwner == owner1 && Other == owner2)
        {
            return box1.Id == Id;
        }
        else if (IdOwner == owner2 && Other == owner1)
        {
            return box2.Id == Id;
        }

        return false;
    }
}
