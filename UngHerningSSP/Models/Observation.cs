using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UngHerningSSP.Models;
public class Observation
{
    public int ID { get; set; }

    public DateTime DateAndTime { get; set; }

    public string Severity { get; set; }

    public string Behavior { get; set; }

    public string Approach { get; set; }

    public int Count { get; set; }

    public string Description { get; set; }

    public Location Location { get; set; }

    public User User { get; set; }
}
