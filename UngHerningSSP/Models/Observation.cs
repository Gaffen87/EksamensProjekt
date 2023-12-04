using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UngHerningSSP.Models;
public class Observation
{
    public int ObservationID { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly Time { get; set; }

    public string Severity { get; set; }

    public string Behavior { get; set; }

    public string Approach { get; set; }

    public int Count { get; set; }

    public string Description { get; set; }

    public Byte[] Image { get; set; }

    public Location Location { get; set; }

    public User user { get; set; }
}
