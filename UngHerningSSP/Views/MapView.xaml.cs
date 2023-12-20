using Esri.ArcGISRuntime.Mapping;
using System.Windows;
using System.Windows.Controls;
using UngHerningSSP.ViewModels;

namespace UngHerningSSP.Views;
/// <summary>
/// Interaction logic for MapView.xaml
/// </summary>
public partial class MapView : Page
{
	MapViewModel viewModel = new();
	public MapView()
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
		if (viewModel.SelectedHotspot != null)
		{
			double latitude = viewModel.SelectedHotspot.Location.Latitude;
			double longitude = viewModel.SelectedHotspot.Location.Longitude;

			Viewpoint view = new(56.13, 8.98, 100000);
			Viewpoint newView = new(latitude, longitude, 10000);
			await MapViewControl.SetViewpointAsync(view, TimeSpan.FromSeconds(1));
			await MapViewControl.SetViewpointAsync(newView, TimeSpan.FromSeconds(1));
		}
		else
		{
			await MapViewControl.SetViewpointAsync(new Viewpoint(56.13, 8.98, 100000), TimeSpan.FromSeconds(1));
		}
	}

	private void btNewObs_Click(object sender, RoutedEventArgs e)
	{
		CreateObsPanel.Visibility = Visibility.Visible;
	}

	private void Button_Click(object sender, RoutedEventArgs e)
	{
		CreateObsPanel.Visibility= Visibility.Collapsed;
	}
}
