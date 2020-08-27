using System;

namespace CourseScheduler.Core.DataStrucures
{
	public struct Time
	{
		public Time(string time)
		{
			if (time.Contains(':'))
			{
				string[] hm = time.Split(':');
				int tempHour = int.Parse(hm[0]);
				int minute = int.Parse(hm[1].Substring(0, 2));
				if (hm[1][2] == 'p' && tempHour != 12)
				{
					tempHour += 12;
				}
				Hour = tempHour;
				Minute = minute;
			}
			else
			{
				Hour = int.Parse(time.Substring(0, 2));
				Minute = int.Parse(time.Substring(2, 2));
			}
		}
		public Time(int hour, int minute)
		{
			Hour = hour;
			Minute = minute;
		}

		public static implicit operator Time(string time)
		{
			return new Time(time);
		}

		public int Hour;
		public int Minute;

		public int ToMinute()
		{
			return Hour * 60 + Minute;
		}

		public static bool operator <(Time time1, Time time2)
		{
			if (time1.Hour < time2.Hour)
			{
				return true;
			}
			else if (time1.Hour > time2.Hour)
			{
				return false;
			}
			else
			{
				if (time1.Minute < time2.Minute)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		public static bool operator >(Time time1, Time time2)
		{
			if (time1.Hour > time2.Hour)
			{
				return true;
			}
			else if (time1.Hour < time2.Hour)
			{
				return false;
			}
			else
			{
				if (time1.Minute > time2.Minute)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		public static bool operator <=(Time time1, Time time2)
		{
			return time1 < time2 || time1 == time2;
		}
		public static bool operator >=(Time time1, Time time2)
		{
			return time1 > time2 || time1 == time2;
		}

		public static bool operator ==(Time time1, Time time2)
		{
			return time1.Hour == time2.Hour && time1.Minute == time2.Minute;
		}

		public static bool operator !=(Time time1, Time time2)
		{
			return time1.Hour != time2.Hour || time1.Minute != time2.Minute;
		}

		public static Time operator +(Time time, int minute)
		{
			int tempMin = time.Minute + minute;
			if (tempMin / 60 == 1)
			{
				return new Time(time.Hour + 1, tempMin - 60);
			}
			else
			{
				return new Time(time.Hour, tempMin);
			}
		}

		public override bool Equals(object obj)
		{
			return obj is Time time &&
				   Hour == time.Hour &&
				   Minute == time.Minute;
		}

		public override int GetHashCode() => HashCode.Combine(Hour, Minute);
	}
}
