using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PluginProg
{
    public abstract class Node : ScriptableObject
    {

        public  string nodeName = "Some Nody";

        [HideInInspector]public string GUID;
        [HideInInspector] public Vector2 position;
        [HideInInspector] public List<DecisionPorts> decisiounPorts = new List<DecisionPorts>();
        [HideInInspector] public bool stateIsRunning = false;

        public void NodeUpdate(BehaviourController behaviourController)
        {
            OnUpdate(behaviourController);
        }



        public abstract void OnStart(BehaviourController behaviourController);
        public abstract void OnStop(BehaviourController behaviourController);
        public abstract void OnUpdate(BehaviourController behaviourController);

        public virtual Node Clone()
        {
            Node cloneNode = Instantiate(this);
            FillNewListWithDecisionPorts(ref cloneNode.decisiounPorts);
            return cloneNode;
        }
        public void FillNewListWithDecisionPorts(ref List<DecisionPorts> targetList)
        {
            targetList = new List<DecisionPorts>();
            foreach(DecisionPorts dec in decisiounPorts)
            {
                DecisionPorts newPort = new DecisionPorts();
                newPort.FillNewPorts(dec);
                targetList.Add(newPort);
            }
        }


    }
}
