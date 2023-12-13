using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using System.Collections.ObjectModel;
using System.Drawing;
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
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateHotspotCommand))]
    private string hotspotTitle = "Nyt Hotspot";
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateHotspotCommand))]
    private string? hotspotColor;

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
            && HotspotTitle != "Nyt Hotspot" 
            && StartTime != string.Empty 
            && EndTime != string.Empty;
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
                Schedule schedule = new() { DayOfWeek = item.Key.ToString(), StartTime = TimeOnly.Parse(StartTime), EndTime = TimeOnly.Parse(EndTime) };
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

 

}
