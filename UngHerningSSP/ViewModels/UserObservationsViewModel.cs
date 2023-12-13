using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UngHerningSSP.Models;

namespace UngHerningSSP.ViewModels;
public partial class UserObservationsViewModel : ViewModelBase
{
    public ObservableCollection<Hotspot> Observations { get; set; }
}
