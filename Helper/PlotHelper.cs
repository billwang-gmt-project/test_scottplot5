using Microsoft.Win32;
using Plots;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Drawing.Color;
using Console = Colorful.Console;

namespace Helper
{
    public class PlotHelper
    {
        //public static void PopupRollingPlotWindow(bool init = false)
        //{
        //    if (init)
        //    {
        //        App.rollingPlotWindow = null;
        //        App.rollingPlotWindowViewModel = null;
        //    }

        //    /// Window is not loaded, create new window
        //    if ((App.rollingPlotWindow != null) && (!App.rollingPlotWindow.IsLoaded))
        //    {
        //        App.rollingPlotWindow = new();
        //    }

        //    // init plot window
        //    App.rollingPlotWindow ??= new();
        //    WindowHelper.PopupWindow(App.rollingPlotWindow, "Rolling Plot Window");

        //    // init plot view model 
        //    if (App.rollingPlotWindowViewModel == null)
        //    {
        //        App.rollingPlotWindowViewModel = new();
        //    }
        //    App.rollingPlotWindowViewModel.PlotPageViewModel ??= new();
        //    App.rollingPlotWindowViewModel.PlotPageViewModel.Chart ??= new();
        //    App.rollingPlotWindow.DataContext = App.rollingPlotWindowViewModel;

        //    //if ((App.rollingPlotWindow != null) && (!App.rollingPlotWindow.IsLoaded))
        //    //{
        //    //    App.rollingPlotWindow = new();
        //    //    InitRollingPlotPage(App.rollingPlotWindow, App.rollingPlotWindowViewModel!.PlotPageViewModel);
        //    //}
        //}
        //public static async Task<(string?, RpmQueueModel?)> ReadRpmFileAsync(string? path = null)
        //{
        //    string? filePath = path;

        //    if (filePath == null)
        //    {
        //        // Open a file dialog with .csv extension and then load the selected file
        //        OpenFileDialog openFileDialog = new OpenFileDialog();
        //        openFileDialog.Filter = "CSV Files (*.csv)|*.csv";
        //        if (openFileDialog.ShowDialog() == true)
        //        {
        //            filePath = openFileDialog.FileName;
        //        }
        //        else
        //        {
        //            return (null, null);
        //        }
        //    }

        //    // Load and Remove header line
        //    string[] lines = await File.ReadAllLinesAsync(filePath);
        //    string[] newLines = new string[lines.Length - 1];
        //    Array.Copy(lines, 1, newLines, 0, lines.Length - 1);
        //    lines = newLines;
        //    LogHelper.AddInfoLog($"LoadRollingRpmFile: {lines.Length}", System.Drawing.Color.Yellow);

        //    RpmQueueModel plotQueue = new() { time = new(), rpm = new(), current = new(), voltage = new() };
        //    foreach (string line in lines)
        //    {
        //        string[] items = line.Split(',');
        //        if (items.Length == 4)
        //        {
        //            plotQueue.time.Enqueue(decimal.Parse(items[0]));
        //            plotQueue.rpm.Enqueue(decimal.Parse(items[1]));
        //            plotQueue.current.Enqueue(decimal.Parse(items[2]));
        //            plotQueue.voltage.Enqueue(decimal.Parse(items[3]));
        //        }
        //    }
        //    return (filePath, plotQueue);
        //}
        //public static async Task LoadRpmToPlotWindow(string? path = null, bool isInit = false)
        //{
        //    // Read file
        //    (string? filePath, RpmQueueModel? plotQueue) = await ReadRpmFileAsync(path);
        //    if ((plotQueue == null) || (filePath == null))
        //    {
        //        Console.WriteLine("No RPM Data", Color.Red);
        //        return;
        //    }

        //    // Show rolling plot window
        //    PopupRollingPlotWindow();

        //    if (isInit)
        //    {
        //        InitRollingPlotPage(App.rollingPlotWindow, App.rollingPlotWindowViewModel!.PlotPageViewModel);
        //    }


        //    App.rollingPlotWindowViewModel!.PlotPageViewModel!.IsLimitDisplaySize = false;

        //    // Add Plot
        //    if (App.rollingPlotWindowViewModel.PlotPageViewModel.PlotList!.Count == 0)
        //    {
        //        App.rollingPlotWindowViewModel!.PlotPageViewModel!.AddPlot(Path.GetFileName(filePath), Path.GetFileName(filePath), Color.Red);
        //    }
        //    else
        //    {
        //        App.rollingPlotWindowViewModel!.PlotPageViewModel!.AddPlot(Path.GetFileName(filePath), Path.GetFileName(filePath));
        //    }

        //    // Update plot data
        //    int index = App.rollingPlotWindowViewModel!.PlotPageViewModel!.PlotList!.Count - 1;
        //    UpdateRpmPlot(App.rollingPlotWindow, App.rollingPlotWindowViewModel!.PlotPageViewModel, plotQueue, index);

        //    //App.rollingPlotWindowViewModel!.PlotPageViewModel.InitCrossLineCursor();
        //}
        //public static bool LoadSingleRpmPlot(string? path = null)
        //{
        //    string? filePath = path;

