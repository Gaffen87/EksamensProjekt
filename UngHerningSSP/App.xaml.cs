using System.Configuration;
using System.Data;
using System.Windows;
using UngHerningSSP.ViewModels;
using UngHerningSSP.Views;

namespace UngHerningSSP;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	protected override void OnStartup(StartupEventArgs e)
	{
		MainWindow main = new MainWindow()
		{
			DataContext = new MainViewModel()
		};
		main.Show();
		base.OnStartup(e);
	}
}

