using PluginProg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "ChaseTargetAction", menuName = "Action/ChaseTargetAction")]
public class ChaseTargetAction : Action
{
    Transform target;
    NavMeshAgent navMeshAgent;

    public override void Act(BehaviourController behaviourController)
    {
       
        
        target = behaviourController.GetComponent<TargetHandler>().target;
        navMeshAgent = behaviourController.GetComponent<NavMeshAgent>();
        navMeshAgent.SetDestination(target.position);
    }

    public override void OnActionEnd(BehaviourController behaviourController)
    {
       
    }

    public override void OnActionInit(BehaviourController behaviourController)
    {
    }

    public override void OnActionStart(BehaviourController behaviourController)
    {
    }
}
