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
}
