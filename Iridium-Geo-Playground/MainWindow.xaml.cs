using System;
using System.Collections.Generic;
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

using Geo = Iridium.Geo;

namespace Iridium_Geo_Playground
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private Geo.Polygon _polygon;
        private Geo.Polygon _box;

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            _polygon = new Geo.Polygon(new[]
            {
                new Geo.Point(10, 10),
                new Geo.Point(30, 20),
                new Geo.Point(35, 40),
                new Geo.Point(20, 20),
                new Geo.Point(5, 20),
            });

            _polygon = _polygon.Scale(5.0);

            _box = _polygon.BoundingBox();
            
            UpdateCanvas();
        }

        private UIElement CreateElement(Geo.Polygon polygon, Color color)
        {
            return new Polygon()
            {
                Points = new PointCollection(polygon.Points.Select(p => new Point(p.X, p.Y))),
                Stroke = new SolidColorBrush(color)
            };
        }

        private void UpdateCanvas()
        {
            Canvas.Children.Clear();
            Canvas.Children.Add(CreateElement(_polygon, Colors.Red));
            Canvas.Children.Add(CreateElement(_box, Colors.Green));

            var bezier = new Geo.BezierCurve(_polygon.Points.ToArray());

            var newPolygon = new Geo.Polygon(bezier.GeneratePoints(50));

            Canvas.Children.Add(CreateElement(newPolygon, Colors.Orange));

        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pt = e.GetPosition(Canvas);

            _polygon = new Geo.Polygon(_polygon.Points.Skip(1).Union(new[] { new Geo.Point(pt.X, pt.Y), }));
            _box = _polygon.BoundingBox();

            UpdateCanvas();
        }
    }


    

}
