using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


namespace PluginProg
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public Action<NodeView> OnNodeSelected;
        public Node node;
        public Port inputPort;
        public List<DecisionPorts> decisiounPorts;

        public NodeView(Node node)
        {
            this.node = node;
            this.title = "";
            this.viewDataKey = node.GUID;

            InitNodeView();
        }



        public void InitNodeView()
        {
            style.left = node.position.x;
            style.top = node.position.y;

            Label stateName = new Label(node.nodeName);
            stateName.style.fontSize = 20f;
            titleContainer.Add(stateName);

            StateNode targetNode = node as StateNode;
            targetNode.RefreshDecisiounPorts();
            outputContainer.Clear();
            CreateOutputPorts();

            inputContainer.Clear();

            if (IsStateNode()) {
                CreateInputPorts();
                CreateActionListedActions();
            }

            if (IsRootNode())
            {
                this.titleContainer.style.backgroundColor = new Color(0,0,0.5f);
                this.capabilities &= ~Capabilities.Deletable;
                CreateInputPorts();
                CreateActionListedActions();
            }

            if (IsAnyDecisiounNode())
            {
                this.titleContainer.style.backgroundColor = new Color(0.5f, 0.5f, 0.1f);
                this.capabilities &= ~Capabilities.Deletable;
            }

            extensionContainer.Clear();
            InitStateColor();

        }
        void InitStateColor()
        {
            VisualElement runningState = new VisualElement();
            runningState.style.height = 20f;
            runningState.style.backgroundColor = Color.grey;

            extensionContainer.Add(runningState);
            RefreshExpandedState();
        }

        public void CreateActionListedActions()
        {

            ToolbarSpacer defaultSpacer = new ToolbarSpacer();
            IStyle spacerStyle = defaultSpacer.style;
            spacerStyle.height = 20;
            spacerStyle.width = 120;
            inputContainer.Add(defaultSpacer);

            VisualElement actionTitleContainer = new VisualElement();
            IStyle titleStyle = actionTitleContainer.style;
            titleStyle.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
            Label actionTitle = new Label("  Current Running Actions  ");
            IStyle actionTitleStyle = actionTitle.style;
            actionTitleStyle.fontSize = 15;

            actionTitleContainer.Add(actionTitle);
            inputContainer.Add(actionTitleContainer);

            inputContainer.Add(defaultSpacer);

            StateNode targetNode = node as StateNode;
            foreach(Action a in targetNode.Actions)
            {
                if (a != null)
                {
                    Label actionLabel = new Label( "              " +a.name);
                    actionLabel.style.fontSize = 12; 
                    inputContainer.Add(actionLabel);
                }
                else
                {
                    Label actionLabel = new Label("               Unknown");
                    actionLabel.style.fontSize = 12;
                    inputContainer.Add(actionLabel);
                }

            }
            
        }

        public void CreateOutputPorts()
        {
            decisiounPorts = new List<DecisionPorts>();
            StateNode targetNode = node as StateNode;
            foreach (Decision d in targetNode.Decisiouns)
            {
                DecisionPorts dp = new DecisionPorts();
                Port outputPortTrue = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
                Port outputPortFalse = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
                if(d != null)
                {
                    dp.decisiounName = d.name;
                }
                else
                {
                    dp.decisiounName = "Unknown";
                }
                outputPortTrue.portName ="true";
                outputPortFalse.portName = "false";
                outputPortTrue.portColor = new Color(0,0.8f,0);
                outputPortFalse.portColor = new Color(0.8f, 0, 0);

                dp.truePort = outputPortTrue;
                dp.falsePort = outputPortFalse;

                decisiounPorts.Add(dp);
            }

            if (decisiounPorts.Count != 0)
            {

                for (int i = 0; i < decisiounPorts.Count; i++)
                {
                    VisualElement decisiounContainer = new VisualElement();
                    IStyle decisiounStyle = decisiounContainer.style;
                    decisiounStyle.width = 120;
                    decisiounStyle.backgroundColor = new Color(0.15f, 0.15f, 0.15f);

                    Label title = new Label(decisiounPorts[i].decisiounName);
                    IStyle titleStyle = title.style;
                    titleStyle.fontSize = 12;

                    decisiounContainer.Add(title);
                    decisiounContainer.Add(decisiounPorts[i].truePort);
                    decisiounContainer.Add(decisiounPorts[i].falsePort);

                    outputContainer.Add(decisiounContainer);

                    ToolbarSpacer spacer = new ToolbarSpacer();
                    IStyle spacerStyle = spacer.style;
                    spacerStyle.height = 10;
                    outputContainer.Add(spacer);


                    node.decisiounPorts[i].truePort = decisiounPorts[i].truePort;
                    node.decisiounPorts[i].falsePort = decisiounPorts[i].falsePort;
                }
            }

        }

        private void CreateInputPorts()
        {
            if (node is StateNode)
            {
                inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            }
            if (inputPort != null)
            {
                Label inputTitle = new Label("State Input");
                inputContainer.Add(inputTitle);
                inputPort.portName = "";
                inputPort.portColor = Color.blue;
                inputContainer.Add(inputPort);
            }
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            node.position.x = newPos.xMin;
            node.position.y = newPos.yMin;
        }


        public override void OnSelected()
        {
            base.OnSelected();
            if(OnNodeSelected != null)
            {
                OnNodeSelected.Invoke(this);
            }
        }


         bool IsStateNode()
         {
            return node.GetType() == typeof(StateNode);
         }

        bool IsRootNode()
        {
            return node.GetType() == typeof(RootNode);
        }

        bool IsAnyDecisiounNode()
        {
            return node.GetType() == typeof(AnyDecisiounNode);
        }


    }

}
