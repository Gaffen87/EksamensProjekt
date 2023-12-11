using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UngHerningSSP.ViewModels;
public partial class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        currentViewModel = new LoginViewModel();
    }

    [ObservableProperty]
    private ViewModelBase currentViewModel;
}
