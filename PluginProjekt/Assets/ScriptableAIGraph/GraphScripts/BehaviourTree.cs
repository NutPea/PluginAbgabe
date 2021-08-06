using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using PluginProg;

namespace PluginProg
{
    [CreateAssetMenu()]
    public class BehaviourTree : ScriptableObject
    {
        public Node rootNode = null;
        public AnyDecisiounNode anyDecisiounNode = null;
        public StateNode currentNode = null;
        public List<Node> nodes = new List<Node>();

        public void OnStart(BehaviourController behaviourController)
        {
            currentNode = rootNode as RootNode;
            currentNode.OnStart(behaviourController);
            anyDecisiounNode.OnStart(behaviourController);

        }

        public void TreeUpdate(BehaviourController behaviourController)
        {
            anyDecisiounNode.NodeUpdate(behaviourController);
            currentNode.NodeUpdate(behaviourController);

            if (anyDecisiounNode.OnChangeAnyDecisionNode())
            {
                currentNode = anyDecisiounNode.GetChangedNode(behaviourController);
            }
            else
            {
                currentNode = currentNode.OnChangeNode(behaviourController);
            }
        }



        public Node CreateNode(System.Type type)
        {
            Node node = ScriptableObject.CreateInstance(type) as Node;
            node.name = type.Name;
            node.GUID = GUID.Generate().ToString();
            nodes.Add(node);

            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();
            return node;
        }

        public void DeleteNode(Node node)
        {
            nodes.Remove(node);
            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
        }

        public void AddChildNode(NodeView parentView , Node child , Port targetPort)
        {
            StateNode stateNode = parentView.node as StateNode;
            if (stateNode)
            {
                foreach(DecisionPorts d in parentView.node.decisiounPorts)
                {
                    if(d.truePort == targetPort)
                    {
                        d.trueNode = child;
                    }
                    else if(d.falsePort == targetPort)
                    {
                        d.falseNode = child;
                    }
                }
            }
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        public void RemoveChildNode(NodeView parentView, Node child, Port targetPort)
        {
            StateNode stateNode = parentView.node as StateNode;
            if (stateNode)
            {
                foreach (DecisionPorts d in parentView.node.decisiounPorts)
                {
                    if (d.truePort == targetPort)
                    {
                        d.trueNode = null;
                    }
                    else if (d.falsePort == targetPort)
                    {
                        d.falseNode = null;
                    }
                }
            }
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }


        public BehaviourTree Clone()
        {
            BehaviourTree cloneTree = Instantiate(this);
            cloneTree.rootNode = this.rootNode.Clone();
            cloneTree.anyDecisiounNode = this.anyDecisiounNode.Clone() as AnyDecisiounNode;
            cloneTree.nodes = nodes.ConvertAll(n => n.Clone());
            return cloneTree;
        }
    }

}
