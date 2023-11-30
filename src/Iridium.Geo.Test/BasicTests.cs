using System;
using System.Collections.Generic;
using System.Numerics;
using Iridium.Geo;
using MathNet.Numerics.LinearAlgebra;
using NUnit.Framework;

namespace Iridium.Geo.Test
{
    [TestFixture]
    public class BasicTests
    {
        [Test]
        public void Ellipse()
        {
            double rY = 5.0;
            double cord = Math.Sqrt(50.0) * 2;
            double rX = (cord - 10.0) / 2 + 5.0;

            Ellipse ellipse = new Ellipse(Point.Zero, rX, rY);

            Assert.That(ellipse.Cord, Is.EqualTo(cord));
            Assert.That(ellipse.CordSegment.Length, Is.EqualTo(10.0).Within(1).Ulps);

            ellipse = new Ellipse(new Point(-5, 0), new Point(5, 0), cord);

            Assert.That(ellipse.Center.X, Is.EqualTo(0.0).Within(1).Ulps);
            Assert.That(ellipse.Center.Y, Is.EqualTo(0.0).Within(1).Ulps);
            Assert.That(ellipse.RadiusX, Is.EqualTo(rX).Within(1).Ulps);
            Assert.That(ellipse.RadiusY, Is.EqualTo(rY).Within(1).Ulps);
        }


        [Test]
        public void EllipseBoundingBox()
        {
            Ellipse ellipse = new Ellipse(Point.Zero, 10, 5);

            var boundingBox = ellipse.BoundingBox();

            Assert.That(boundingBox.Width, Is.EqualTo(20.0));
            Assert.That(boundingBox.Height, Is.EqualTo(10.0));
            Assert.That(boundingBox.P1.X, Is.EqualTo(-10.0));
        }


        [Test]
        public void LineSegmentAngle()
        {
            LineSegment seg = new LineSegment(new Point(0, 0), new Point(0, 1));

            Assert.That(seg.Angle, Is.EqualTo(Math.PI / 2));
        }


    }

    [TestFixture]
    public class TransformTests
    {
        [Test]
        public void RotateEllipse90()
        {
            Ellipse ellipse = new Ellipse(Point.Zero, 10, 5);

            ellipse = ellipse.Rotate(Math.PI / 2);

            var boundingBox = ellipse.BoundingBox();

            Assert.That(boundingBox.Width, Is.EqualTo(10.0));
            Assert.That(boundingBox.Height, Is.EqualTo(20.0));
            Assert.That(boundingBox.P1.X, Is.EqualTo(-5.0));
        }

        [Test]
        public void RotatePoint()
        {
            Point p = new Point(4, 3);

            Point p2 = new Point(-3, 4);

            var angle = Math.Atan2(4, -3);

            Point p2a = p.Transform(AffineMatrix2D.CreateRotate(Math.PI / 2));
            Point p2b = p.Rotate(Math.PI / 2);
            Point p2c = new Point(Point.Zero, angle, 5);

            Assert.That(p2a.X, Is.EqualTo(p2.X).Within(0.001));
            Assert.That(p2a.Y, Is.EqualTo(p2.Y).Within(0.001));
            Assert.That(p2b.X, Is.EqualTo(p2.X).Within(0.001));
            Assert.That(p2b.Y, Is.EqualTo(p2.Y).Within(0.001));
            Assert.That(p2c.X, Is.EqualTo(p2.X).Within(0.001));
            Assert.That(p2c.Y, Is.EqualTo(p2.Y).Within(0.001));
        }

        [Test]
        public void MirrorAroundPoint()
        {
            Point p = new Point(4, 3);
            Point p2 = new Point(-2, -1);

            Point p2a = p.Transform(AffineMatrix2D.CreateTranslate(-1, -1).Rotate(Math.PI).Translate(1, 1));
            Point p2b = p.Mirror(new Point(1, 1));

            Assert.That(p2b.X, Is.EqualTo(p2.X).Within(0.000001));
            Assert.That(p2b.Y, Is.EqualTo(p2.Y).Within(0.000001));

            Assert.That(p2a.X, Is.EqualTo(p2.X).Within(0.000001));
            Assert.That(p2a.Y, Is.EqualTo(p2.Y).Within(0.000001));
        }

