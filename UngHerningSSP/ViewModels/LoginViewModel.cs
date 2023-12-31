﻿using CommunityToolkit.Mvvm.ComponentModel;
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

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(NavigateCommand))]
	private string? username;
	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(NavigateCommand))]
	private string? password;

	// Tjekker om brugeren findes i databasen og sætter info i appsettings.json alt efter brugerens info
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
					App.config.GetSection("CurrentUser").GetSection("IsAdmin").Value = "true";
				}

				App.config.GetSection("CurrentUser").GetSection("Name").Value = $"{user.FirstName} {user.LastName}";
				App.config.GetSection("CurrentUser").GetSection("UserID").Value = user.ID.ToString();
				result = true;
				MessageBox.Show($"Logget ind som {App.config.GetSection("CurrentUser").GetSection("Name").Value}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}
		catch (Exception e)
		{
			Debug.WriteLine(e.Message);
			MessageBox.Show("Forkert brugernavn eller password!", "Fejl i login", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		return result;
	}

	[RelayCommand(CanExecute = (nameof(CanNavigate)))]
	public void Navigate()
	{

	}
	private bool CanNavigate()
	{
		return Username != null && Password != null;
	}
}
