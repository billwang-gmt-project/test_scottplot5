using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Helper;
using OpenTK.Graphics.ES11;



//using Functions;
using ScottPlot;
using ScottPlot.Plottable;
using ScottPlot.Plottables;
using ScottPlot.WPF;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Reflection.Metadata.BlobBuilder;
using Application = System.Windows.Application;
using Console = Colorful.Console;
using Point = System.Windows.Point;
using Text = ScottPlot.Plottables.Text;


namespace Plots
{
    public partial class PlotPageViewModel : ObservableObject
    {
        public delegate void LogDelegate(string message);
        public LogDelegate? LogMessage;

        #region MVVM
        [ObservableProperty]
        public WpfPlot? chart;
        [ObservableProperty]
        public ObservableCollection<PlotListModel>? plotList; // Must use ObservableCollection when binding to item control to dynamic add/remove update
        [ObservableProperty]
        public int plotIndex = -1;
        partial void OnPlotIndexChanged(int value)
        {
            if (Chart == null)
                return;
            if (PlotList == null)
                return;
            if (PlotList.Count == 0)
                return;
            if (PlotList[PlotIndex].plot == null)
                return;
            // determine point nearest the cursor
            CurrentPlot = PlotList[PlotIndex].plot;
            RefreshChart();
        }
        [ObservableProperty]
        public bool isShowCursor = false;
        partial void OnIsShowCursorChanged(bool value)
        {
            if (Chart == null)
                return;
            if (cursorMarker == null)
                return;
            if (PlotList == null)
                return;
            if (PlotList.Count == 0)
                return;
            if (CurrentPlot == null)
                return;

            if (!CurrentPlot.IsVisible)
            {
                return;
            }

            cursorMarker.IsVisible = value;
            cursorText.IsVisible = value;

            ShowCrossLineCursor(value);

            RefreshChart("OnIsShowCursorChanged");
        }
        [ObservableProperty]
        public bool isShowLegend = false;
        partial void OnIsShowLegendChanged(bool value)
        {
            if (Chart == null) return;
            Chart.Plot.Legend.IsVisible = value;
            RefreshChart("OnIsShowLegendChanged");
        }
        [ObservableProperty]
        bool isLimitDisplaySize = true;
        [ObservableProperty]
        int displayLength = 0;
        [ObservableProperty]
        public bool isUpdatePlot = true;
        partial void OnIsUpdatePlotChanged(bool value)
        {
            //if (value == true)
            //{
            //    Chart.Interaction.Enable();
            //}
            //else
            //{
            //    Chart.Interaction.Enable();
            //}
        }
        [ObservableProperty]
        string statusString = "";
        [ObservableProperty]
        string cursorString = "";

        [ObservableProperty]
        public bool isYaxisAuto = true;
        partial void OnIsYaxisAutoChanged(bool value)
        {
            if (Chart == null)
                return;

            if (value)
            {
                Chart.Plot.Axes.AutoScaleY();
            }
            else
            {
                Chart.Plot.Axes.SetLimitsY(YaxisMin, YaxisMax);
            }
            RefreshChart("OnIsYaxisAutoChanged");
        }
        [ObservableProperty]
        public double yaxisMin = 0;
        partial void OnYaxisMinChanged(double value)
        {
            if (Chart == null)
                return;
            if (IsYaxisAuto)
                return;

            Chart.Plot.Axes.SetLimitsY(value, YaxisMax);
            RefreshChart("OnYaxisMaxChanged");
        }
        [ObservableProperty]
        public double yaxisMax = 5000;
        partial void OnYaxisMaxChanged(double value)
        {
            if (Chart == null)
                return;
            if (IsYaxisAuto)
                return;

            Chart.Plot.Axes.SetLimitsY(YaxisMin, value);
            RefreshChart("OnYaxisMinChanged");
        }
        [ObservableProperty]
        int displaySize = 5000;
        partial void OnDisplaySizeChanged(int value)
        {
            //if (value > GlobalConstants.maxDisplaySize)
            //{
            //    DisplaySize = GlobalConstants.maxDisplaySize;
            //}
            //else if (value < 10)
            //{
            //    DisplaySize = 10;
            //}
        }