        [Test]
        public void TranslatePoint()
        {
            Point p = new Point(4, 3);
            Point p2 = new Point(6, 4);

            Point p2a = p.Translate(2, 1);
            Point p2b = p.Transform(AffineMatrix2D.CreateTranslate(2, 1));
            Point p2c = p.Transform(AffineMatrix2D.Identity.Translate(2, 1));

            Assert.That(p2a.X, Is.EqualTo(p2.X));
            Assert.That(p2a.Y, Is.EqualTo(p2.Y));
            Assert.That(p2b.X, Is.EqualTo(p2.X));
            Assert.That(p2b.Y, Is.EqualTo(p2.Y));
            Assert.That(p2c.X, Is.EqualTo(p2.X));
            Assert.That(p2c.Y, Is.EqualTo(p2.Y));

        }

        public class MatrixTestCase
        {
            public MatrixTestCase(AffineMatrix2D matrix, Point point, Point expected)
            {
                Matrix = matrix;
                Point = point;
                Expected = expected;
            }

            public AffineMatrix2D Matrix;
            public Point Point;
            public Point Expected;
        }

        public static IEnumerable<MatrixTestCase> MatrixTestCases()
        {
            var p = new Point(1, 2);

            yield return new MatrixTestCase(AffineMatrix2D.Identity, p, p);
            yield return new MatrixTestCase(AffineMatrix2D.CreateScale(2, 3), p, new Point(p.X * 2, p.Y * 3));
            yield return new MatrixTestCase(AffineMatrix2D.CreateScale(2, 3, new Point(5,7)), p, new Point(5-8, 7-15));
            yield return new MatrixTestCase(AffineMatrix2D.MirrorX, p, new Point(1, -2));
            yield return new MatrixTestCase(AffineMatrix2D.MirrorY, p, new Point(-1, 2));
            yield return new MatrixTestCase(AffineMatrix2D.CreateMirror(new Point(5, 7)), p, new Point(9, 12));
            yield return new MatrixTestCase(AffineMatrix2D.CreateMirror(new LineSegment(new Point(1, 4), new Point(3, 2))), p, new Point(3, 4));
            yield return new MatrixTestCase(AffineMatrix2D.CreateRotate(Math.PI / 2), p, new Point(-2, 1));
            yield return new MatrixTestCase(AffineMatrix2D.CreateRotate(Math.PI / 2, new Point(3,1)), p, new Point(2, -1));
        }

        [Test]
        [TestCaseSource(nameof(MatrixTestCases))]
        public void TestMatrixTransformations(MatrixTestCase testCase)
        {
            var result = testCase.Point.Transform(testCase.Matrix);

            Assert.That(result.X, Is.EqualTo(testCase.Expected.X).Within(0.0001), "X");
            Assert.That(result.Y, Is.EqualTo(testCase.Expected.Y).Within(0.0001), "Y");

            TestRectangleTransform(testCase.Matrix);
            TestPolygonTransform(testCase.Matrix);
            TestEllipseTransform(testCase.Matrix);
        }

        private void TestRectangleTransform(AffineMatrix2D matrix)
        {
            Rectangle r = new Rectangle(RandomPoint(), RandomPoint());

            Polygon p = r.Transform(matrix);

            ComparePoints(p.Points[0], r.P1.Transform(matrix));
            ComparePoints(p.Points[2], r.P2.Transform(matrix));
        }

