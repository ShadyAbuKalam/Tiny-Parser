using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Shields.GraphViz.Components;
using Shields.GraphViz.Models;
using Shields.GraphViz.Services;

namespace TinyParser
{
    /// <summary>
    /// Interaction logic for GraphingWindow.xaml
    /// </summary>
    public partial class GraphingWindow
    {
        Graph _graph ;
        static int _nodeIndex;
        readonly Dictionary<SyntaxNode, NodeId> _idsDict = new Dictionary<SyntaxNode, NodeId>();

        public GraphingWindow()
        {
            InitializeComponent();
        }

        private string GetGraphvizBinaryPath()
        {
            const string cmd = "dot";
            var proc = new Process
            {
                StartInfo =
                {
                    FileName = "where.exe",
                    Arguments = cmd,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };
            proc.Start();
            proc.WaitForExit();

            if (proc.ExitCode == 0)
            {
                var output = proc.StandardOutput.ReadToEnd();
                output = Path.GetDirectoryName(output.Substring(0, output.Length - 2));
                proc.Close();
                return output;

            }
            proc.Close();
            throw new Exception("Graphviz is not installed on your system or Its bin folder is  not registered in PATH");
        }

        
        public async void Draw(SyntaxNode node)
        {
            try
            {
                _idsDict.Clear();
                _nodeIndex = 0;

                _graph = Graph.Undirected.Add(AttributeStatement.Graph.Set("splines", "true"));

                DrawNode(node);
                IRenderer renderer = new Renderer(GetGraphvizBinaryPath());
                var fileName = Path.GetTempPath() + Guid.NewGuid() + ".png";
                Stream file = File.Create(fileName);

                do
                {
                    await Task.WhenAll(renderer.RunAsync(
                        _graph, file,
                        RendererLayouts.Dot,
                        RendererFormats.Png,
                        CancellationToken.None));
                } while (file.Length == 0);
                file.Close();                
                var image  = new BitmapImage(new Uri(fileName));
                
                GraphImage.Source = image;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Close();
            }
            
        }

        private void DrawNode(SyntaxNode node)
        {
            var nodeStatement = CreateNode(node);
            _graph = _graph.Add(nodeStatement);

            foreach (var childNode in node.ChildNodes)
            {
                DrawNode(childNode);

                _graph = _graph.Add(EdgeStatement.For(_idsDict[node], _idsDict[childNode]));
            }
            //Set all children on same level
            if (node.ChildNodes.Count > 2)
                _graph = _graph.Add(new SameRankStatement(node.ChildNodes.ConvertAll(snode => _idsDict[snode])));

            if (node.Sibling == null) return;
            DrawNode(node.Sibling);

            _graph = _graph.Add(EdgeStatement.For(_idsDict[node], _idsDict[node.Sibling]).Set("headport", "w").Set("tailport", "e"));
            _graph = _graph.Add(new SameRankStatement(new List<NodeId> { _idsDict[node], _idsDict[node.Sibling] }));
        }

        private NodeStatement CreateNode(SyntaxNode node)
        {
            var id = new NodeId(_nodeIndex.ToString());
            _idsDict.Add(node, id);
            var n = NodeStatement.For(id.Id).Set("label", node.Label);
            if (node.Shape == NodeShape.Square)
                n =n.Set("shape", "box");
            if(node.ChildNodes.TrueForAll(child => child.Sibling==null)) // If the children has no siblings, make the output edges drawn in order
                n = n.Set("ordering", "out");


            _nodeIndex++;
            return n;
        }
        
    }

    internal class SameRankStatement : Statement
    {
        public List<NodeId> NodeList { get;  }

        public SameRankStatement(List<NodeId > nodeList) : base(ImmutableDictionary<Id, Id>.Empty)
        {
            if(nodeList.Count <2 )
                throw  new InvalidOperationException("There must be two nodes at least");
            NodeList = nodeList;
            
            
        }

        public override void WriteTo(StreamWriter writer, GraphKinds graphKind)
        {
            writer.Write("{rank=same; ");
            foreach (var nodeId in NodeList)
            {
                nodeId.WriteTo(writer);
                writer.Write(" "); //Spaces between the nodes is important
            }
       

            writer.Write(";}");
        }
    }
}