        /*-------- Relay Commands --------*/
        [RelayCommand(CanExecute = nameof(CanExecute))]
        void ShowHidePlot(object? param)
        {
            // Open Plot List Window
            PlotListWindow? PlotListWindow = Application.Current.Windows.OfType<PlotListWindow>().FirstOrDefault();
            if (PlotListWindow == null)
            {
                // The window is not open, so create a new instance and show it
                PlotListWindow = new PlotListWindow();
                //PlotListWindow.Owner = App.Current.MainWindow; // Set parent window
                //oxyPlotViewWindow.DataContext = App.pwmControlViewModel;
                PlotListWindow.Show();
                PlotListWindow.DataContext = new PlotListWidowViewModel(this);
                if ((window != null) && (window.IsLoaded))
                {
                    PlotListWindow.Owner = window;
                }
            }
            else
            {
                // The window is already open, so bring it to the front
                PlotListWindow.Activate();
            }
        }
        [RelayCommand]
        void ShowAllPlot(object? param)
        {
            if (PlotList is null)
                return;
            if (PlotList.Count == 0)
                return;

            foreach (PlotListModel plot in PlotList)
            {
                plot.IsVisible = true;
            }
        }

        [RelayCommand]
        void HideAllPlot(object? param)
        {
            if (PlotList is null)
                return;
            if (PlotList.Count == 0)
                return;
            foreach (PlotListModel plot in PlotList)
            {
                plot.IsVisible = false;
            }
        }

        #endregion // MVVM

        public Window? window; // for cursor detection
        public SignalXY? CurrentPlot { get; set; }

        public Marker? cursorMarker { get; set; }
        Text? cursorText;

        // Add draggable horizontal and vertical lines
        CrosslineModel[] crosslines = new CrosslineModel[2] { new(), new() };
        int crosslineIndex = 0;
        Annotation? crosslineCursorAnnotation;


        AxisLine? PlottableBeingDragged = null;
        VerticalLine? vLineBeingDragged = null;
        HorizontalLine? hLineBeingDragged = null;

        public void Init(Window _window, bool clearPlots = true)
        {
            this.window = _window;

            Chart ??= new();

            if (clearPlots)
                Chart.Plot.Clear();

            PlotList ??= new();

            Chart.Plot.Legend.IsVisible = false;

            //// Crosss Line Cursors
            InitCursor();

            // use events for custom mouse interactivity
            Chart.MouseDown += Chart_MouseDown;
            Chart.MouseUp += Chart_MouseUp;
            Chart.MouseMove += Chart_MouseMove;
            Chart.MouseWheel += Chart_MouseWheel;

            InitMenu();

            //// Init custom mouse actions
            // https://github.com/ScottPlot/ScottPlot/blob/main/src/ScottPlot5/ScottPlot5%20Demos/ScottPlot5%20WinForms%20Demo/Demos/CustomMouseActions.cs
            // richTextBox1.Text = "middle-click-drag pan, right-click zoom rectangle, right-click autoscale, left-click menu";
            ScottPlot.Control.InputBindings customInputBindings = new()
            {
                // Standard
                // "left-click-drag pan, right-click-drag zoom, middle-click autoscale, right-click menu"
                DragPanButton = ScottPlot.Control.MouseButton.Left,
                DragZoomRectangleButton = ScottPlot.Control.MouseButton.Middle,
                DragZoomButton = ScottPlot.Control.MouseButton.Right,
                ClickAutoAxisButton = ScottPlot.Control.MouseButton.Left,
                ClickContextMenuButton = ScottPlot.Control.MouseButton.Right,
                DoubleClickButton = ScottPlot.Control.MouseButton.Left,
                ZoomInWheelDirection = ScottPlot.Control.MouseWheelDirection.Up,
                ZoomOutWheelDirection = ScottPlot.Control.MouseWheelDirection.Down,
                LockHorizontalAxisKey = ScottPlot.Control.Key.Shift,
                LockVerticalAxisKey = ScottPlot.Control.Key.Ctrl,
                PanZoomRectangleKey = ScottPlot.Control.Key.Alt,

                //// Custom
                //DragPanButton = ScottPlot.Control.MouseButton.Middle,
                //DragZoomRectangleButton = ScottPlot.Control.MouseButton.Right,
                //DragZoomButton = ScottPlot.Control.MouseButton.Right,
                //ZoomInWheelDirection = ScottPlot.Control.MouseWheelDirection.Up,
                //ZoomOutWheelDirection = ScottPlot.Control.MouseWheelDirection.Down,
                //ClickAutoAxisButton = ScottPlot.Control.MouseButton.Right,
                //ClickContextMenuButton = ScottPlot.Control.MouseButton.Left,
            };

            ScottPlot.Control.Interaction interaction = new(Chart)
            {
                //Inputs = ScottPlot.Control.InputBindings.Standard(), 
                Inputs = customInputBindings,
                Actions = ScottPlot.Control.PlotActions.Standard(),
            };

            Chart.Interaction = interaction;

            Chart.Refresh();

            LogMessage = Console.WriteLine;
        }

