using CommunityToolkit.Mvvm.ComponentModel;
using ScottPlot.Plottables;
using ScottPlot.WPF;
using System.Windows.Media.Imaging;

namespace Plots
{
    public partial class PlotListModel : ObservableObject
    {
        public static int MaxDataPoints = 100000;
        public PlotPageViewModel viewModel;
        WpfPlot? chart;
        public PlotListModel(PlotPageViewModel viewModel, SignalXY? plot = null)
        {
            this.viewModel = viewModel;
            chart = viewModel.Chart;
            this.plot = plot;

            //var plt = new ScottPlot.Plot();
            //plt.Add(plot);
            //var bitMap = plt.RenderLegend();
            //bitMapImage = PlotPageViewModel.ConvertToBitmap(bitMap);
        }
        [ObservableProperty]
        public BitmapImage? bitMapImage;
        [ObservableProperty]
        public SignalXY? plot = null;
        public double[]? xs = new double[MaxDataPoints];
        public double[]? ys = new double[MaxDataPoints];
        [ObservableProperty]
        public string? name = "";
        [ObservableProperty]
        public string? description = "";
        [ObservableProperty]
        public bool isVisible = true;
        partial void OnIsVisibleChanged(bool value)
        {
            if (Plot is not null)
            {
                if (chart != null)
                {
                    chart.Dispatcher.Invoke(() =>
                    {
                        Plot.IsVisible = value;
                        //chart.Plot.AxisAuto();
                        chart.Refresh();
                    });
                }
            }
        }
    }
}
