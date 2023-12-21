using Esri.ArcGISRuntime.Mapping;
using System.Windows;
using System.Windows.Controls;
using UngHerningSSP.ViewModels;

namespace UngHerningSSP.Views;
/// <summary>
/// Interaction logic for UserObservationsView.xaml
/// </summary>
public partial class ObservationsView : Page
{
	ObservationsViewModel viewModel = new();
	public ObservationsView()
	{
		InitializeComponent();
		DataContext = viewModel;
		if (App.config.GetSection("CurrentUser").GetSection("IsAdmin").Value == "false")
		{
			btEdit.Visibility = Visibility.Collapsed;
			btDelete.Visibility = Visibility.Collapsed;
		}
	}

	private void Button_Click(object sender, RoutedEventArgs e)
	{
		btCancel.Visibility = Visibility.Visible;
		btSave.Visibility = Visibility.Visible;
		tbSeverity.Visibility = Visibility.Collapsed;
		cbSeverity.Visibility = Visibility.Visible;
		tbBehaviour.Visibility = Visibility.Collapsed;
		cbBehaviour.Visibility = Visibility.Visible;
		tbApproach.Visibility = Visibility.Collapsed;
		cbApproach.Visibility = Visibility.Visible;
		tbCount.Visibility = Visibility.Collapsed;
		intCount.Visibility = Visibility.Visible;
		tbDescription.IsEnabled = true;
    }

	private void Button_Click_1(object sender, RoutedEventArgs e)
	{
		btCancel.Visibility = Visibility.Collapsed;
		btSave.Visibility = Visibility.Collapsed;
		tbSeverity.Visibility = Visibility.Visible;
		cbSeverity.Visibility = Visibility.Collapsed;
		tbBehaviour.Visibility = Visibility.Visible;
		cbBehaviour.Visibility = Visibility.Collapsed;
		tbApproach.Visibility = Visibility.Visible;
		cbApproach.Visibility = Visibility.Collapsed;
		tbCount.Visibility = Visibility.Visible;
		intCount.Visibility = Visibility.Collapsed;
		tbDescription.IsEnabled = false;
	}

	private async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (viewModel.SelectedObservation != null)
		{
			double latitude = viewModel.SelectedObservation.Location.Latitude;
			double longitude = viewModel.SelectedObservation.Location.Longitude;

			Viewpoint view = new(56.13, 8.98, 100000);
			Viewpoint newView = new(latitude, longitude, 10000);
			await ObsMapView.SetViewpointAsync(view, TimeSpan.FromSeconds(1));
			await ObsMapView.SetViewpointAsync(newView, TimeSpan.FromSeconds(1));
		}
		else
		{
			await ObsMapView.SetViewpointAsync(new Viewpoint(56.13, 8.98, 100000), TimeSpan.FromSeconds(1));
		}
	}
}