        void InitMenu()
        {
            Chart.Menu.Reset();
            Chart.Menu.Add("Clear", (Chart) =>
            {
                Chart.Plot.Clear();
                PlotList.Clear();
                Chart.Plot.Axes.AutoScale();
                Chart.Refresh();
            });
            Chart.Menu.Add("Open in new window", (Chart) =>
            {
                WpfPlotViewer.Launch(Chart.Plot, "");
            });
            Chart.Menu.Add("Help", (Chart) =>
            {
                ShowMessage(
                            "Left-click \t\t:\tAuto Scale\n" +
                            "Left-click-Drag \t\t:\tPan\n" +
                            "Right-click-Drag \t\t:\tZoom rectangle\n" +
                            "Right-click \t\t:\tMenu");
            });
        }

        private void ShowMessage(string message)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = message;
            textBlock.FontSize = 24;
            textBlock.Background = System.Windows.Media.Brushes.LightYellow;
            textBlock.Foreground = System.Windows.Media.Brushes.Blue;
            textBlock.FontFamily = new System.Windows.Media.FontFamily("Colas");

            Window messageWindow = new Window();
            messageWindow.Width = 640;
            messageWindow.Height = 240;
            messageWindow.Title = "Mouse Help";
            messageWindow.Content = textBlock;
            messageWindow.ShowDialog();
        }
        /// <summary>
        /// Adds a plot to the view model.
        /// </summary>
        /// <param name="viewModel">The view model to add the plot to.</param>
        /// <param name="x">The x-values of the plot.</param>
        /// <param name="y">The y-values of the plot.</param>
        /// <param name="name">The name of the plot.</param>
        /// <param name="description">The description of the plot.</param>
        /// <param name="color">The color of the plot.</param>
        public void AddPlot(PlotPageViewModel? viewModel, double[]? x = null, double[]? y = null,
                string? name = null, string? description = null, ScottPlot.Color? color = null)
        {
            if (viewModel == null)
                return;

            viewModel.Chart ??= new();
            viewModel.PlotList ??= new();
            PlotListModel plotListModel = new(viewModel)
            {
                Name = name ?? "signalXY",
                Description = description ?? "signalXY",
            };
            // TODO: add legend image
            viewModel.PlotList.Add(plotListModel);
            int index = viewModel.PlotList.Count - 1;
            viewModel!.PlotIndex = index;
            plotListModel.Plot = viewModel.Chart!.Plot.Add.SignalXY(viewModel.PlotList[index].xs!, viewModel.PlotList[index].ys!, color);
            plotListModel.Plot.LegendText = name ??= "plot"; // Set legend text
            plotListModel.Plot.MarkerShape = MarkerShape.FilledCircle;
            plotListModel.Plot.MarkerSize = 3;
            viewModel.CurrentPlot = viewModel.PlotList!.Last().Plot;

            if ((x == null) || (y == null))
            {
                return;
            }
            UpdatePlot(viewModel, index, x, y);
            return;
        }
        public void UpdatePlot(PlotPageViewModel? viewModel, int index, double[]? x, double[]? y)
        {
            if (!viewModel.IsUpdatePlot || viewModel == null || viewModel.PlotList == null || index > viewModel.PlotList.Count - 1 ||
                viewModel.PlotList[index].plot == null || viewModel.PlotList[index].xs == null || viewModel.PlotList[index].ys == null ||
                x == null || y == null)
            {
                return;
            }

            int length = IsLimitDisplaySize ? Math.Min(DisplaySize, x.Length) : x.Length;
            DisplayLength = length;

            Array.Copy(x.TakeLast(length).ToArray(), viewModel.PlotList[index].xs, length);
            Array.Copy(y.TakeLast(length).ToArray(), viewModel.PlotList[index].ys, length);

            if (viewModel.IsYaxisAuto)
            {
                viewModel.Chart.Plot.Axes.AutoScaleY();
            }
            else
            {
                viewModel.Chart.Plot.Axes.SetLimitsY(viewModel.YaxisMin, viewModel.YaxisMax);
            }

            viewModel.PlotList[index].Plot.Data.MaximumIndex = length == 0 ? 0 : length - 1;
            int minIndex = viewModel.PlotList[index].plot.Data.MaximumIndex - length - 1;
            minIndex = (minIndex < 0) ? 0 : minIndex;
            viewModel.PlotList[index].Plot.Data.MinimumIndex = IsLimitDisplaySize ? minIndex : 0;

            var limits = viewModel.Chart.Plot.Axes.GetDataLimits();
            viewModel.Chart.Plot.Axes.SetLimitsX(viewModel.PlotList[index].xs[viewModel.PlotList[index].Plot.Data.MinimumIndex], limits.XRange.Max);

            viewModel.RefreshChart();
            GC.Collect();
        }
        public void RefreshChart(string? description = "")
        {
            if (window == null) { return; }

            if (Chart == null)
                return;

            UIHelper.InvokeOnUIThread(window, () =>
            {
                Chart.Refresh();
            });
        }

