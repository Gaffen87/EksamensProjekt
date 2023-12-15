using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UngHerningSSP.Views;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
	}

	private void Frame_ContentRendered(object sender, EventArgs e)
	{
		btMap.IsEnabled = false;
		btObs.IsEnabled = false;
		Frame frame = sender as Frame;
		if (frame!.Content is UserMapView)
		{
			btObs.IsEnabled = true;
			btLogOut.Visibility = Visibility.Visible;
		}
		if (frame.Content is UserObservationsView)
		{ 
			btMap.IsEnabled = true;
			btLogOut.Visibility = Visibility.Visible;
		}
	}

	private void btMap_Click(object sender, RoutedEventArgs e)
	{
		MainFrame.NavigationService.Navigate(new UserMapView());
	}

	private void btObs_Click(object sender, RoutedEventArgs e)
	{
		MainFrame.NavigationService.Navigate(new UserObservationsView());
	}

	private void btLogOut_Click(object sender, RoutedEventArgs e)
	{
		App.config.GetSection("CurrentUser").GetSection("IsAdmin").Value = "false";
		MainFrame.NavigationService.Navigate(new LoginView());
		btLogOut.Visibility = Visibility.Collapsed;
	}
}