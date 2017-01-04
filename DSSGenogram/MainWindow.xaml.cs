using System;
using System.Windows;
using DSSGenogram.Models;
using GraphX.Controls.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GraphX.Controls;

namespace DSSGenogram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            gg_Area.VertexClicked += gg_Area_VertexClicked;
            gg_Area.DragEnter += gg_Area_DragEnter;
            gg_Area.VertexMouseUp += Gg_Area_VertexMouseUp;

            //gg_Area.GetAllVertexControls //+= new DragEventHandler(all_Vertex_DragEnter);

            Loaded += gg_Loaded;
        }

        private void Gg_Area_VertexMouseUp(object sender, VertexSelectedEventArgs args)
        {
            throw new NotImplementedException();
        }
        
        private void gg_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                gg_Area.ClearLayout();

                var graph = new GraphField();

                graph.AddVertex(new DataVertex() { ID = 1 });
                graph.AddVertex(new DataVertex() { ID = 2 });
                graph.AddVertex(new DataVertex() { ID = 3 });
                                                                                
                var gglogic = new LogicCoreExample();
                gg_Area.LogicCore = gglogic;

                gg_Area.GenerateGraph(graph);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void gg_Area_VertexClicked(object sender, VertexClickedEventArgs args)
        {
            if (args.Modifiers == ModifierKeys.Control )
            {

            }

            args.MouseArgs.Handled = true;
        }

        private void gg_Area_DragEnter(object sender, DragEventArgs e)
        {
            if (e.KeyStates.Equals(ModifierKeys.Control))
            {

            }
            e.Handled = true;
        }

        private void all_Vertex_DragEnter(object sender, DragEventArgs e)
        {

        }
    }
}
