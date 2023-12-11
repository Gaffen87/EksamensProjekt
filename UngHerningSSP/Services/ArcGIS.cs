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
	// Opsætter kortet der vises via MapView control
	public static Map SetupMap()
	{
		// Sætter kortets udseende, zoom, sted og referenceskala
		Map map = new(BasemapStyle.OSMLightGray)
		{
			InitialViewpoint = new Viewpoint(56.13, 8.98, 100000),

			ReferenceScale = 100000
		};
		return map;
	}

	// Opretter et nyt lag til grafik på kortet som bruges til at vise symboler
	public static GraphicsOverlay InitGraphicsOverlay()
	{
		var symboler = new GraphicsOverlay();
		symboler.ScaleSymbols = true; // Grafik skalerer sammen med kortet

		return symboler;
	}

	public static MapPoint CreateMarker(MapPoint location)
	{
		return new MapPoint(location.X, location.Y, location.SpatialReference);
	}

	// Opretter nyt symbol med de valgte egenskaber 
	public static SimpleMarkerSymbol CreateSymbol(SimpleMarkerSymbolStyle style, string color, double size)
	{
		Color symbolColor = Color.Red;

		switch (color)
		{
			case "Rød":
				symbolColor = Color.Red;
				break;
			case "Gul":
				symbolColor = Color.Yellow;
				break;
			case "Grøn":
				symbolColor = Color.Green; 
				break;
		}

		var symbol = new SimpleMarkerSymbol
		{
			Style = style,
			Color = Color.FromArgb(100, symbolColor),
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
