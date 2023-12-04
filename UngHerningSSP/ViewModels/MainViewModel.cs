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
        CurrentViewModel = new LoginViewModel(this);
    }

    [ObservableProperty]
    private ViewModelBase currentViewModel;
}
