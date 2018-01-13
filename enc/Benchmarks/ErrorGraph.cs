using Encog.Neural.Networks.Training.Propagation;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using System.Windows.Forms;

namespace enc.Benchmarks
{
    public partial class ErrorGraph : Form
    {
        public ErrorGraph(double[,] results, Propagation[] algorithms)
        {
            InitializeComponent();

            var model = new PlotModel { Title = "Błąd uczenia" };

            for (int series = 0; series < results.GetLength(1); series++)
            {
                var lineSeries = new LineSeries()
                {
                    Title = algorithms[series].ToString(),
                };
                for (int sample = 0; sample < results.GetLength(0); sample++)
                {
                    lineSeries.Points.Add(new DataPoint(sample, results[sample, series]));
                }
                model.Series.Add(lineSeries);
            }

            var myPlot = new PlotView
            {
                Model = model,
                Dock = DockStyle.Fill,
            };


            Controls.Add(myPlot);
        }
    }
}