        private void Chart_MouseDown(object? sender, System.Windows.Input.MouseEventArgs e)
        {
            // sender is the object that raised the event
            // Get the mouse position
            //Point position = e.GetPosition(sender as UIElement);
            Pixel pixelPos = Chart.GetPlotPixelPosition(e);
            var lineUnderMouse = GetLineUnderMouse((float)pixelPos.X, (float)pixelPos.Y);
            if (lineUnderMouse is not null)
            {
                PlottableBeingDragged = lineUnderMouse;
                if (lineUnderMouse is VerticalLine vLine)
                {
                    vLineBeingDragged = vLine;
                    foreach (var crossline in crosslines)
                    {
                        if (crossline.vLine == vLine)
                        {
                            crosslineIndex = Array.IndexOf(crosslines, crossline);
                        }
                    }
                    Debug.WriteLine($"index ={crosslineIndex}");
                }

                Chart.Interaction.Disable(); // disable panning while dragging
            }
        }

        private void Chart_MouseUp(object? sender, System.Windows.Input.MouseEventArgs e)
        {
            PlottableBeingDragged = null;
            vLineBeingDragged = null;
            hLineBeingDragged = null;
            Chart.Interaction.Enable(); // enable panning again
            Chart.Refresh();
        }

        private void Chart_MouseMove(object? sender, MouseEventArgs e)
        {
            if (CurrentPlot == null)
            {
                return;
            }

            if (!IsShowCursor)
            {
                return;
            }

            // Get the mouse position
            Pixel mousePixel = Chart.GetPlotPixelPosition(e);
            Coordinates mouseLocation = Chart.Plot.GetCoordinates(mousePixel);
            DataPoint dp = CurrentPlot.Data.GetNearest(mouseLocation, Chart.Plot.LastRender, (float)Chart.Plot.Axes.GetLimits().XRange.Max);

            cursorMarker.X = dp.X;
            cursorMarker.Y = dp.Y;
            cursorMarker.Color = CurrentPlot.Color;
            cursorText.LabelText = $"{dp.X:N4}, {dp.Y:N1}";
            cursorText.Location = new(dp.X, dp.Y);

            if (PlottableBeingDragged != null)
            {
                if (PlottableBeingDragged is VerticalLine)
                {
                    UpdateCrossLinesPosition(crosslines[crosslineIndex], dp);
                    UpdateCrossLineText();
                }
            }

            foreach (var crossline in crosslines)
            {
                crossline.xAnnotation.OffsetX = Chart.Plot.GetPixel(new(crossline.point.Value.X, crossline.point.Value.Y)).X - 40;
            }


            crosslineCursorAnnotation!.Text = $"{dp.X:N4}, {dp.Y:N1}\nC0: {crosslines[0]!.point!.Value.X:N4}, {crosslines[0]!.point!.Value.Y:N1}\n" +
                $"C1: {crosslines[1]!.point!.Value.X:N4}, {crosslines[1]!.point!.Value.Y:N1}\n" +
                 $"ΔX: {crosslines[1]!.point!.Value.X - crosslines[0]!.point!.Value.X:N4}\nΔY: {crosslines[1]!.point!.Value.Y - crosslines[0]!.point!.Value.Y:N1}";

            Chart.Refresh();
        }

