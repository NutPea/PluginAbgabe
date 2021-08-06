using PluginProg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetIsInRangeDecision", menuName = "Decision/TargetIsInRangeDecision")]
public class TargetIsInRangeDecision : Decision
{
    public float distance;
    Transform target;
    bool init;


    public override bool Decide(BehaviourController behaviourController)
    {
        return (Vector3.Distance(behaviourController.transform.position, target.position) < distance);
    }

    public override void OnDecideEnd(BehaviourController behaviourController)
    {
       
    }

    public override void OnDecideInit(BehaviourController behaviourController)
    {
        target = behaviourController.GetComponent<TargetHandler>().target;
    }

    public override void OnDecideStart(BehaviourController behaviourController)
    {
    }
}
