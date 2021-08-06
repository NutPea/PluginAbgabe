using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PluginProg;
public abstract class Decision : ScriptableObject
{
    public abstract void OnDecideInit(BehaviourController behaviourController);
    public abstract void OnDecideStart(BehaviourController behaviourController);
    public abstract void OnDecideEnd(BehaviourController behaviourController);
    public abstract bool Decide(BehaviourController behaviourController);

    public Decision Clone()
    {
        return Instantiate(this);
    }
    
}
