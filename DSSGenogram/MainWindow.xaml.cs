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
using DSSGenogram.Utils;
using GraphX.PCL.Common.Enums;
using GraphX.PCL.Logic.Algorithms.LayoutAlgorithms;

namespace DSSGenogram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private VertexControl _ecFrom;
        private readonly EditorObjectManager _editorManager;

        public MainWindow()
        {
            InitializeComponent();

            _editorManager = new EditorObjectManager(gg_Area, gg_zoomctrl);

            gg_Area.VertexSelected += Gg_Area_VertexSelected;
            gg_Area.DragEnter += gg_Area_DragEnter;
            gg_Area.VertexMouseUp += Gg_Area_VertexMouseUp;

            //gg_Area.GetAllVertexControls //+= new DragEventHandler(all_Vertex_DragEnter);

            gg_zoomctrl.MouseDown += Gg_zoomctrl_MouseDown;

            btnCreate.Click += BtnCreate_Click;
            btnLayout.Click += BtnLayout_Click;

            Loaded += gg_Loaded;
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            var setVBData = new Views.GetVertexData();
            setVBData.ShowDialog();

            VertexControl vc = new VertexControl(setVBData.DataVertex);

            gg_Area.AddVertexAndData(setVBData.DataVertex, vc);

            gg_Area.RelayoutGraph();
            gg_zoomctrl.ZoomToFill();
        }

        private void BtnLayout_Click(object sender, RoutedEventArgs e)
        {
            gg_Area.LogicCore.DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.EfficientSugiyama;
            var prms = gg_Area.LogicCore.AlgorithmFactory.CreateLayoutParameters(LayoutAlgorithmTypeEnum.EfficientSugiyama) as EfficientSugiyamaLayoutParameters;
            prms.EdgeRouting = SugiyamaEdgeRoutings.Orthogonal;
            prms.LayerDistance = prms.VertexDistance = 100;
            gg_Area.LogicCore.EdgeCurvingEnabled = false;
            gg_Area.LogicCore.DefaultLayoutAlgorithmParams = prms;

            gg_Area.RelayoutGraph();
            gg_zoomctrl.ZoomToFill();
        }

        private void Gg_zoomctrl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //create vertices and edges only in Edit mode
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var pos = gg_zoomctrl.TranslatePoint(e.GetPosition(gg_zoomctrl), gg_Area);
                pos.Offset(-22.5, -22.5);
                var vc = CreateVertexControl(pos);
                if (_ecFrom != null)
                    CreateEdgeControl(vc);
            }

        }

        private void Gg_Area_VertexSelected(object sender, VertexSelectedEventArgs args)
        {
            CreateEdgeControl(args.VertexControl);
        }

        private void Gg_Area_VertexMouseUp(object sender, VertexSelectedEventArgs args)
        {
            return;
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


                foreach (var item in graph.Vertices)
                    ThemedDataStorage.FillDataVertex(item);

                gg_Area.GenerateGraph(graph);
                gg_zoomctrl.ZoomToFill();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private VertexControl CreateVertexControl(Point position)
        {
            var data = new DataVertex("Vertex " + (gg_Area.VertexList.Count + 1)) { ImageId = ShowcaseHelper.Rand.Next(0, ThemedDataStorage.EditorImages.Count) };
            var vc = new VertexControl(data);
            vc.SetPosition(position);
            gg_Area.AddVertexAndData(data, vc, true);
            return vc;
        }


        private void CreateEdgeControl(VertexControl vc)
        {
            try
            {
                if (_ecFrom == null)
                {
                    _editorManager.CreateVirtualEdge(vc, vc.GetPosition());
                    _ecFrom = vc;
                    HighlightBehaviour.SetHighlighted(_ecFrom, true);
                    return;
                }
                if (_ecFrom == vc) return;

                var data = new DataEdge((DataVertex)_ecFrom.Vertex, (DataVertex)vc.Vertex);
                var ec = new EdgeControl(_ecFrom, vc, data);
                gg_Area.InsertEdgeAndData(data, ec, 0, true);

                HighlightBehaviour.SetHighlighted(_ecFrom, false);
                _ecFrom = null;
                _editorManager.DestroyVirtualEdge();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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

    public class EdgeBlueprint : IDisposable
    {
        public VertexControl Source { get; set; }
        public Point TargetPos { get; set; }
        public Path EdgePath { get; set; }

        public EdgeBlueprint(VertexControl source, Point targetPos, Brush brush)
        {
            EdgePath = new Path() { Stroke = brush, Data = new LineGeometry() };
            Source = source;
            Source.PositionChanged += Source_PositionChanged;
        }

        void Source_PositionChanged(object sender, VertexPositionEventArgs args)
        {
            UpdateGeometry(Source.GetCenterPosition(), TargetPos);
        }

        internal void UpdateTargetPosition(Point point)
        {
            TargetPos = point;
            UpdateGeometry(Source.GetCenterPosition(), point);
        }

        private void UpdateGeometry(Point start, Point end)
        {
            EdgePath.Data = new LineGeometry(start, end);
            (EdgePath.Data as LineGeometry).Freeze();
        }

        public void Dispose()
        {
            Source.PositionChanged -= Source_PositionChanged;
            Source = null;
        }
    }
}
