using ScriptCs;
using ScriptCs.Contracts;
using ScriptCs.Hosting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Napkin.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public ScriptServices Root { get; private set; }

        private string napkinDocument = "napkinDoc.txt";
        private string napkinTransformScript = "napkinTransform.csx";

        public MainWindow()
        {
            InitializeComponent();

            if (File.Exists(napkinDocument))
                textBox.Text = File.ReadAllText(napkinDocument);
            else
            {
                textBox.Text = @"Item Id=1
    Name=Foo
    Description=Fuu

Item Id=2
    Name=Bar

    Item Id=201
        Name=Baz

Item
    Id=3
    Name=Bax";
            }

            if (File.Exists(napkinTransformScript))
                textBox2.Text = File.ReadAllText(napkinTransformScript);
            else
            {
                textBox2.Text = @"var node = Require<NapkinPack>().Node();
node.Children[2].Properties[""Name""]";
            }

            textBox.TextChanged += TextBox_TextChanged;
            textBox2.TextChanged += TextBox2_TextChanged;

            Execute();

        }

        private bool isExecuting;
        private bool queuedExecution;

        private void saveDocuments()
        {
            System.IO.File.WriteAllText(napkinDocument, textBox.Text);
            System.IO.File.WriteAllText(napkinTransformScript, textBox2.Text);
        }

        private void Execute()
        {
            if (isExecuting)
            {
                queuedExecution = true;
                return;
            }
            isExecuting = true;

            // save
            saveDocuments();

            var host = new ScriptCsHost();
            host.Root.Executor.Initialize(new[] { "System", "System.Linq" }, new[] { new NapkinSyntaxScriptPack(textBox.Text) });
            host.Root.Executor.AddReferenceAndImportNamespaces(new[] { typeof(IScriptExecutor), typeof(Napkin.Node), typeof(Napkin.Wpf.NapkinPack) });

            var result = host.Root.Executor.ExecuteScript(textBox2.Text, new string[0]);

            if (result.ReturnValue != null)
                textBox1.Text = result.ReturnValue.ToString();
            else
            {
                if (result.CompileExceptionInfo != null) textBox1.Text = result.CompileExceptionInfo.SourceException.ToString();
                if (result.ExecuteExceptionInfo != null) textBox1.Text = result.ExecuteExceptionInfo.SourceException.ToString();
            }

            host.Root.Executor.Terminate();

            isExecuting = false;
            if (queuedExecution)
            {
                queuedExecution = false;
                Execute();
            }

        }

        private void TextBox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            Execute();
        }

        static string Render(string input)
        {
            return new Napkin.Html.Haml().Render(input);
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {

                //textBox2.Text = Newtonsoft.Json.JsonConvert.SerializeObject(nodes,Newtonsoft.Json.Formatting.Indented);

                textBox1.Text = Render(textBox.Text);

            }
            catch (Exception ex)
            {
                textBox1.Text = ex.ToString();

            }

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Execute();
        }






        public TreeViewModel TreeModel
        {
            get
            {
                return new TreeViewModel
                {
                    Items = new ObservableCollection<NodeViewModel>{
                           new NodeViewModel { Name = "Root", Children =  new ObservableCollection<NodeViewModel> {
                              new NodeViewModel { Name = "Level1" ,  Children = new ObservableCollection<NodeViewModel>{
                                  new NodeViewModel{ Name = "Level2"}}} } }}
                };
            }
        }


    }
    public class TreeViewModel
    {
        public ObservableCollection<NodeViewModel> Items { get; set; }
    }

    public class NodeViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ObservableCollection<NodeViewModel> Children { get; set; }
    }

}
