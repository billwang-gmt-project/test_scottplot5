using System.Windows;

namespace Plots
{
    /// <summary>
    /// Interaction logic for RollingPlotView.xaml
    /// </summary>
    public partial class PlotWindow : Window
    {
        public PlotWindow()
        {
            InitializeComponent();
        }

        //public List<Axis>? AxisList { get; set; } = new List<Axis>();
        //public Axis AddAxis(Edge edge)
        //{
        //    if (AxisList is null)
        //    {
        //        AxisList = new List<Axis>();
        //    }
        //    AxisList.Add(chart.Plot.AddAxis(edge));
        //    return AxisList.Last();
        //}
    }
}
