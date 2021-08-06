using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PluginProg;

public abstract class Action : ScriptableObject
{

    public abstract void OnActionInit(BehaviourController behaviourController);
    public abstract void OnActionEnd(BehaviourController behaviourController);
    public abstract void Act(BehaviourController behaviourController);
    public abstract void OnActionStart(BehaviourController behaviourController);

    public Action Clone()
    {
        return Instantiate(this);
    }
}
