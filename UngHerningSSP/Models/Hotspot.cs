using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UngHerningSSP.Models;
public class Hotspot
{
    public int ID { get; set; }

    public string Name { get; set; }

    public string Priority { get; set; }

    public Location Location { get; set; }

    public User User { get; set; }

    public Schedule[] Schedules { get; set; }
}
