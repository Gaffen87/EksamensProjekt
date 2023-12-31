﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using System.Collections;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Windows;
using UngHerningSSP.Models;
using UngHerningSSP.Models.Repositories;
using UngHerningSSP.Services;
using static System.Formats.Asn1.AsnWriter;

namespace UngHerningSSP.ViewModels;
public partial class MapViewModel : ViewModelBase
{
    // dependencies
    HotspotRepo hotspotRepo = new();
    UserRepo userRepo = new();
    LocationRepo locationRepo = new();
    ScheduleRepo scheduleRepo = new();
    ObservationsRepo observationsRepo = new();

    public MapViewModel()
    {
        Initialize();
        Shape();
    }

    // lister der indeholder alle hotspots og hændelser fra db
    public ObservableCollection<Hotspot>? Hotspots { get; set; }
    public ObservableCollection<Observation>? Observations { get; set; }

    // Lister der bruges til dropdowns i UI
    public List<string> Colors { get; set; } = new() { "Rød", "Gul", "Grøn" };
    public List<string> FilterColors { get; set; } = new() { "Alle", "Rød", "Gul", "Grøn" };
	public List<string> Behaviours { get; set; } = new()
	{ "Hærværk", "Fest", "Rusmidler", "Andet", "Intet at bemærke" };
	public List<string> Approaches { get; set; } = new()
	{ "Intet relevant", "Relationsarbejde", "Samtale", "Guidning", "Positive fællesskaber", "Andet" };

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
	private async Task Shape()
	{
        ShapefileFeatureTable herningShapefile = await ShapefileFeatureTable.OpenAsync(@"../../../Services/ShapeFile/_12_kms_kommunegraenser.shp");

        FeatureLayer newLayer = new(herningShapefile);

        Map!.OperationalLayers.Add(newLayer);
    } // Henter information fra en shapefile om kommunegrænser og markerer Herning kommune


    // properties til at holde på det nyligt lavede symbol på kortet
    [ObservableProperty] private SimpleMarkerSymbol? currentSymbol;
    [ObservableProperty] private Graphic? currentGraphic;
    [ObservableProperty] private MapPoint? currentMapPoint;
    [ObservableProperty] private double size = 20;

    // Properties relateret til hotspot
    [ObservableProperty][NotifyCanExecuteChangedFor(nameof(CreateHotspotCommand))]
    private string hotspotTitle = "Nyt Hotspot";
    [ObservableProperty][NotifyCanExecuteChangedFor(nameof(CreateHotspotCommand))]
    private string? hotspotColor;
    partial void OnHotspotColorChanged(string? value) // Giver hotspot rigtig farve alt efter valg i dropdown menu
    {
        switch (value)
        {
            case "Rød":
                CurrentSymbol!.Color = Color.FromArgb(100, Color.Red);
                break;
            case "Gul":
                CurrentSymbol!.Color = Color.FromArgb(100, Color.Yellow);
                break;
            case "Grøn":
                CurrentSymbol!.Color = Color.FromArgb(100, Color.Green);
                break;
        }
    }

    [ObservableProperty][NotifyCanExecuteChangedFor(nameof(DeleteHotspotCommand))]
    private Hotspot? selectedHotspot;

    // Properties relateret til filtrering af hotspot visning
    [ObservableProperty] private string? filterColor;
    [ObservableProperty] private Dictionary<DayOfWeek, bool>? filterDays;
    // Til TimePicker i Opret Hotspot
    [ObservableProperty] private string startTime;
    [ObservableProperty] private string endTime;
    [ObservableProperty] private Dictionary<DayOfWeek, bool>? selectedDays;

    // Properties relateret til Observations
    [ObservableProperty] private Observation? createdObservation = new();

    [ObservableProperty][NotifyCanExecuteChangedFor(nameof(CreateObservationCommand))]
    private string approach; 
    partial void OnApproachChanged(string value)
	{
		CreatedObservation!.Approach = value;
	}
    [ObservableProperty][NotifyCanExecuteChangedFor(nameof(CreateObservationCommand))]
    private string behaviour;
	partial void OnBehaviourChanged(string value)
	{
		CreatedObservation!.Behaviour = value;
	}
    [ObservableProperty][NotifyCanExecuteChangedFor(nameof(CreateObservationCommand))]
    private string severity;
    partial void OnSeverityChanged(string value)
    {
        CreatedObservation!.Severity = value;
    }

	public string UserName { get; set; } = App.config.GetSection("CurrentUser").GetSection("Name").Value ?? "";
    public DateTime CurrentTime { get; set; } = DateTime.Now;

    // Henter brugeren der er logget ind fra db
    private User CurrentUser()
    {
		return userRepo.Retrieve(int.Parse(App.config.GetSection("CurrentUser").GetSection("UserID").Value!));
	}

