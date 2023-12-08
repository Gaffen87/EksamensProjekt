using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using UngHerningSSP.Models;
using UngHerningSSP.Models.Repositories;

namespace UngHerningSSP.ViewModels;
public partial class UserMapViewModel : ViewModelBase
{
    HotspotRepo hotspotRepo = new();
    UserRepo userRepo = new();
    LocationRepo locationRepo = new();
    public UserMapViewModel()
    {
        Hotspots = new(hotspotRepo.RetrieveAllHotspots());
        SetupMap();
        CreateGraphics();
	}

    public ObservableCollection<Hotspot> Hotspots { get; set; }

    public void CreateHotspot()
    {
        User user = userRepo.Retrieve(2);
        Location location = new() { Latitude = Location.GetLatitude(CurrentMapPoint), Longitude = Location.GetLongitude(CurrentMapPoint) };
        location.ID = locationRepo.InsertLocation(location);
        Hotspot hotspot = new() { Title = HotspotTitle, Priority = "Rød", Location = location, User = user };
        hotspotRepo.InsertHotspot(hotspot, user, location);

        Hotspots.Add(hotspot);
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
    [NotifyPropertyChangedFor(nameof(CurrentPoint))]
    private double size = 10;
    [ObservableProperty]
    private string hotspotTitle = "Ny Hotspot";
    [ObservableProperty]
    private string? hotspotColor;

    public string[] Colors { get; set; } = { "-- Vælg Prioritet --", "Rød", "Gul", "Grøn" };

	partial void OnHotspotColorChanged(string? value)
	{
		
	}

	private void SetupMap()
    {
        Map = new(BasemapStyle.OSMLightGray);

        Map.InitialViewpoint = new Viewpoint(56.13, 8.98, 100000);

        Map.ReferenceScale = 100000;
    }

    private void CreateGraphics()
    {
        GraphicsOverlays = new GraphicsOverlayCollection();

        Symboler = new GraphicsOverlay();

        Symboler.ScaleSymbols = true;

        GraphicsOverlays.Add(Symboler);

        var point = CreatePoint(8.98, 56.13, Size);

        Symboler.Graphics.Add(point);
    }

    private Graphic CreatePoint(double longitude, double latitude, double size)
    {
        var location = new MapPoint(longitude, latitude, SpatialReferences.Wgs84);

        var symbol = CreateSymbol(SimpleMarkerSymbolStyle.Circle, Color.Yellow);

        return new Graphic(location, symbol);
    }

    public void CreateNewPoint(MapPoint location)
    {
        var newPoint = new MapPoint(location.X, location.Y, location.SpatialReference);

        CurrentMapPoint = newPoint;

        var symbol = CreateSymbol(SimpleMarkerSymbolStyle.Circle, Color.Red);

        var newGraphic = new Graphic(newPoint, symbol);

        CurrentGraphic = newGraphic;

        Symboler.Graphics.Add(newGraphic);

        CurrentPoint = symbol;
	}

    public SimpleMarkerSymbol CreateSymbol(SimpleMarkerSymbolStyle style, Color color)
    {
        var symbol = new SimpleMarkerSymbol
        {
            Style = style,
            Color = Color.FromArgb(50, color),
            Size = this.Size
        };

        symbol.Outline = new SimpleLineSymbol
        {
            Style = SimpleLineSymbolStyle.Solid,
            Color = Color.Black,
            Width = 0.2
        };

        return symbol;
    }

    [RelayCommand]
    public void DeletePoint()
    {
        Symboler.Graphics.Remove(CurrentGraphic);
    }
}
