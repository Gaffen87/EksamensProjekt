using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Security;
using System.Windows;
using UngHerningSSP.Models;
using UngHerningSSP.Models.Repositories;

namespace UngHerningSSP.ViewModels;
public partial class LoginViewModel : ViewModelBase
{
	UserRepo userRepo = new();
	public LoginViewModel()
    {

	}

	[ObservableProperty] 
	private string? username;
	[ObservableProperty]
	private string? password;

	public bool ValidateUser()
	{
		bool result = false;
		try
		{
			User user = userRepo.Retrieve(Username!, Password!);

			if (user != null)
			{
				if (user.IsAdmin)
				{
					App.config.GetSection("UserSettings").GetSection("IsAdmin").Value = "true";
				}

				App.config.GetSection("UserSettings").GetSection("Name").Value = $"{user.FirstName} {user.LastName}";
				result = true;
				MessageBox.Show($"Logget ind som {App.config.GetSection("UserSettings").GetSection("Name").Value}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}
		catch (Exception e)
		{
			Debug.WriteLine(e.Message);
			MessageBox.Show("Forkert brugernavn eller password!", "Fejl i login", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		return result;
	}
}
