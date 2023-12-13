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
    public UserMapViewModel()
    {
        Hotspots = new(hotspotRepo.RetrieveAllHotspots());
        Initialize();
        PopulateMap();
	}

    public ObservableCollection<Hotspot> Hotspots { get; set; }

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
        hotspotRepo.InsertHotspot(hotspot, CurrentUser(), ClickedLocation());

        Hotspots.Add(hotspot);
    }
    private bool CanCreate()
    {
        return HotspotColor != null && HotspotTitle != string.Empty && HotspotTitle != "Nyt Hotspot";
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

    private void Initialize()
    {
        Map = ArcGIS.SetupMap();
        Symboler = ArcGIS.InitGraphicsOverlay();
        GraphicsOverlays = new()
		{
			Symboler
		};
    }

    public void CreateNewPoint(MapPoint location)
    {
        CurrentMapPoint = ArcGIS.CreateMarker(location);

        CurrentPoint = ArcGIS.CreateSymbol(SimpleMarkerSymbolStyle.Circle, HotspotColor!, Size);

        CurrentGraphic = new Graphic(CurrentMapPoint, CurrentPoint);

        Symboler!.Graphics.Add(CurrentGraphic);
	}

    private void PopulateMap() 
    {
        foreach (Hotspot hotspot in Hotspots)
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

    // Til TimePicker i Opret Hotspot
    [ObservableProperty]
    private string? hour;
    [ObservableProperty]
    private string? minute;

    public int[] Hours { get; set; } = Enumerable.Range(0, 24).ToArray();
    public int[] Minutes { get; set; } = Enumerable.Range(0, 59).Where(x => x % 5 == 0).ToArray();
}
