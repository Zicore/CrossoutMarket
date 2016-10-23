using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
using System.IO;

namespace Crossout.Images
{
    class ChartImageCreator
    {
        public void GenerateMinimalChart(IList<DataPoint> series, Stream outputStream)
        {
            var ch = new Chart();
            ch.Height = 40;
            ch.Width = 100;
            ch.ChartAreas.Add(new ChartArea());
            ch.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            ch.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;
            ch.ChartAreas[0].AxisX.LineColor = Color.White;
            ch.ChartAreas[0].AxisY.LineColor = Color.White;
            ch.ChartAreas[0].AxisX.LabelStyle.Enabled = false;
            ch.ChartAreas[0].AxisY.LabelStyle.Enabled = false;
            ch.ChartAreas[0].BackColor = Color.Transparent;
            ch.ChartAreas[0].AxisX.MajorTickMark.Enabled = false;
            ch.ChartAreas[0].AxisY.MajorTickMark.Enabled = false;
            ch.BackColor = Color.Transparent;
            var s = new Series();
            s.ChartType = SeriesChartType.Line;
            s.Color = Color.White;
            foreach (var pnt in series)
            {
                s.Points.Add(pnt);
            }
            ch.Series.Add(s);
            ch.SaveImage(outputStream, ChartImageFormat.Png);
        }
    }
}