        private void Chart_MouseWheel(object? sender, MouseEventArgs e)
        {
            if (!IsShowCursor)
            {
                return;
            }

            foreach (var crossline in crosslines)
            {
                crossline.xAnnotation.OffsetX = Chart.Plot.GetPixel(new(crossline.point.Value.X, crossline.point.Value.Y)).X - 40;
            }

        }
        public void InitCursor()
        {
            //ClearAnnotations();
            //ClearCrosslines();
            //ClearTexts();

            /// Marker
            cursorMarker = Chart.Plot.Add.Marker(0, 0, MarkerShape.OpenCircle, 10);
            cursorMarker.IsVisible = IsShowCursor;

            cursorText = Chart.Plot.Add.Text("", 0, 0);
            cursorText.IsVisible = IsShowCursor;
            cursorText.LabelBackgroundColor = ScottPlot.Colors.Yellow;
            cursorText.LabelBold = true;
            cursorText.IsVisible = IsShowCursor;
            cursorText.LabelBorderColor = ScottPlot.Colors.Black;
            cursorText.Alignment = Alignment.LowerLeft;

            crosslineCursorAnnotation = Chart.Plot.Add.Annotation($"", Alignment.UpperLeft);
            crosslineCursorAnnotation.Alignment = ScottPlot.Alignment.UpperLeft;
            crosslineCursorAnnotation.IsVisible = false;
            crosslineCursorAnnotation.Label.FontSize = 12;
            crosslineCursorAnnotation.Label.ForeColor = ScottPlot.Colors.Black;
            crosslineCursorAnnotation.Label.BackgroundColor = ScottPlot.Colors.Transparent;
            crosslineCursorAnnotation.Label.ShadowColor = ScottPlot.Colors.Transparent;
            crosslineCursorAnnotation.LabelText = "C0: 0, 0\nC1: 0, 0\nΔX: 0, ΔY: 0";
            crosslineCursorAnnotation.LabelBold = true;

            // Add C0, C1 cross lines
            foreach (var crossline in crosslines)
            {
                int index = Array.IndexOf(crosslines, crossline);

                double pointX = 0;
                double pointY = 0;

                crossline.vLine = Chart.Plot.Add.VerticalLine(pointX);
                crossline.vLine.LinePattern = LinePattern.DenselyDashed;
                crossline.vLine.LineColor = ScottPlot.Colors.DarkCyan;
                crossline.vLine.LineWidth = 2;
                crossline.vLine.IsVisible = IsShowCursor;
                crossline.vLine.IsDraggable = true;
                crossline.vLine.Text = "";
                crossline.vLine.LabelFontSize = 12;
                crossline.vLine.ManualLabelAlignment = Alignment.LowerCenter;
                crossline.vLine.LabelBackgroundColor = ScottPlot.Colors.Transparent;
                crossline.vLine.LabelFontColor = ScottPlot.Colors.Black;

                crossline.hLine = Chart.Plot.Add.HorizontalLine(pointY);
                crossline.hLine.LinePattern = LinePattern.DenselyDashed;
                crossline.hLine.LineColor = ScottPlot.Colors.DarkCyan;
                crossline.hLine.LineWidth = 2;
                crossline.hLine.IsVisible = IsShowCursor;
                crossline.hLine.IsDraggable = false;
                crossline.hLine.Text = "";
                crossline.hLine.LabelFontSize = 12;
                crossline.hLine.ManualLabelAlignment = Alignment.LowerLeft;
                crossline.hLine.LabelBackgroundColor = ScottPlot.Colors.Transparent;
                crossline.hLine.LabelFontColor = ScottPlot.Colors.Black;
                crossline.hLine.LabelRotation = 0;
                crossline.hLine.LabelOffsetX = 5;

                crossline.xAnnotation = Chart.Plot.Add.Annotation($"C{index}", Alignment.UpperLeft);
                crossline.xAnnotation.LabelShadowColor = ScottPlot.Colors.Transparent;
                crossline.xAnnotation.LabelBackgroundColor = ScottPlot.Colors.Transparent;
                crossline.xAnnotation.LabelBold = true;
                crossline.xAnnotation.IsVisible = IsShowCursor;

                crossline.xText = Chart.Plot.Add.Text($"C{index}", 0, 0);
                crossline.xText.LabelBackgroundColor = ScottPlot.Colors.Yellow;
                crossline.xText.LabelBold = true;
                crossline.xText.IsVisible = IsShowCursor;
                crossline.xText.LabelBorderColor = ScottPlot.Colors.Black;
                crossline.xText.Alignment = Alignment.UpperCenter;
                //crossline.yText = Chart.Plot.Add.Text($"C{index}", 0, 0);

                ClearLegends();
            }
        }
        void UpdateCrossLinesPosition(CrosslineModel crossline, DataPoint dp)
        {
            if (dp.IsReal)
            {
                crossline.vLine.X = dp.X;
                crossline.hLine.Y = dp.Y;
                crossline.point = dp;

                var dataLimits = Chart.Plot.Axes.GetDataLimits();
                var axisLimits = Chart.Plot.Axes.GetLimits();
            }
        }

