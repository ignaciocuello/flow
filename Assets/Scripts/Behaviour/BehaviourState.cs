using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviourState<TEntity> : ScriptableObject where TEntity : Entity {

	public virtual void BehaviourEnter(TEntity entity)
    {
    }

    public virtual void BehaviourFixedUpdate(TEntity entity)
    {
    }

    public virtual void BehaviourExit(TEntity entity)
    {
    }
}
