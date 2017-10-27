using System;

namespace Declarant.Task.Models
{
    public class EventItem : ModelBase
    {
	    public string Name { get; set; }

	    public string Desciption { get; set; }

	    public DateTime StartTime { get; set; }

	    public DateTime? EndTime { get; set; }
    }
}
