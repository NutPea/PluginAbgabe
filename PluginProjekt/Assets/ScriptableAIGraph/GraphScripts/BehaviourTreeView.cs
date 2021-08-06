using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using PluginProg;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class BehaviourTreeView : GraphView
{
    public Action<NodeView> OnNodeSelected;
    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> { }
    BehaviourTree tree;

    public BehaviourTreeView(){
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/ScriptableAIGraph/ScriptableAIEditor.uss");
        this.styleSheets.Add(styleSheet);

        var miniMap = new MiniMap();
        miniMap.anchored = true;
        miniMap.SetPosition(new Rect(700, 0, 200, 140));
        Add(miniMap);
    }

    public NodeView FindNodeView(PluginProg.Node node)
    {
        return GetNodeByGuid(node.GUID) as NodeView;
    }

    internal void PopulateView(BehaviourTree tree)
    {
        this.tree = tree;
        InitView();
       
    }

    public void InitView()
    {
        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements.ToList());
        graphViewChanged += OnGraphViewChanged;

        if(tree.rootNode == null)
        {

            RootNode rootNode = tree.CreateNode(typeof(RootNode)) as RootNode;
            rootNode.nodeName = "Start State";
            
            tree.rootNode = rootNode;

            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }

        if(tree.anyDecisiounNode == null)
        {
            AnyDecisiounNode anyDecisiounNode = tree.CreateNode(typeof(AnyDecisiounNode)) as AnyDecisiounNode;
            anyDecisiounNode.nodeName = "Any Decisioun";

            tree.anyDecisiounNode = anyDecisiounNode;

            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }
        
        //Create Node Views
        tree.nodes.ForEach(n => CreateNodeView(n));
        //Create Edges
        tree.nodes.ForEach(n => {
            var children = n.decisiounPorts;
            children.ForEach(c =>
            {
                NodeView parentView = FindNodeView(n);
                if (c.falseNode != null)
                {
                    NodeView falseNodeView = FindNodeView(c.falseNode);
                    Edge edge = c.falsePort.ConnectTo(falseNodeView.inputPort);
                    AddElement(edge);
                }
                if (c.trueNode != null)
                {
                    NodeView trueNodeView = FindNodeView(c.trueNode);
                    Edge edge = c.truePort.ConnectTo(trueNodeView.inputPort);
                    AddElement(edge);
                }
            });

        });
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort =>
        endPort.direction != startPort.direction && endPort.node != startPort.node)
            .ToList();
    }


    //Hier werden Edges verglichen und sachen verbunden wie States mit States :)
    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach(elem =>
            {
                NodeView nodeView = elem as NodeView;
                if(nodeView != null)
                {
                    tree.DeleteNode(nodeView.node);
                }

                Edge edge = elem as Edge;
                if(edge != null)
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;

                    tree.RemoveChildNode(parentView, childView.node , edge.output);
                }
            });

        }

        if(graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(edge =>
            {
                NodeView parentView = edge.output.node as NodeView;
                NodeView childView = edge.input.node as NodeView;

                tree.AddChildNode(parentView, childView.node , edge.output);
            });
        }
        return graphViewChange;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        //base.BuildContextualMenu(evt);
        {
            var type = typeof(StateNode);
            evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}",(a) => CreateNode(type));
        }
    }

    private void CreateNode(Type type)
    {
        PluginProg.Node node = tree.CreateNode(type);
        Debug.Log(node);
        CreateNodeView(node);
    }

    private void CreateNodeView(PluginProg.Node node)
    {
        NodeView nodeView = new NodeView(node);
        nodeView.OnNodeSelected += OnNodeSelected;
        
        AddElement(nodeView);

    }
    
    public void TreeUpdate()
    {

         OnChangeActionAndDecisiounList();

    }

    private void OnChangeActionAndDecisiounList()
    {
        if (tree.nodes.Count > 0)
        {
            tree.nodes.ForEach(n =>
            {
                if (n.GetType() == typeof(StateNode) || n.GetType() == typeof(RootNode))
                {
                    StateNode s = n as StateNode;
                    if (s.RefreshDecisiounPorts())
                    {
                        NodeView nodeView = FindNodeView(n);
                        nodeView.CreateOutputPorts();
                        InitView();
                    }
                    if (s.CheckRefreshActionLabels())
                    {
                        NodeView nodeView = FindNodeView(n);
                        nodeView.InitNodeView();
                    }
                    if (s.CheckRefreshChangedDecisiounList())
                    {
                        NodeView nodeView = FindNodeView(n);
                        nodeView.InitNodeView();
                    }
                    if (s.CheckRefreshChangedActionList())
                    {
                        NodeView nodeView = FindNodeView(n);
                        nodeView.InitNodeView();
                    }
                }
                if (n.GetType() == typeof(AnyDecisiounNode))
                {
                    StateNode s = n as StateNode;
                    if (s.CheckRefreshChangedDecisiounList())
                    {
                        NodeView nodeView = FindNodeView(n);
                        nodeView.InitNodeView();
                    }
                    if (s.RefreshDecisiounPorts())
                    {
                        NodeView nodeView = FindNodeView(n);
                        nodeView.CreateOutputPorts();
                        InitView();
                    }
                }

            });
        }
    }
}
