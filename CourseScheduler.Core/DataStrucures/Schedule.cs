using System;
using System.Collections.Generic;
using System.Text;

namespace CourseScheduler.Core.DataStrucures
{
	public class Schedule
	{
		public Schedule(Time start, Time end)
		{
			Start = start;
			End = end;
		}

		public readonly Time Start;
		public readonly Time End;

		public int SpanInMinute()
		{
			return End.ToMinute() - Start.ToMinute();
		}

		public bool IsOverlap(Schedule another)
		{
			if (another.Start > Start && another.Start < End)
			{
				return true;
			}
			else if (another.End > Start && another.End < End)
			{
				return true;
			}
			else if (another.Start <= Start && another.End >= End)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
