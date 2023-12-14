using Esri.ArcGISRuntime.Mapping;
using System.Windows;
using System.Windows.Controls;
using UngHerningSSP.ViewModels;

namespace UngHerningSSP.Views;
/// <summary>
/// Interaction logic for UserMapView.xaml
/// </summary>
public partial class UserMapView : Page
{
	UserMapViewModel viewModel = new();
	public UserMapView()
	{
		InitializeComponent();
		DataContext = viewModel;

		if (App.config.GetSection("CurrentUser").GetSection("IsAdmin").Value == "true")
		{
			btDelete.Visibility = Visibility.Visible;
			btNewObs.Visibility = Visibility.Collapsed;
		}
	}

	private void MapView_GeoViewTapped(object sender, Esri.ArcGISRuntime.UI.Controls.GeoViewInputEventArgs e)
	{
		if (App.config.GetSection("CurrentUser").GetSection("IsAdmin").Value == "true")
		{
			if (MarkerControl.Visibility == Visibility.Collapsed)
			{
				MarkerControl.Visibility = Visibility.Visible;
				viewModel.CreateNewPoint(e.Location!);
			}
			else
			{
				viewModel.DeletePoint();
				viewModel.CreateNewPoint(e.Location!);
			}
		}
	}

	private void btSave_Click(object sender, RoutedEventArgs e)
	{
		MarkerControl.Visibility = Visibility.Collapsed;
	}

	private void btCancel_Click(object sender, RoutedEventArgs e)
	{
		MarkerControl.Visibility = Visibility.Collapsed;
	}

	private async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		Viewpoint view = new(56.13, 8.98, 100000);
		Viewpoint newView = new(viewModel.SelectedHotspot.Location.Latitude, viewModel.SelectedHotspot.Location.Longitude, 10000);
		await MapView.SetViewpointAsync(view, TimeSpan.FromSeconds(1));
		await MapView.SetViewpointAsync(newView, TimeSpan.FromSeconds(1));
	}
}
