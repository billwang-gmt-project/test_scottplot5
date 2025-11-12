using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Helper;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WPF;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Xaml;
using test_scottplot5;
using Vanara.Extensions.Reflection;

namespace Plots
{
    public partial class PlotWindowTestViewModel : ObservableObject
    {
        List<double>[] xsList = new List<double>[2] { new(), new() };
        List<double>[] ysList = new List<double>[2] { new(), new() };
        readonly Stopwatch Stopwatch = Stopwatch.StartNew();
        int index = 0;
        DataLogger? logger;
        DataStreamer? dataStreamer;

        public void UpdateValues()
        {
            double x = Stopwatch.Elapsed.TotalMilliseconds;
            double y1 = Math.Sin(2 * Math.PI * 1 * x / 1000);
            double y2 = Math.Sin(2 * Math.PI * 0.5 * x / 1000);
            xsList[0].Add(x);
            ysList[0].Add(y1);
            xsList[1].Add(x);
            ysList[1].Add(y2);
            index++;
        }

        public PlotWindowTestViewModel()
        {
            PlotPageViewModel ??= new();
            PlotPageViewModel.Init(App.plotWindow);
            index = 0;

            for (int i = 0; i < 100; i++)
            {
                UpdateValues();
                Thread.Sleep(1);
            }

            PlotPageViewModel.AddPlot(PlotPageViewModel, xsList[0].ToArray(), ysList[0].ToArray(), "RPM1", "RPM1", ScottPlot.Colors.Red);
            PlotPageViewModel.AddPlot(PlotPageViewModel, xsList[1].ToArray(), ysList[1].ToArray(), "RPM2");


            Task.Run(async () =>
            {
                while (true)
                    for (int i = 0; i < 100; i++)
                    {
                        UpdateValues();
                        PlotPageViewModel.UpdatePlot(PlotPageViewModel, 0, xsList[0].ToArray(), ysList[0].ToArray());
                        PlotPageViewModel.UpdatePlot(PlotPageViewModel, 1, xsList[1].ToArray(), ysList[1].ToArray());
                        await Task.Delay(1);
                    }
            });
        }

        [ObservableProperty]
        PlotPageViewModel? plotPageViewModel;

        [RelayCommand]
        async Task LoadRollingRpm(object? param)
        {
            //if ((App.rollingPlotWindow == null) || (!App.rollingPlotWindow.IsLoaded) || (App.rollingPlotWindowViewModel == null))
            //{
            //    await PlotHelper.LoadRpmToPlotWindow(isInit: true);
            //}
            //else
            //{
            //    await PlotHelper.LoadRpmToPlotWindow();
            //}
        }
    }
}
