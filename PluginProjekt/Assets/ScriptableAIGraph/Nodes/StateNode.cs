using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PluginProg
{
    public class StateNode : Node
    {
        [SerializeField]private List<Action> actions = new List<Action>();
        [SerializeField]private List<Decision> decisiouns = new List<Decision>();
        public List<Decision> Decisiouns
        {
            get {
                    return decisiouns;
            }
        }

        public List<Action> Actions
        {
            get
            {
                return actions;
            }
        }

        bool init = false;

        public override void OnStart(BehaviourController behaviourController)
        {
            stateIsRunning = true;

            if (!init)
            {
                foreach (Action a in actions)
                {
                    a.OnActionInit(behaviourController);
                }

                foreach (Decision d in decisiouns)
                {
                    d.OnDecideInit(behaviourController);
                }
                init = true;
            }

            foreach(Action a in actions)
            {
                a.OnActionStart(behaviourController);
            }

            foreach(Decision d in decisiouns)
            {
                d.OnDecideStart(behaviourController);
            }
        }
        
        public override void OnStop(BehaviourController behaviourController)
        {
            stateIsRunning = false;

            foreach (Action a in actions)
            {
                a.OnActionEnd(behaviourController);
            }

            foreach (Decision d in decisiouns)
            {
                d.OnDecideEnd(behaviourController);
            }
        }
        public override void OnUpdate(BehaviourController behaviourController)
        {
            ActionHandler(behaviourController);
            DecisionHandler(behaviourController);
        }

        void ActionHandler(BehaviourController behaviourController)
        {
            foreach (Action a in actions)
            {
                a.Act(behaviourController);
            }
        }

        protected virtual void DecisionHandler(BehaviourController behaviourController)
        {
            if (decisiouns.Count == 0)
            {
                return;
            }

            for(int i = 0; i< decisiouns.Count; i++)
            {
                if (decisiouns[i].Decide(behaviourController))
                {
                    if(decisiounPorts[i].trueNode != null)
                    {
                        changedNode = decisiounPorts[i].trueNode as StateNode;
                        shouldChange = true;
                    }
                }
                else
                {
                    if (decisiounPorts[i].falseNode != null)
                    {
                        changedNode = decisiounPorts[i].falseNode as StateNode;
                        shouldChange = true;
                    }
                }
            }


        }

        protected bool shouldChange = false;
        protected StateNode changedNode;

        public StateNode OnChangeNode(BehaviourController behaviourController)
        {
            StateNode returnedNode = this as StateNode;
            if (shouldChange)
            {
                returnedNode = changedNode;
                OnStop(behaviourController);
                returnedNode.OnStart(behaviourController);
                shouldChange = false;
            }
            return returnedNode;
        }
        

        public override Node Clone()
        {
            StateNode cloneNode = Instantiate(this);
            cloneNode.actions = FillListWithActionClones(this.actions);
            cloneNode.decisiouns = this.decisiouns.ConvertAll(c => c.Clone());
            FillNewListWithDecisionPorts(ref cloneNode.decisiounPorts);
            return cloneNode;
        }

        List<Action> FillListWithActionClones(List<Action> copyList)
        {
            List<Action> emptyList = new List<Action>();
            foreach(Action a in copyList)
            {
                emptyList.Add(a.Clone());
            }
            return emptyList;
        }

        List<Decision> FillListWithDecisionClones(List<Decision> copyList)
        {
            List<Decision> emptyList = new List<Decision>();
            foreach (Decision d in copyList)
            {
                emptyList.Add(d.Clone());
            }
            return emptyList;
        }

        public bool RefreshDecisiounPorts()
        {
            bool hasBeenRefreshed = false;
            if (Decisiouns.Count > decisiounPorts.Count)
            {
                for (int i = 0; i < Decisiouns.Count - decisiounPorts.Count; i++)
                {
                    DecisionPorts emptyDecPort = new DecisionPorts();
                    decisiounPorts.Add(emptyDecPort);
                }
                hasBeenRefreshed = true;
            }

            if(Decisiouns.Count < decisiounPorts.Count)
            {
                List<DecisionPorts> decisiounPortsContainer = new List<DecisionPorts>();
                for (int i = 0; i < Decisiouns.Count; i++)
                {
                    decisiounPortsContainer.Add(decisiounPorts[i]);
                }
                decisiounPorts = decisiounPortsContainer;
                hasBeenRefreshed = true;
            }

            return hasBeenRefreshed;
        }


        private List<Decision> currentDecisiounListReferenz = new List<Decision>();
        private List<Decision> decisiounListReferenz = new List<Decision>();


        public bool CheckRefreshChangedDecisiounList()
        {
            return ListValuesHasBeenChanged<Decision>(ref currentDecisiounListReferenz,ref decisiounListReferenz,decisiouns);
        }

        private List<Action> currentActionListReferenz = new List<Action>();
        private List<Action> decisiounActionReferenz = new List<Action>();

        public bool CheckRefreshChangedActionList()
        {
            return ListValuesHasBeenChanged<Action>(ref currentActionListReferenz, ref decisiounActionReferenz, actions);
        }

        bool ListValuesHasBeenChanged<T>(ref List<T> currentList , ref List<T> unUpdatedCurrentList , List<T> realList)
        {
            currentList = realList;
            bool returnValue = false;
            if (currentList.Count == unUpdatedCurrentList.Count)
            {

                for (int i = 0; i < currentList.Count; i++)
                {
                    if (currentList[i] != null)
                    {
                        if (currentList[i].Equals(unUpdatedCurrentList[i]))
                        {
                            returnValue = false;
                        }
                        else
                        {
                            FillList(ref unUpdatedCurrentList, currentList);
                            returnValue = true;
                        }
                    }
                }

                if (realList.Count > 0)
                {
                    if (currentList[realList.Count - 1] == null)
                    {
                        if (unUpdatedCurrentList[realList.Count - 1] != null)
                        {
                            FillList(ref unUpdatedCurrentList, currentList);
                            returnValue = true;
                        }
                    }
                }

            }
            else
            {
                FillList(ref unUpdatedCurrentList, currentList);
            }
            return returnValue;
        }


        void FillList<T>(ref List<T> firstList , List<T> secondList)
        {
            firstList = new List<T>();
            foreach (T d in secondList)
            {
                firstList.Add(d);
            }
        }

        private int listLength;
        private int currentlistLength;
        public bool CheckRefreshActionLabels()
        {
            currentlistLength = actions.Count;
            if(currentlistLength != listLength)
            {
                listLength = currentlistLength;
                return true;
            }
            return false;
        }
        
    }
}
