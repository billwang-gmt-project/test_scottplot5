using Plots;
using System.Configuration;
using System.Data;
using System.Windows;

namespace test_scottplot5
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static PlotWindowTestViewModel plotWindowViewModel;
        public static PlotWindow plotWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            plotWindow ??= new();
            plotWindowViewModel ??= new();
            plotWindow.DataContext = plotWindowViewModel;
            plotWindow.Show();
        }
    }



}
