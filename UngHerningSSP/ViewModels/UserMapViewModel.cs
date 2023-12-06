using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Mapping;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.Symbology;
using System.Drawing;
using System.Transactions;
using WinRT;
using CommunityToolkit.Mvvm.Input;

namespace UngHerningSSP.ViewModels;
public partial class UserMapViewModel : ViewModelBase
{
    public UserMapViewModel()
    {
        SetupMap();
        CreateGraphics();
	}

    [ObservableProperty]
	private Map? map;
    [ObservableProperty]
    private GraphicsOverlayCollection? graphicsOverlays;
    [ObservableProperty]
    private GraphicsOverlay symboler;
    [ObservableProperty]
    private SimpleMarkerSymbol currentPoint;
    [ObservableProperty]
    private Graphic currentGraphic;
    [ObservableProperty]
    private MapPoint currentMapPoint;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentPoint))]
    private double size = 10;




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

        var symbol = new SimpleMarkerSymbol
        {
            Style = SimpleMarkerSymbolStyle.Circle,
            Color = Color.FromArgb(50, Color.Red),
            Size = size
        };

        symbol.Outline = new SimpleLineSymbol
        {
            Style = SimpleLineSymbolStyle.Solid,
            Color = Color.Orange,
            Width = 0.5
        };

        return new Graphic(location, symbol);
    }

    public void CreateNewPoint(MapPoint location)
    {
        var newPoint = new MapPoint(location.X, location.Y, location.SpatialReference);

        CurrentMapPoint = newPoint;

		var symbol = new SimpleMarkerSymbol
		{
			Style = SimpleMarkerSymbolStyle.Circle,
			Color = Color.FromArgb(50, Color.Red),
			Size = this.Size
		};

		symbol.Outline = new SimpleLineSymbol
		{
			Style = SimpleLineSymbolStyle.Solid,
			Color = Color.Orange,
			Width = 0.5
		};

        var newGraphic = new Graphic(newPoint, symbol);

        CurrentGraphic = newGraphic;

        Symboler.Graphics.Add(newGraphic);

        CurrentPoint = symbol;
	}

    [RelayCommand]
    public void DeletePoint()
    {
        Symboler.Graphics.Remove(CurrentGraphic);
    }
}
