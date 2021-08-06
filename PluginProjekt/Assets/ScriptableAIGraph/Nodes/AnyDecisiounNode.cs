using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PluginProg
{
    public class AnyDecisiounNode : StateNode
    {
        public override void OnUpdate(BehaviourController behaviourController)
        {
            AnyDecisionHandler (behaviourController);

        }

        public override Node Clone()
        {
            AnyDecisiounNode cloneNode = Instantiate(this);
            FillNewListWithDecisionPorts(ref cloneNode.decisiounPorts);
            return cloneNode;
        }

        protected void AnyDecisionHandler(BehaviourController behaviourController)
        {
            if (Decisiouns.Count == 0)
            {
                return;
            }

            for (int i = 0; i < Decisiouns.Count; i++)
            {
                if (Decisiouns[i].Decide(behaviourController))
                {
                    if (decisiounPorts[i].trueNode != null)
                    {
                        if (!decisiounPorts[i].trueNode.stateIsRunning)
                        {
                            changedNode = decisiounPorts[i].trueNode as StateNode;
                            shouldChange = true;
                        }
                    }
                }
                else
                {
                    if (decisiounPorts[i].falseNode != null)
                    {
                        if (!decisiounPorts[i].falseNode.stateIsRunning)
                        {
                            changedNode = decisiounPorts[i].falseNode as StateNode;
                            shouldChange = true;
                        }
                    }
                }
            }
        }

        public bool OnChangeAnyDecisionNode()
        {
            bool hasTriggertDecision = false;
            if (shouldChange)
            {
                hasTriggertDecision = true;
                shouldChange = false;
            }
            return hasTriggertDecision;
        }



        public StateNode GetChangedNode(BehaviourController behaviourController)
        {
            OnStop(behaviourController);
            changedNode.OnStart(behaviourController);
            return changedNode;
        }

    }
}
