using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UngHerningSSP.Models;
using UngHerningSSP.Models.Repositories;

namespace UngHerningSSP.ViewModels;
public partial class UserObservationsViewModel : ViewModelBase
{
    ObservationsRepo observationsRepo = new();
    public UserObservationsViewModel()
    {
        Observations = new();
    }

    public ObservableCollection<Hotspot> Observations { get; set; }
}
