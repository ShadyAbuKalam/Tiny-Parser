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

    /*public class SyntaxNode
    {
        protected string Label { get; set; }
        protected SyntaxNode Sibling { get; set; }
        protected NodeShape Shape { get; set; }
    }

    public class IfNode : SyntaxNode
    {
        private SyntaxNode TestNode { get; set; }
        private SyntaxNode ThenNode { get; set; }
        private SyntaxNode ElseNode { get; set; }
    }

    public class RepeatNode : SyntaxNode
    {
        private SyntaxNode BodyNode { get; set; }
        private SyntaxNode TestNode { get; set; }
    }

    public class AssignNode : SyntaxNode
    {
        private string IdentifierName { get; set; }
        private SyntaxNode Expression { get; set; }
    }

    public class WriteNode : SyntaxNode
    {
        private SyntaxNode Expression { get; set; }
    }

    public class ReadNode : SyntaxNode
    {
        private string IdentifierName { get; set; }
    }

    public class OpNode : SyntaxNode
    {
        private string OpKind { get; set; }
        private SyntaxNode LeftOperandNode { get; set; }
        private SyntaxNode RightOperandNode { get; set; }
    }

    public class ConstNode : SyntaxNode
    {
        private string ConstValue { get; set; }
    }

    public class IdNode : SyntaxNode
    {
        private string IdentifierName { get; set; }
    }*/
}
