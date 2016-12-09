using System.Collections.Generic;
using System.Windows;

namespace TinyParser
{
    public enum NodeShape { Round, Square }

    public class SyntaxNode
    {
        public string Label { get; set; }
        public NodeShape Shape { get; set; }
        public SyntaxNode Sibling { get; set; }
        public List<SyntaxNode> ChildNodes { get; set; }

        public SyntaxNode()
        {
            ChildNodes = new List<SyntaxNode>();
        }
    }
}
