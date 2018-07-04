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

namespace Iridium.Geo.Playground
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

        private Random _rnd = new Random();

        Geo.Point randomPoint() => new Geo.Point(_rnd.NextDouble() * 10, _rnd.NextDouble() * 10);

        Geo.BezierCurve randomBezier(int order = -1)
        {
            if (order < 0)
                order = _rnd.Next(5);

            var points = new Geo.Point[order + 1];

            for (int i = 0; i < order + 1; i++)
                points[i] = randomPoint();

            return new Geo.BezierCurve(points);
        }


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

        private UIElement CreateElement(Geo.Polyline polygon, Color color)
        {
            return new System.Windows.Shapes.Polyline()
            {
                Points = new PointCollection(polygon.Points.Select(p => new System.Windows.Point(p.X, p.Y))),
                Stroke = new SolidColorBrush(color)
            };
        }

        private UIElement CreateElement(Geo.Polygon polygon, Color color)
        {
            return new System.Windows.Shapes.Polygon()
            {
                Points = new PointCollection(polygon.Points.Select(p => new System.Windows.Point(p.X, p.Y))),
                Stroke = new SolidColorBrush(color)
            };
        }

        private UIElement CreateElement(Geo.BezierCurve bezier, Color color)
        {
            return new System.Windows.Shapes.Polyline()
            {
                Points = new PointCollection(bezier.GeneratePoints(50).Select(p => new System.Windows.Point(p.X, p.Y))),
                Stroke = new SolidColorBrush(color)
            };
        }

        private UIElement CreateElement(Geo.Point point, Color color)
        {
            var circle = new System.Windows.Shapes.Ellipse()
            {
                Width = 8,
                Height = 8,
                
                Fill= new SolidColorBrush(color),
                StrokeThickness =  0
            };

            Canvas.SetLeft(circle, point.X - 4);
            Canvas.SetTop(circle, point.Y - 4);

            return circle;
        }


        private void UpdateCanvas()
        {
            Canvas.Children.Clear();
            Canvas.Children.Add(CreateElement(_polygon, Colors.Red));
            //Canvas.Children.Add(CreateElement(_box, Colors.Green));

            var bezier = new Geo.BezierCurve(_polygon.Points.ToArray());

            var subBeziers = bezier.Split(0.4);
            var subColors = new[] {Colors.Blue, Colors.Cyan};

            var newPolygon = new Geo.Polyline(bezier.GeneratePoints(50));

            Canvas.Children.Add(CreateElement(newPolygon, Colors.Orange));

            int color = 0;
            foreach (var subBezier in new[]{ subBeziers.Item1,subBeziers.Item2})
            {
                var b  = subBezier.Translate(0, 20);

                Canvas.Children.Add(CreateElement(b, subColors[color++]));

                var intersections = b.Intersections(bezier);

                foreach (var intersection in intersections)
                {
                    Canvas.Children.Add(CreateElement(intersection, Colors.Black));
                }
            }

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