    // gemmer den klikkede lokation i db
    private Location ClickedLocation()
    {
		Location location = new() { Latitude = Location.GetLatitude(CurrentMapPoint!), Longitude = Location.GetLongitude(CurrentMapPoint!) };
		location.ID = locationRepo.InsertLocation(location);
        return location;
	}

    // Enumerater de valgte dage og laver schedule efter hvilke der er valgt i UI. Gemmer Schedule i DB
    private List<Schedule> CreateSchedule(Hotspot hotspot)
    {
        List<Schedule> schedules = new();
        foreach(var item in SelectedDays!)
        {
            if (item.Value == true)
            {
                Schedule schedule = new() { DayOfWeek = item.Key.ToString(), StartTime = DateTime.Parse(StartTime), EndTime = DateTime.Parse(EndTime) };
                schedule.ID = scheduleRepo.Insert(schedule, hotspot);
                schedules.Add(schedule);
            }
        }

        if (schedules.Count > 0 && DateTime.Parse(StartTime!) < DateTime.Parse(EndTime!)) 
        {
            return schedules;
        }
        throw new ArgumentNullException();
    }

    // Initialisering af MapViewModel-klassen
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
        PopulateMap(Observations);
	}

    // Står for at lave et visuelt symbol hvor brugeren har klikket
    public void CreateNewPoint(MapPoint location)
    {
        CurrentMapPoint = location;

        CurrentSymbol = ArcGIS.CreateSymbol(SimpleMarkerSymbolStyle.Circle, HotspotColor!, Size);

        CurrentGraphic = new Graphic(CurrentMapPoint, CurrentSymbol);

        HotspotLayer!.Graphics.Add(CurrentGraphic);
	}

    // indsætter punkter på kortet ud fra en liste og tager højde for om det er en hændelse eller hotspot
    private void PopulateMap(IEnumerable items) 
    {
        foreach (var item in items)
        {
            if (item is Hotspot)
            {
                var hotspot = item as Hotspot;
                var symbol = ArcGIS.CreateSymbol(SimpleMarkerSymbolStyle.Circle, hotspot!.Priority, 20);
                MapPoint location = new(hotspot.Location.Longitude, hotspot.Location.Latitude, SpatialReferences.Wgs84);
                HotspotLayer!.Graphics.Add(new Graphic(location, symbol));
            }
            if (item is Observation)
            {
                var observation = item as Observation;
				var symbol = ArcGIS.CreateSymbol(SimpleMarkerSymbolStyle.X, observation!.Severity, 10);
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
        
        try
        {
            hotspot.Schedules = CreateSchedule(hotspot);
            Hotspots!.Add(hotspot);
        }
        catch (ArgumentNullException)
        {
			HotspotLayer!.Graphics.Clear();
			PopulateMap(Hotspots!);
            hotspotRepo.Delete(hotspot);
            MessageBox.Show($"Der skete en fejl: Vælg et valid tidspunkt", "Fejl");
		}
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
        ObservationLayer?.Graphics.Clear();
        List<Hotspot> filter = new();
        List<Observation> obsfilter = new();
        foreach (var item in FilterDays!)
        {
            if (item.Value == true)
            {
                filter.AddRange(Hotspots!.Where(x => x.Schedules.Any(x => x.DayOfWeek == item.Key.ToString())).ToList());
                obsfilter.AddRange(Observations!.Where(x => x.DateAndTime.DayOfWeek == item.Key));
            }
        }
        if (FilterColor != "Alle")
        {
            filter = filter.Where(x => x.Priority == FilterColor).ToList();
            obsfilter = obsfilter.Where(x => x.Severity == FilterColor).ToList();
        }
        filter = filter.Distinct().ToList();
        obsfilter = obsfilter.Distinct().ToList();
        PopulateMap(filter);
        PopulateMap(obsfilter);
    }

    [RelayCommand]
    public void ClearFilter()
    {
        HotspotLayer!.Graphics.Clear();
        ObservationLayer!.Graphics.Clear();
        PopulateMap(Hotspots!);
        PopulateMap(Observations!);
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
        Random rand = new();
        int latint = rand.Next(110, 175);
        int lonint = rand.Next(900, 999);
        CreatedObservation!.Location = new Location() 
        { 
            Latitude = double.Parse($"56.{latint}", CultureInfo.InvariantCulture), 
            Longitude = double.Parse($"8.{lonint}", CultureInfo.InvariantCulture) 
        };
        CreatedObservation!.Location.ID = locationRepo.InsertLocation(CreatedObservation.Location);
        CreatedObservation!.User = CurrentUser();
        CreatedObservation.DateAndTime = DateTime.Now;
        CreatedObservation!.ID = observationsRepo.Insert(CreatedObservation!);
        Observations!.Add(CreatedObservation!);
        PopulateMap(Observations);
    }
    private bool CanCreateObservation()
    {
        return CreatedObservation!.Approach != null
            && CreatedObservation.Behaviour != null
            && CreatedObservation.Severity != null;
    }
}
