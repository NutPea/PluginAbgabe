using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PluginProg
{
    public class BehaviourController : MonoBehaviour
    {
        public BehaviourTree tree;
        void Start()
        {
            if (tree != null)
            {
                tree = tree.Clone();
                tree.OnStart(this);
            }
            else
            {
                Debug.Log("No Tree");
            }
        }

        // Update is called once per frame
        void Update()
        {
            tree.TreeUpdate(this);
        }
    }
}
