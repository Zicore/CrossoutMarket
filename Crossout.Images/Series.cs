using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineChart
{
    public class Series
    {
        public List<DataPoint> Items { get; set; } = new List<DataPoint>();

        public double MinX
        {
            get { return Items.Min(x => x.XValue); }
        }

        public double MaxX
        {
            get { return Items.Max(x => x.XValue); }
        }

        public double MinY
        {
            get { return Items.Min(x => x.YValue); }
        }

        public double MaxY
        {
            get { return Items.Max(x => x.YValue); }
        }

        public Pen SeriesPen { get; set; } = new Pen(Brushes.White, 1.5f);
        public Pen FramePen { get; set; } = new Pen(Brushes.White, 1.5f);

        public void Draw(Graphics g, Chart chart)
        {
            float xOffset = chart.Bounds.Width * 0.02f;
            float xOffsetHalf = xOffset * 0.5f;
            float widthSubOffset = chart.Bounds.Width - xOffset;
            float xStep = widthSubOffset / Items.Count;

            float yOffset = chart.Bounds.Height * 0.02f;
            float yStep = (chart.Bounds.Height - yOffset) / (float)MaxY;
            
            List<PointF> points = new List<PointF>();

            GraphicsPath path = new GraphicsPath();

            PointF oldPoint = PointF.Empty;


            foreach (var d in Items)
            {
                int index = Items.IndexOf(d);
                var p = new PointF(xOffsetHalf + xStep * index, chart.Bounds.Height - (float)(yStep * d.YValue));
                points.Add(p);
            }
            
            path.AddLines(points.ToArray());
            g.DrawPath(SeriesPen, path);

            // lowest y value
            float lowerY = yOffset + chart.Bounds.Height - yOffset * 2;
            // outermost right x value
            float rightX = xOffsetHalf + chart.Bounds.Width - xOffset;

            // line from top to bottom on the left
            PointF topToBottomP1 = new PointF(xOffsetHalf, yOffset);
            PointF topToBottomP2 = new PointF(xOffsetHalf, lowerY);
            g.DrawLine(FramePen, topToBottomP1, topToBottomP2);

            // line from right to left on the bottom
            PointF leftToRightP1 = new PointF(xOffsetHalf, lowerY);
            PointF leftToRightP2 = new PointF(rightX, lowerY);
            g.DrawLine(FramePen, leftToRightP1, leftToRightP2);
        }
    }
}
