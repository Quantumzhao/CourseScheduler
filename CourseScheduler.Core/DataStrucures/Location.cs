using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using UIEngine;
using UIEngine.Core;

namespace CourseScheduler.Core.DataStrucures
{
	public class Location
	{
		public Location(string building, string room)
		{
			Building = building;
			Room = room;
		}

		[Visible(nameof(Building))]
		public string Building { get; set; }
		[Visible(nameof(Room))]
		public string Room { get; set; }
	}
}
