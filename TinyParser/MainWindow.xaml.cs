using System.Windows;
using System.Windows.Interop;

namespace TinyParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Parse_Click(object sender, RoutedEventArgs e)
        {
            var parser = new Parser(Editor.Text);
            var syntaxNode = parser.Parse();
            var gw = new GraphingWindow();
            gw.Show();
            gw.Draw(syntaxNode);

        }
    }
}
