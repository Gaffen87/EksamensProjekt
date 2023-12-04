using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UngHerningSSP.Models;
public class Schedule
{
    public int ScheduleID { get; set; }

    public string DayOfWeek { get; set; }

    public bool IsVisible { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }
}
