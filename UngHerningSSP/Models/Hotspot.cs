using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UngHerningSSP.Models;
public class Hotspot
{
    public string HotspotID { get; set; }

    public string Name { get; set; }

    public string Priority { get; set; }

    public Location location { get; set; }

    public User User { get; set; }

    public Schedule[] schedules { get; set; }

}
