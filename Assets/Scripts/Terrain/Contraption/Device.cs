using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Device : MonoBehaviour  {

    //called when connection starts sending packet, but hasn't arrived  yet
    public virtual void PreHeat(Connection connection)
    {
    }

    public abstract void Activate();

    public abstract void Deactivate();

}
