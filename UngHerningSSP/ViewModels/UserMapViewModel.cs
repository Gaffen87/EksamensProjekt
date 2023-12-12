using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
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

		CreateNewPoint(new MapPoint(8.98, 56.13, SpatialReferences.Wgs84));
	}

    public ObservableCollection<Hotspot> Hotspots { get; set; }

    [RelayCommand(CanExecute = nameof(CanCreate))]
    public void CreateHotspot()
    {
        User user = userRepo.Retrieve(2);
        Location location = new() { Latitude = Location.GetLatitude(CurrentMapPoint!), Longitude = Location.GetLongitude(CurrentMapPoint!) };
        location.ID = locationRepo.InsertLocation(location);
        Hotspot hotspot = new() { Title = HotspotTitle, Priority = HotspotColor!, Location = location, User = user };
        hotspotRepo.InsertHotspot(hotspot, user, location);

        Hotspots.Add(hotspot);
    }
    
    private bool CanCreate()
    {
        return HotspotColor != null && HotspotTitle != string.Empty && HotspotTitle != "Nyt Hotspot";
    }

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

    public List<string> Colors { get; set; } = new() {"Rød", "Gul", "Grøn" };

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
