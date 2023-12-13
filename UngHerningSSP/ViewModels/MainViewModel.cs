using CommunityToolkit.Mvvm.ComponentModel;

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
