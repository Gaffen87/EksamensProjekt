using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace UngHerningSSP.ViewModels;
public partial class LoginViewModel : ViewModelBase
{
	private readonly MainViewModel mvm;

	public LoginViewModel(MainViewModel mvm)
    {
		this.mvm = mvm;
	}

    [RelayCommand]
	private void Navigate()
	{
		mvm.CurrentViewModel = new UserMapViewModel();
	}
}