        private AxisLine? GetLineUnderMouse(float x, float y)
        {
            CoordinateRect rect = Chart.Plot.GetCoordinateRect(x, y, radius: 10);

            foreach (AxisLine axLine in Chart.Plot.GetPlottables<AxisLine>().Reverse())
            {
                if (axLine.IsUnderMouse(rect))
                    return axLine;
            }

            return null;
        }

        public void UpdateCrossLineText()
        {
            foreach (var crossline in crosslines)
            {
                var dataLimits = Chart.Plot.Axes.GetDataLimits();
                var axisLimits = Chart.Plot.Axes.GetLimits();
                crossline.xText.Location = new() { X = crossline.vLine.X, Y = Math.Min(dataLimits.YRange.Max, axisLimits.YRange.Max) };
            }
        }

        public void ClearAnnotations()
        {
            if (Chart == null)
            {
                return;
            }

            var plots = Chart.Plot.GetPlottables();

            // Delete all annotations in plots
            foreach (var plot in plots)
            {
                if (plot is Annotation annotation)
                {
                    Chart.Plot.Remove(annotation);
                }
            }
        }

        public void ClearCrosslines()
        {
            crosslines = new CrosslineModel[2] { new(), new() };

            if (Chart == null)
            {
                return;
            }

            var plots = Chart.Plot.GetPlottables();

            // Delete all annotations in plots
            foreach (var plot in plots)
            {
                if ((plot is HorizontalLine) || ((plot is VerticalLine)))
                {
                    Chart.Plot.Remove(plot);
                }
            }
        }

        // Clear all text contains "C0", "C1"
        public void ClearTexts()
        {
            if (Chart == null)
            {
                return;
            }

            var plots = Chart.Plot.GetPlottables();

            // Delete all annotations in plots
            foreach (var plot in plots)
            {
                if (plot is Text text)
                {
                    if (text.LabelText.Contains("C0") || text.LabelText.Contains("C1"))
                        Chart.Plot.Remove(text);
                }
            }
        }
        public void ClearLegends()
        {
            if (Chart == null)
            {
                return;
            }

            var plots = Chart.Plot.GetPlottables();

            // Delete all annotations in plots
            foreach (var plot in plots)
            {
                if (plot is VerticalLine vLine)
                {
                    //vLine.LegendItems.ToList();
                }

                if (plot is VerticalLine hLine)
                {

                }
            }
        }

        public System.Drawing.Color GetComplementColor(System.Drawing.Color color)
        {
            return System.Drawing.Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
        }

        void ShowCrossLineCursor(bool isShow, ScottPlot.Color? color = null)
        {
            double pointX = 0;
            double pointY = 0;
            DataPoint dp = new();
            if (CurrentPlot != null)
            {
                var limits = Chart.Plot.Axes.GetDataLimits();
                pointX = (limits.XRange.Min + limits.XRange.Max) / 2;
                dp = new DataPoint(pointX, 0, 0);
            }

            foreach (var crossline in crosslines)
            {
                crossline.hLine.IsVisible = isShow;
                crossline.vLine.IsVisible = isShow;
                crossline.hLine.Y = pointY;
                crossline.vLine.X = pointX;
                crossline.xAnnotation.IsVisible = false;
                crossline.xText.IsVisible = isShow;
                ClearLegends();
                UpdateCrossLinesPosition(crossline, dp);
            }
            UpdateCrossLineText();

            crosslineCursorAnnotation!.IsVisible = isShow;
        }

