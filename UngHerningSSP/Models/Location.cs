using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UngHerningSSP.Models;
public class Location
{
    public int ID { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string Address { get; set; }

    public string AddressNum { get; set; }

    public int PostalCode { get; set; }

    public string City { get; set; }
}
