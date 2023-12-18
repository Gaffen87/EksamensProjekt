using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using System.Collections;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows;
using UngHerningSSP.Models;
using UngHerningSSP.Models.Repositories;
using UngHerningSSP.Services;

namespace UngHerningSSP.ViewModels;
public partial class MapViewModel : ViewModelBase
{
    HotspotRepo hotspotRepo = new();
    UserRepo userRepo = new();
    LocationRepo locationRepo = new();
    ScheduleRepo scheduleRepo = new();
    ObservationsRepo observationsRepo = new();
    public MapViewModel()
    {
        Initialize();
	}

    public ObservableCollection<Hotspot>? Hotspots { get; set; }
    public ObservableCollection<Observation>? Observations { get; set; }

    public List<string> Colors { get; set; } = new() {"Rød", "Gul", "Grøn" };
    public List<string> FilterColors { get; set; } = new() {"Alle", "Rød", "Gul", "Grøn" };

    // Properties relateret til kort og grafik på kortet
    [ObservableProperty] private Map? map;
    [ObservableProperty] private GraphicsOverlayCollection? graphicsOverlays;
    [ObservableProperty] private GraphicsOverlay? hotspotLayer;
    [ObservableProperty] private GraphicsOverlay? observationLayer;
    [ObservableProperty] private bool showHotspots = true;
	partial void OnShowHotspotsChanged(bool value)
	{
		HotspotLayer!.IsVisible = value;
	}
    [ObservableProperty] private bool showObservations = true;
	partial void OnShowObservationsChanged(bool value)
	{
		ObservationLayer!.IsVisible = value;
	}

	[ObservableProperty] private SimpleMarkerSymbol? currentPoint;
    [ObservableProperty] private Graphic? currentGraphic;
    [ObservableProperty] private MapPoint? currentMapPoint;
    [ObservableProperty] private double size = 10;

