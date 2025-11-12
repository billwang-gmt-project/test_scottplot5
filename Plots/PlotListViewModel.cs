using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Plots
{
    public partial class PlotListWidowViewModel : ObservableObject
    {
        PlotPageViewModel? plotPageViewModel;
        public PlotListWidowViewModel(PlotPageViewModel? plotPageViewModel)
        {
            if (plotPageViewModel == null)
            {
                return;
            }
            this.plotPageViewModel = plotPageViewModel;
            this.PlotList = plotPageViewModel.PlotList;
        }

        [ObservableProperty]
        public ObservableCollection<PlotListModel>? plotList; // Must use ObservableCollection when binding to item control to dynamic add/remove update

        [RelayCommand]
        void ClearAllPlots()
        {
            if (plotPageViewModel == null)
                return;

            plotPageViewModel.ClearPlots();
            plotPageViewModel.RefreshChart();
        }
        [RelayCommand]
        void ShowAllPlot()
        {
            if (plotPageViewModel == null)
                return;

            if (this.PlotList == null)
            {
                return;
            }
            foreach (var plot in PlotList)
            {
                plot.IsVisible = true;
            }

            plotPageViewModel.RefreshChart();
        }
        [RelayCommand]
        void HideAllPlot()
        {
            if (plotPageViewModel == null)
                return;

            if (this.PlotList == null)
            {
                return;
            }
            foreach (var plot in PlotList)
            {
                plot.IsVisible = false;
            }
            plotPageViewModel.RefreshChart();
        }
    }
}
