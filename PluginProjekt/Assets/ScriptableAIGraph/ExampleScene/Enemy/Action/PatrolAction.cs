using PluginProg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "PatrolAction", menuName = "Action/PatrolAction")]
public class PatrolAction : Action
{
    int currentWaypointIndex;
    List<Transform> waypoints;
    NavMeshAgent navMeshAgent;


    public override void Act(BehaviourController behaviourController)
    {
        if(Vector3.Distance(behaviourController.gameObject.transform.position , waypoints[currentWaypointIndex].position) < 0.5)
        {
            currentWaypointIndex++;
            if(currentWaypointIndex > waypoints.Count - 1)
            {
                currentWaypointIndex = 0;
            }
            navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    public override void OnActionEnd(BehaviourController behaviourController)
    {
        
    }

    public override void OnActionInit(BehaviourController behaviourController)
    {
            waypoints = behaviourController.GetComponent<WaypointHandler>().waypoints;
            navMeshAgent = behaviourController.GetComponent<NavMeshAgent>();
            currentWaypointIndex = 0;
            navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    public override void OnActionStart(BehaviourController behaviourController)
    {

        navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
    }
}
