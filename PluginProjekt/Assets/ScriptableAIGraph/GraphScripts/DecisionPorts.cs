using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace PluginProg
{
    [Serializable]
    public class DecisionPorts
    {
        public string decisiounName;
        public Port truePort;
        public Port falsePort;
        public Node trueNode;
        public Node falseNode;

        public void FillNewPorts(DecisionPorts value)
        {
            this.decisiounName = value.decisiounName;
            this.truePort = value.truePort;
            this.falsePort = value.falsePort;
            this.trueNode = value.trueNode;
            this.falseNode = value.falseNode;

        }

    }
}