        //    if (filePath == null)
        //    {
        //        // Open a file dialog with .csv extension and then load the selected file
        //        OpenFileDialog openFileDialog = new OpenFileDialog();
        //        openFileDialog.Filter = "CSV Files (*.csv)|*.csv";
        //        if (openFileDialog.ShowDialog() == true)
        //        {
        //            filePath = openFileDialog.FileName;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }

        //    PopupRollingPlotWindow(true);


        //    // check null
        //    if (App.rollingPlotWindowViewModel == null)
        //    {
        //        LogHelper.AddInfoLog("rollingPlotViewModel is null", System.Drawing.Color.Red);
        //        return false;
        //    }
        //    if (App.rollingPlotWindowViewModel.PlotPageViewModel == null)
        //    {
        //        LogHelper.AddInfoLog("rollingPlotViewModel.PlotViewModel is null", System.Drawing.Color.Red);
        //        return false;
        //    }
        //    if (App.rollingPlotWindowViewModel.PlotPageViewModel.Chart == null)
        //    {
        //        LogHelper.AddInfoLog("rollingPlotViewModel.PlotViewModel.Chart is null", System.Drawing.Color.Red);
        //        return false;
        //    }
        //    App.rollingPlotWindowViewModel.PlotPageViewModel.ClearPlots();
        //    // Load and Remove header line
        //    string[] lines = File.ReadAllLines(filePath);
        //    string[] newLines = new string[lines.Length - 1];
        //    Array.Copy(lines, 1, newLines, 0, lines.Length - 1);
        //    lines = newLines;
        //    LogHelper.AddInfoLog($"LoadRollingRpmFile: {lines.Length}", System.Drawing.Color.Yellow);

        //    // Add data to plot
        //    //++ Add Plot
        //    App.rollingPlotWindowViewModel.PlotPageViewModel.AddPlot(Path.GetFileName(filePath), Path.GetFileName(filePath), Color.Red);
        //    int index = App.rollingPlotWindowViewModel.PlotPageViewModel.PlotList!.Count - 1;
        //    RpmQueueModel plotQueue = new() { time = new(), rpm = new(), current = new(), voltage = new() };
        //    foreach (string line in lines)
        //    {
        //        string[] items = line.Split(',');
        //        if (items.Length == 4)
        //        {
        //            plotQueue.time.Enqueue(decimal.Parse(items[0]));
        //            plotQueue.rpm.Enqueue(decimal.Parse(items[1]));
        //            plotQueue.current.Enqueue(decimal.Parse(items[2]));
        //            plotQueue.voltage.Enqueue(decimal.Parse(items[3]));
        //        }
        //    }
        //    UpdateRpmPlot(App.rollingPlotWindow, App.rollingPlotWindowViewModel.PlotPageViewModel, plotQueue, index);
        //    return true;
        //}
        //public static void UpdateRpmPlot(Window? window, PlotPageViewModel? plotPageViewModel, RpmQueueModel? plotQueue, int index = 0)
        //{
        //    if (plotQueue == null)
        //        return;

        //    if (window == null)
        //        return;

        //    if (plotPageViewModel == null)
        //        return;

        //    //await semaphorePlot.WaitAsync();
        //    RpmQueueModel PlotQueueClone = (RpmQueueModel)plotQueue.Clone();
        //    //semaphorePlot.Release();

        //    if (PlotQueueClone!.time!.IsEmpty)
        //    {
        //        return;
        //    }

        //    if ((PlotQueueClone!.time.Count != PlotQueueClone.rpm!.Count) && (PlotQueueClone!.time.Count != PlotQueueClone.current!.Count))
        //        return;

        //    UIHelper.InvokeOnUIThread(window!, () =>
        //    {
        //        plotPageViewModel.UpdateXy(index, PlotQueueClone.time.Select(x => (double)x).ToArray(), PlotQueueClone!.rpm!.Select(x => (double)x).ToArray());

        //    });
        //}
        //public static void InitRollingPlotPage(Window? window, PlotPageViewModel? PlotPageViewModel)
        //{
        //    if (window == null)
        //    {
        //        LogHelper.AddInfoLog("Window is null", Color.Red);
        //        return;
        //    }

        //    if (PlotPageViewModel is null)
        //    {
        //        LogHelper.AddInfoLog("PlotViewModel is null", System.Drawing.Color.Red);
        //        return;
        //    }

        //    if (PlotPageViewModel.Chart == null)
        //    {
        //        LogHelper.AddInfoLog("PlotViewModel.Chart is null", System.Drawing.Color.Red);
        //        return;
        //    }

        //    // Clear chart
        //    PlotPageViewModel.Chart.Plot.Clear();

        //    //++ Link window and plotPageViewmodel    
        //    PlotPageViewModel.Init(window, PlotPageViewModel.Chart);

        //    PlotPageViewModel.IsUpdatePlot = true;
        //    PlotPageViewModel.IsShowCursor = false;
        //    PlotPageViewModel.IsLimitDisplaySize = true;

        //    //+ Set Plot Axis
        //    PlotPageViewModel.Chart.Plot.BottomAxis.Label("Time(S)");
        //    PlotPageViewModel.Chart.Plot.YAxis.Label("RPM1");
        //    PlotPageViewModel.Chart.Plot.YAxis.Color(Color.Red);


        //    PlotPageViewModel.Chart.Refresh();
        //}
    }
}