    // Properties relateret til hotspot
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateHotspotCommand))]
    private string hotspotTitle = "Nyt Hotspot";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateHotspotCommand))]
    private string? hotspotColor;
	partial void OnHotspotColorChanged(string? value)
    {
        switch (value)
        {
            case "Rød":
                CurrentPoint!.Color = Color.FromArgb(100, Color.Red);
                break;
            case "Gul":
                CurrentPoint!.Color = Color.FromArgb(100, Color.Yellow);
                break;
            case "Grøn":
                CurrentPoint!.Color = Color.FromArgb(100, Color.Green);
                break;
        }
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteHotspotCommand))]
    private Hotspot? selectedHotspot;

	// Properties relateret til filtrering af hotspot visning
	[ObservableProperty] private string? filterColor;
    [ObservableProperty] private Dictionary<DayOfWeek, bool>? filterDays;
	// Til TimePicker i Opret Hotspot
	[ObservableProperty] private string? startTime;
	[ObservableProperty] private string? endTime;
    [ObservableProperty] private Dictionary<DayOfWeek, bool>? selectedDays;

    // Properties relateret til Observations
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateObservationCommand))]
    private Observation? createdObservation;

	public string UserName { get; set; } = App.config.GetSection("CurrentUser").GetSection("Name").Value ?? "";
    public DateTime CurrentTime { get; set; } = DateTime.Now;

    public List<string> Behaviours { get; set; } = new() 
    { "Hærværk", "Fest", "Rusmidler", "Andet", "Intet at bemærke" };
    public List<string> Approaches { get; set; } = new() 
    { "Intet relevant", "Relationsarbejde", "Samtale", "Guidning", "Positive fællesskaber", "Andet" };

    private User CurrentUser()
    {
		return userRepo.Retrieve(int.Parse(App.config.GetSection("CurrentUser").GetSection("UserID").Value!));
	}

    private Location ClickedLocation()
    {
		Location location = new() { Latitude = Location.GetLatitude(CurrentMapPoint!), Longitude = Location.GetLongitude(CurrentMapPoint!) };
		location.ID = locationRepo.InsertLocation(location);
        return location;
	}

    private List<Schedule> CreateSchedule(Hotspot hotspot)
    {
        List<Schedule> schedules = new();
        foreach(var item in SelectedDays!)
        {
            //bool success = TimeOnly.TryParse(StartTime, out TimeOnly startTime) && TimeOnly.TryParse(EndTime, out TimeOnly endTime);
            if (item.Value == true)
            {
                Schedule schedule = new() { DayOfWeek = item.Key.ToString(), StartTime = DateTime.Parse(StartTime), EndTime = DateTime.Parse(EndTime) };
                schedule.ID = scheduleRepo.Insert(schedule, hotspot);
                schedules.Add(schedule);
            }
        }

        return schedules;
    }

    private void Initialize()
    {
		Hotspots = new(hotspotRepo.RetrieveAll());
        Observations = new(observationsRepo.GetList());

        SelectedDays = new Dictionary<DayOfWeek, bool>()
	    {
		    { DayOfWeek.Monday, false },
		    { DayOfWeek.Tuesday, false },
		    { DayOfWeek.Wednesday, false },
		    { DayOfWeek.Thursday, false },
		    { DayOfWeek.Friday, false },
		    { DayOfWeek.Saturday, false },
		    { DayOfWeek.Sunday, false }
	    };
		FilterDays = new Dictionary<DayOfWeek, bool>()
		{
			{ DayOfWeek.Monday, true },
			{ DayOfWeek.Tuesday, true },
			{ DayOfWeek.Wednesday, true },
			{ DayOfWeek.Thursday, true },
			{ DayOfWeek.Friday, true },
			{ DayOfWeek.Saturday, true },
			{ DayOfWeek.Sunday, true }
		};

		Map = ArcGIS.SetupMap(56.13, 8.98);
        HotspotLayer = ArcGIS.InitGraphicsOverlay();
        ObservationLayer = ArcGIS.InitGraphicsOverlay();
        GraphicsOverlays = new()
		{
			HotspotLayer,
            ObservationLayer
		};

		PopulateMap(Hotspots);
	}

    public void CreateNewPoint(MapPoint location)
    {
        CurrentMapPoint = ArcGIS.CreateMarker(location);

        CurrentPoint = ArcGIS.CreateSymbol(SimpleMarkerSymbolStyle.Circle, HotspotColor!, Size);

        CurrentGraphic = new Graphic(CurrentMapPoint, CurrentPoint);

        HotspotLayer!.Graphics.Add(CurrentGraphic);
	}

    private void PopulateMap(IEnumerable items) 
    {
        foreach (var item in items)
        {
            if (item is Hotspot)
            {
                var hotspot = item as Hotspot;
                var symbol = ArcGIS.CreateSymbol(SimpleMarkerSymbolStyle.Circle, hotspot!.Priority, Size);
                MapPoint location = new(hotspot.Location.Longitude, hotspot.Location.Latitude, SpatialReferences.Wgs84);
                HotspotLayer!.Graphics.Add(new Graphic(location, symbol));
            }
            if (item is Observation)
            {
                var observation = item as Observation;
				var symbol = ArcGIS.CreateSymbol(SimpleMarkerSymbolStyle.X, observation!.Severity, Size);
				MapPoint location = new(observation.Location.Longitude, observation.Location.Latitude, SpatialReferences.Wgs84);
				ObservationLayer!.Graphics.Add(new Graphic(location, symbol));
			}
        }
    }

    [RelayCommand(CanExecute = nameof(CanCreate))]
    public void CreateHotspot()
    {
        Hotspot hotspot = new() { Title = HotspotTitle, Priority = HotspotColor!, Location = ClickedLocation(), User = CurrentUser() };
        hotspot.ID = hotspotRepo.Insert(hotspot, CurrentUser(), ClickedLocation());
        hotspot.Schedules = CreateSchedule(hotspot);

        Hotspots!.Add(hotspot);
    }
    private bool CanCreate()
    {
        return HotspotColor != null
            && HotspotTitle != string.Empty
            && HotspotTitle != "Nyt Hotspot";
    }

    [RelayCommand]
    public void DeletePoint()
    {
        HotspotLayer!.Graphics.Remove(CurrentGraphic!);
    }

    [RelayCommand]
    public void FilterMap()
    {
        HotspotLayer!.Graphics.Clear();
        List<Hotspot> filter = new();
        foreach (var item in FilterDays!)
        {
            if (item.Value == true)
            {
                filter.AddRange(Hotspots!.Where(x => x.Schedules.Any(x => x.DayOfWeek == item.Key.ToString())).ToList());
            }
        }
        if (FilterColor != "Alle")
        {
            filter = filter.Where(x => x.Priority == FilterColor).ToList();
        }
        filter = filter.Distinct().ToList();
        PopulateMap(filter);
    }

    [RelayCommand]
    public void ClearFilter()
    {
        HotspotLayer!.Graphics.Clear();
        PopulateMap(Hotspots!);
    }

    [RelayCommand(CanExecute = nameof(CanDelete))]
    public void DeleteHotspot()
    {
        var result = MessageBox.Show($"Vil du slette Hotspottet: {SelectedHotspot!.Title}", "Er du sikker?", MessageBoxButton.YesNo);
        if (result == MessageBoxResult.Yes)
        {
            hotspotRepo.Delete(SelectedHotspot!);
            Hotspots!.Remove(SelectedHotspot!);
            HotspotLayer!.Graphics.Clear();
            PopulateMap(Hotspots);
        }
    }
    private bool CanDelete()
    {
        return SelectedHotspot != null;
    }

    [RelayCommand(CanExecute = nameof(CanCreateObservation))]
    public void CreateObservation()
    {

    }
    private bool CanCreateObservation()
    {
        return CreatedObservation.Approach != null
            && CreatedObservation.Behavior != null
            && CreatedObservation.Severity != null;
    }
}
