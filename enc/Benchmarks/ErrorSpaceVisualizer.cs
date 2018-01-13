using enc.Utils;
using Encog.ML.Data;
using Encog.Neural.Networks;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace enc.Benchmarks
{
    public partial class ErrorSpaceVisualizer : Form
    {
        public ErrorSpaceVisualizer(Point<double>[,] results, BasicNetwork network, IMLDataSet dataSet, Object[] seriesLabels)
        {
            InitializeComponent();

            var model = new PlotModel();
            double ratio = (double)this.Width / this.Height;
            double x0 = -4*ratio;
            double x1 = 4*ratio;
            double y0 = -4;
            double y1 = 4;

            //generate values
            var drawingNetwork = (BasicNetwork)network.Clone();
            Func<double, double, double> peaks = (x, y) =>
                {
                    drawingNetwork.Flat.Weights[0] = x;
                    drawingNetwork.Flat.Weights[1] = y;
                    return drawingNetwork.CalculateError(dataSet);
                };
            var xx = ArrayBuilder.CreateVector(x0, x1, 100);
            var yy = ArrayBuilder.CreateVector(y0, y1, 100);
            var peaksData = ArrayBuilder.Evaluate(peaks, xx, yy);

            var contourSeries = new ContourSeries
            {
                Color = OxyColors.Gray,
                LabelBackground = OxyColors.WhiteSmoke,
                Background = OxyColors.White,
                ColumnCoordinates = xx,
                RowCoordinates = yy,
                Data = peaksData,
            };
            model.Series.Add(contourSeries);


            for (int series = 0; series < results.GetLength(1); series++)
            {
                var lineSeries = new LineSeries();
                lineSeries.Title = seriesLabels[series].GetType().Name;
                for (int pt = 0; pt < results.GetLength(0); pt++)
                {
                    var x = results[pt, series].X;
                    var y = results[pt, series].Y;
                    lineSeries.Points.Add(new DataPoint(x, y));
                }

                lineSeries.LineStyle = LineStyle.Solid;
                lineSeries.StrokeThickness = 1;
                lineSeries.MarkerStrokeThickness = 1.5;
                lineSeries.MarkerType = MarkerType.Circle;
                model.Series.Add(lineSeries);
            }


            var myPlot = new PlotView
            {
                Model = model,
                Dock = DockStyle.Fill,
                BackColor = Color.White,
            };


            Controls.Add(myPlot);
        }
    }
}
