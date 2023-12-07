using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;
using UngHerningSSP.DataAccess;
using UngHerningSSP.Models;
using UngHerningSSP.Models.Repositories;
using UngHerningSSP.ViewModels;
using UngHerningSSP.Views;

namespace UngHerningSSP;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	public readonly static IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

	protected override void OnStartup(StartupEventArgs e)
	{
		MainWindow main = new()
		{
			DataContext = new MainViewModel()
		};
		main.Show();
		base.OnStartup(e);
		Esri.ArcGISRuntime.ArcGISRuntimeEnvironment.ApiKey = "AAPKf3d1da66588d4eeb86250040d0d006b66nTEGQ35pEtvNAJVQ1Zo5-IORkoALRCSqPHiiTE9azkgejCSEV2qhr-Z2Pl6dJ-I";
	}
}

