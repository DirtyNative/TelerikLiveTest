using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelerikLiveTest
{
	public class DataPoint
	{
		public DataPoint()
		{
		}

		public DataPoint(DateTime x, double y)
		{
			X = x;
			Y = y;
		}

		public DateTime X { get; set; }

		public double Y { get; set; }
	}
}