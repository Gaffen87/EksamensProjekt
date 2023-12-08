using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UngHerningSSP.Services;

public class ArcGIS
{
	public static Map SetupMap()
	{
		Map map = new(BasemapStyle.OSMLightGray);

		map.InitialViewpoint = new Viewpoint(56.13, 8.98, 100000);

		map.ReferenceScale = 100000;
		return map;
	}

	public static GraphicsOverlay InitGraphicsOverlay()
	{
		var symboler = new GraphicsOverlay();
		symboler.ScaleSymbols = true;

		return symboler;
	}

	public static MapPoint CreateMarker(MapPoint location)
	{
		return new MapPoint(location.X, location.Y, location.SpatialReference);


	}

	public static SimpleMarkerSymbol CreateSymbol(SimpleMarkerSymbolStyle style, Color color, double size)
	{
		var symbol = new SimpleMarkerSymbol
		{
			Style = style,
			Color = Color.FromArgb(50, color),
			Size = size
		};

		symbol.Outline = new SimpleLineSymbol
		{
			Style = SimpleLineSymbolStyle.Solid,
			Color = Color.Black,
			Width = 0.2
		};

		return symbol;
	}
}
