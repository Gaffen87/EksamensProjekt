using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using UngHerningSSP.Models;
using UngHerningSSP.Models.Repositories;
using UngHerningSSP.Services;

namespace UngHerningSSP.ViewModels;
public partial class UserMapViewModel : ViewModelBase
{
    HotspotRepo hotspotRepo = new();
    UserRepo userRepo = new();
    LocationRepo locationRepo = new();
    ScheduleRepo scheduleRepo = new();
    public UserMapViewModel()
    {
        
        Initialize();
        
	}

    public ObservableCollection<Hotspot>? Hotspots { get; set; }

    public List<string> Colors { get; set; } = new() {"Rød", "Gul", "Grøn" };
    public List<string> FilterColors { get; set; } = new() {"Alle", "Rød", "Gul", "Grøn" };

    // Properties relateret til kort og grafik på kortet
    [ObservableProperty]
	private Map? map;
    [ObservableProperty]
    private GraphicsOverlayCollection? graphicsOverlays;
    [ObservableProperty]
    private GraphicsOverlay? symboler;
    [ObservableProperty]
    private SimpleMarkerSymbol? currentPoint;
    [ObservableProperty]
    private Graphic? currentGraphic;
    [ObservableProperty]
    private MapPoint? currentMapPoint;
    [ObservableProperty]
    private double size = 10;

    // Properties relateret til hotspot
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateHotspotCommand))]
    private string hotspotTitle = "Nyt Hotspot";
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateHotspotCommand))]
    private string? hotspotColor;

    // Properties relateret til filtrering af hotspot visning
    [ObservableProperty]
    private string? filterColor;
    [ObservableProperty]
    private Dictionary<DayOfWeek, bool>? filterDays;

	// Til TimePicker i Opret Hotspot
	[ObservableProperty]
	private string? startTime;
	[ObservableProperty]
	private string? endTime;
    [ObservableProperty]
    private Dictionary<DayOfWeek, bool>? selectedDays;

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
    [RelayCommand(CanExecute = nameof(CanCreate))]
    public void CreateHotspot()
    {
        Hotspot hotspot = new() { Title = HotspotTitle, Priority = HotspotColor!, Location = ClickedLocation(), User = CurrentUser() };
        hotspot.ID = hotspotRepo.InsertHotspot(hotspot, CurrentUser(), ClickedLocation());
        hotspot.Schedules = CreateSchedule(hotspot);

        Hotspots!.Add(hotspot);
    }
    private bool CanCreate()
    {
        return HotspotColor != null
            && HotspotTitle != string.Empty
            && HotspotTitle != "Nyt Hotspot";
    }

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
		Hotspots = new(hotspotRepo.RetrieveAllHotspots());

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

		Map = ArcGIS.SetupMap();
        Symboler = ArcGIS.InitGraphicsOverlay();
        GraphicsOverlays = new()
		{
			Symboler
		};

		PopulateMap(Hotspots);
	}

    public void CreateNewPoint(MapPoint location)
    {
        CurrentMapPoint = ArcGIS.CreateMarker(location);

        CurrentPoint = ArcGIS.CreateSymbol(SimpleMarkerSymbolStyle.Circle, HotspotColor!, Size);

        CurrentGraphic = new Graphic(CurrentMapPoint, CurrentPoint);

        Symboler!.Graphics.Add(CurrentGraphic);
	}

    private void PopulateMap(IEnumerable<Hotspot> hotspots) 
    {
        foreach (Hotspot hotspot in hotspots)
        {
            var symbol = ArcGIS.CreateSymbol(SimpleMarkerSymbolStyle.Circle, hotspot.Priority, Size);
			MapPoint location = new(hotspot.Location.Longitude, hotspot.Location.Latitude, SpatialReferences.Wgs84);
            Symboler!.Graphics.Add(new Graphic(location, symbol));
        }
    }

    [RelayCommand]
    public void DeletePoint()
    {
        Symboler!.Graphics.Remove(CurrentGraphic!);
    }

    [RelayCommand]
    public void FilterMap()
    {
        Symboler!.Graphics.Clear();
        List<Hotspot> filter = new();
        foreach (var item in FilterDays!)
        {
            if (item.Value == true)
            {
                filter = Hotspots!.Where(x => x.Schedules == null || x.Schedules.Any(x => x.DayOfWeek == item.Key.ToString())).ToList();
            }
        }
        if (FilterColor != "Alle")
        {
            filter = filter.Where(x => x.Priority == FilterColor).ToList();
        }
        PopulateMap(filter);
    }

    [RelayCommand]
    public void ClearFilter()
    {
        Symboler!.Graphics.Clear();
        PopulateMap(Hotspots!);
    }
}
