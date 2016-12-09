using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Shields.GraphViz.Components;
using Shields.GraphViz.Models;
using Shields.GraphViz.Services;
using Path = System.IO.Path;

namespace TinyParser
{
    /// <summary>
    /// Interaction logic for GraphingWindow.xaml
    /// </summary>
    public partial class GraphingWindow : Window
    {
        Graph _graph ;
        static int nodeIndex = 0;
        Dictionary<SyntaxNode, NodeId> idsDict = new Dictionary<SyntaxNode, NodeId>();

        public GraphingWindow()
        {
            InitializeComponent();
        }

        private string GetGraphvizBinaryPath()
        {
            string cmd = "dot";
            Process proc = new Process();
            proc.StartInfo.FileName = "where.exe";
            proc.StartInfo.Arguments = cmd;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.Start();
            proc.WaitForExit();

            if (proc.ExitCode == 0)
            {
                string output = proc.StandardOutput.ReadToEnd();
                output = Path.GetDirectoryName(output.Substring(0, output.Length - 2));
                proc.Close();
                return output;

            }
            else
            {
                proc.Close();
                throw new Exception("Graphviz is not installed on your system or Its bin folder is  not registered in PATH");
            }

        }

        
        public async void Draw(SyntaxNode node)
        {
            try
            {
                idsDict.Clear();
                nodeIndex = 0;

                _graph = Graph.Undirected.Add(AttributeStatement.Graph.Set("splines", "true"));

                DrawNode(node);
                IRenderer renderer = new Renderer(GetGraphvizBinaryPath());
                string fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".png";

                using (Stream file = File.Create(fileName))
                {
                    await renderer.RunAsync(
                        _graph, file,
                        RendererLayouts.Dot,
                        RendererFormats.Png,
                        CancellationToken.None);
                }
                graphImage.Source = new BitmapImage(new Uri(fileName));

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                this.Close();
                ;
            }
            
        }

        private void DrawNode(SyntaxNode node)
        {
            var NodeStatement = CreateNode(node);
            _graph = _graph.Add(NodeStatement);
            foreach (var childNode in node.ChildNodes)
            {
                DrawNode(childNode);

                _graph = _graph.Add(EdgeStatement.For(idsDict[node], idsDict[childNode]));
            }
            //Set all children on same level
            if (node.ChildNodes.Count > 2)
                _graph = _graph.Add(new SameRankStatement(node.ChildNodes.ConvertAll(snode => idsDict[snode])));

            if (node.Sibling != null)
            {
                DrawNode(node.Sibling);
                
                _graph = _graph.Add(EdgeStatement.For(idsDict[node], idsDict[node.Sibling]).Set("headport","w").Set("tailport","e"));
                _graph = _graph.Add(new SameRankStatement(new List<NodeId>() { idsDict[node], idsDict[node.Sibling] }));

            }
        }
        
        private NodeStatement CreateNode(SyntaxNode node)
        {
            var id = new NodeId(nodeIndex.ToString());
            idsDict.Add(node, id);
            NodeStatement n;
            if (node.Shape == NodeShape.Square)
                n = NodeStatement.For(id.Id).Set("label", node.Label).Set("shape", "box");
            else
                n = NodeStatement.For(id.Id).Set("label", node.Label);

            nodeIndex++;
            return n;
        }
        
    }

     class SameRankStatement : Statement
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