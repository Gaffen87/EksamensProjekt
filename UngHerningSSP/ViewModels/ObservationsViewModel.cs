using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Esri.ArcGISRuntime.Mapping;
using System.Collections.ObjectModel;
using System.Windows;
using UngHerningSSP.Models;
using UngHerningSSP.Models.Repositories;

namespace UngHerningSSP.ViewModels;
public partial class ObservationsViewModel : ViewModelBase
{
    ObservationsRepo observationsRepo = new();
    public ObservationsViewModel()
    {
        Observations = new(observationsRepo.RetrieveAll());
    }

    public ObservableCollection<Observation> Observations { get; set; }

	public List<string> Colors { get; set; } = new() { "Rød", "Gul", "Grøn" };
	public List<string> Behaviours { get; set; } = new()
	{ "Hærværk", "Fest", "Rusmidler", "Andet", "Intet at bemærke" };
	public List<string> Approaches { get; set; } = new()
	{ "Intet relevant", "Relationsarbejde", "Samtale", "Guidning", "Positive fællesskaber", "Andet" };

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(DeleteObservationCommand))]
	[NotifyCanExecuteChangedFor(nameof(UpdateObservationCommand))]
	private Observation? selectedObservation;

	[ObservableProperty] private Map? map;
	public void SetupMap(double latitude, double longitude)
	{
		Map = new(BasemapStyle.OSMStreets)
		{
			InitialViewpoint = new Viewpoint(latitude, longitude, 10000),
		};
	}

	[RelayCommand(CanExecute = nameof(CanUpdateDelete))]
	public void DeleteObservation()
	{
		var result = MessageBox.Show("Vil du slette observationen?", "Er du sikker?", MessageBoxButton.YesNo);
		if (result == MessageBoxResult.Yes)
		{
			observationsRepo.Delete(SelectedObservation!);
			Observations.Remove(SelectedObservation!);
		}
	}
	private bool CanUpdateDelete() => SelectedObservation != null;
	[RelayCommand(CanExecute = nameof(CanUpdateDelete))]
	public void UpdateObservation()
	{
		observationsRepo.Update(SelectedObservation!);
		MessageBox.Show("Observation opdateret");
	}
}
