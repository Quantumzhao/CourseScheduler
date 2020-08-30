using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CourseScheduler.Core.DataStrucures
{
	public struct Location
	{
		public Location(string building, string room)
		{
			Building = building;
			Room = room;
		}

		public string Building;
		public string Room;
	}
}
