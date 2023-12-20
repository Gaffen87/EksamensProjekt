using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using UngHerningSSP.ViewModels;

namespace UngHerningSSP.Views;
/// <summary>
/// Interaction logic for LoginView.xaml
/// </summary>
public partial class LoginView : Page
{
	LoginViewModel viewModel = new();
	public LoginView()
	{
		InitializeComponent();
		DataContext = viewModel;
	}

	private void LoginButton_Click(object sender, RoutedEventArgs e)
	{
		if (viewModel.ValidateUser())
		{
			NavigationService.Navigate(new MapView());
		}
	}

	private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
	{
		viewModel.Password = PasswordBox.Password;
	}
}
