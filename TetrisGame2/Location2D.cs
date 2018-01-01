using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisGame2
{
	class Location2D
	{
		public int x;
		public int y;

		public Location2D()
		{
			x = 0;
			y = 0;
		}

		public Location2D(int _x, int _y)
		{
			x = _x;
			y = _y;
		}
	}
}