        private void TestPolygonTransform(AffineMatrix2D matrix)
        {
            Polygon orig = new Polygon(new[] {RandomPoint(), RandomPoint(), RandomPoint(), RandomPoint()});
            Polygon transformed = orig.Transform(matrix);

            ComparePoints(transformed.Points[0], orig.Points[0].Transform(matrix));
            ComparePoints(transformed.Points[1], orig.Points[1].Transform(matrix));
            ComparePoints(transformed.Points[2], orig.Points[2].Transform(matrix));
            ComparePoints(transformed.Points[3], orig.Points[3].Transform(matrix));
        }

        private void TestEllipseTransform(AffineMatrix2D matrix)
        {
            var (p1, p2) = (RandomPoint(), RandomPoint());

            Ellipse orig = new Ellipse(p1, p2, p1.DistanceTo(p2) * 1.5);
            Ellipse transformed = orig.Transform(matrix);

            ComparePoints(orig.PointAt(1).Transform(matrix), transformed.PointAt(1));
        }

        private static void ComparePoints(Point point, Point expected)
        {
            if (Math.Abs(point.X-expected.X) < 0.0001 && Math.Abs(point.Y-expected.Y) < 0.0001)
                return;

            Assert.Fail($"Expected: ({expected.X},{expected.Y}), Actual: ({point.X},{point.Y})");
        }

        private static Random _rnd = new Random();
        private static Point RandomPoint() => new Point(_rnd.NextDouble() * 100, _rnd.NextDouble() * 100);
    }

    [TestFixture]
    public class MatrixTests
    {
        private (Matrix<double>,AffineMatrix2D) CreateTestMatrix()
        {
            var matrix1 = MathNet.Numerics.LinearAlgebra.Double.Matrix.Build.Dense(3, 3, new double[]
            {
                1,2,3,
                4,5,6,
                0,0,1
            }).Transpose();

            var matrix2 = new AffineMatrix2D(1, 2, 4, 5, 3, 6);

            Assert.That(matrix2.xx, Is.EqualTo(matrix1[0, 0]).Within(0.001), "xx");
            Assert.That(matrix2.xy, Is.EqualTo(matrix1[0, 1]).Within(0.001), "xy");
            Assert.That(matrix2.tx, Is.EqualTo(matrix1[0, 2]).Within(0.001), "tx");
            Assert.That(matrix2.yx, Is.EqualTo(matrix1[1, 0]).Within(0.001), "yx");
            Assert.That(matrix2.yy, Is.EqualTo(matrix1[1, 1]).Within(0.001), "yy");
            Assert.That(matrix2.ty, Is.EqualTo(matrix1[1, 2]).Within(0.001), "ty");

            return (matrix1, matrix2);
        }

        private void CompareMatrix(Matrix<double> matrix1, AffineMatrix2D matrix2)
        {
            Assert.That(matrix2.xx, Is.EqualTo(matrix1[0, 0]).Within(0.001), "xx");
            Assert.That(matrix2.xy, Is.EqualTo(matrix1[0, 1]).Within(0.001), "xy");
            Assert.That(matrix2.tx, Is.EqualTo(matrix1[0, 2]).Within(0.001), "tx");
            Assert.That(matrix2.yx, Is.EqualTo(matrix1[1, 0]).Within(0.001), "yx");
            Assert.That(matrix2.yy, Is.EqualTo(matrix1[1, 1]).Within(0.001), "yy");
            Assert.That(matrix2.ty, Is.EqualTo(matrix1[1, 2]).Within(0.001), "ty");
        }

        [Test]
        public void TestDeterminant()
        {
            var (matrix1,matrix2) = CreateTestMatrix();

            var det1 = matrix1.Determinant();
            var det2 = matrix2.Determinant();

            Assert.That(det1, Is.EqualTo(det2));
        }

        [Test]
        public void TestInvert()
        {
            var (matrix1, matrix2) = CreateTestMatrix();

            var inv1 = matrix1.Inverse();
            var inv2 = matrix2.Invert();

            CompareMatrix(inv1, inv2);

        }

    }
}