        public void UpdateStatusText(string status)
        {
            StatusString = status;
        }
        public void UpdateXy(int index, double[] x, double[] y)
        {
            if (!IsUpdatePlot)
                return;

            if (Chart == null)
            {
                return;
            }
            if (PlotList == null)
            {
                return;
            }

            if (PlotList.Count == 0)
            {
                return;
            }

            if (x.Length != y.Length)
                return;

            UpdateXy(PlotList[index].plot, x, y);
        }
        public void UpdateXy(string name, double[] x, double[] y)
        {
            if (!IsUpdatePlot)
                return;

            if (Chart == null)
            {
                return;
            }
            if (PlotList == null)
            {
                return;
            }

            if (PlotList.Count == 0)
            {
                return;
            }

            if (x.Length != y.Length)
                return;

            PlotListModel? plotListModel = PlotList.Where(x => x.Name == name).Select(x => x).FirstOrDefault();
            if (plotListModel == null)
            {
                return;
            }
            UpdateXy(plotListModel.plot, x, y);
        }
        public void UpdateXy(SignalXY? plot, double[] x, double[] y)
        {
            if (Chart == null)
            {
                return;
            }
            if (plot == null)
            {
                Console.WriteLine($"Plot {nameof(plot)} is null");
                return;
            }

            // Clone x and y
            double[] xClone = (double[])x.Clone();
            double[] yClone = (double[])y.Clone();
            if (xClone.Length != yClone.Length)
            {
                return;
            }

            // Update plot
            Chart.Plot.Remove(plot);
            double[] xs = IsLimitDisplaySize ? xClone.TakeLast(DisplaySize).ToArray() : x;
            double[] ys = IsLimitDisplaySize ? yClone.TakeLast(DisplaySize).ToArray() : y;
            plot = Chart.Plot.Add.SignalXY(xs, ys);

            if (IsYaxisAuto)
            {
                Chart.Plot.Axes.AutoScaleY();
            }
            else
            {
                Chart.Plot.Axes.SetLimitsY(YaxisMin, YaxisMax);
            }
            Chart.Plot.Axes.AutoScaleX();

            DisplayLength = xs.Length;

            //foreach (var crossline in crosslines)
            //{
            //    UIHelper.InvokeOnUIThread(window!, () => UpdateCrossLineCursor(crossline));
            //}
            RefreshChart();

            GC.Collect();
        }
        private bool CanExecute(object? param)
        {
            return true;  // 假設都執行
        }
        public void ClearPlots()
        {
            if (Chart == null)
                return;

            Chart.Plot.Clear();
            PlotList?.Clear();
        }

        /// <summary>
        /// Returns the SignalXY object and data point beneath the mouse,
        /// or null if nothing is beneath the mouse.
        /// </summary>
        private static (SignalXY? signalXY, DataPoint point) GetSignalXYUnderMouse(Plot plot, double x, double y)
        {
            Pixel mousePixel = new(x, y);

            Coordinates mouseLocation = plot.GetCoordinates(mousePixel);

            foreach (SignalXY signal in plot.GetPlottables<SignalXY>().Reverse())
            {
                DataPoint nearest = signal.Data.GetNearest(mouseLocation, plot.LastRender);
                if (nearest.IsReal)
                {
                    return (signal, nearest);
                }
            }

            return (null, DataPoint.None);
        }

        //public static BitmapImage ConvertToBitmapImage(System.Drawing.Bitmap bitmap)
        //{
        //    using (var memory = new MemoryStream())
        //    {
        //        bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
        //        memory.Position = 0;

        //        var bitmapImage = new BitmapImage();
        //        bitmapImage.BeginInit();
        //        bitmapImage.StreamSource = memory;
        //        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        //        bitmapImage.EndInit();
        //        bitmapImage.Freeze(); // optional

        //        return bitmapImage;
        //    }
        //}
        //public static BitmapImage ConvertToBitmap(System.Drawing.Bitmap bitmap)
        //{
        //    BitmapImage bi = new();
        //    bi.BeginInit();
        //    MemoryStream ms = new();
        //    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
        //    ms.Seek(0, SeekOrigin.Begin);
        //    bi.StreamSource = ms;
        //    bi.EndInit();
        //    return bi;
        //}
    }
}