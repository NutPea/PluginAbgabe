using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using PluginProg;


public class ScriptableAIEditor : EditorWindow
{
    InspectorView inspectorView;
    BehaviourTreeView treeView;

    bool treeHasbeenInit = false;
    [MenuItem("ScriptableAIEditor/Graph")]
    public static void OpenWindow()
    {
        ScriptableAIEditor wnd = GetWindow<ScriptableAIEditor>();
        wnd.titleContent = new GUIContent("ScriptableAIEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ScriptableAIGraph/ScriptableAIEditor.uxml");
        visualTree.CloneTree(root);


        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/ScriptableAIGraph/ScriptableAIEditor.uss");
        root.styleSheets.Add(styleSheet);
        treeView = root.Q<BehaviourTreeView>();
        inspectorView = root.Q<InspectorView>();
        treeView.OnNodeSelected = OnNodeSelectionChanged;
        OnSelectionChange();
    }

    public void OnGUI()
    {
        if (treeView != null && treeHasbeenInit)
        {
            treeView.TreeUpdate();
        }

    }

    private void OnSelectionChange()
    {
        BehaviourTree tree = Selection.activeObject as BehaviourTree;
        if (tree && AssetDatabase.OpenAsset(tree.GetInstanceID()))
        {
            treeView.PopulateView(tree);
        }
    }
    void OnNodeSelectionChanged(NodeView node)
    {
        treeHasbeenInit = true;
        inspectorView.UpdateSelection(node);
    }
}