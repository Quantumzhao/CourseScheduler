using System;
using System.Collections.Generic;
using System.Text;
using UIEngine;

namespace CourseScheduler.Core.DataStrucures
{
	public class ClassSpan
	{
		public ClassSpan(Time start, Time end)
		{
			Start = start;
			End = end;
		}

		[Visible(nameof(Start))]
		public Time Start { get; }
		[Visible(nameof(End))]
		public Time End { get; }

		public int SpanInMinute()
		{
			return End.ToMinute() - Start.ToMinute();
		}

		public bool IsOverlap(ClassSpan another)
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

		public override string ToString() => $"{Start} - {End}";
	}
}
