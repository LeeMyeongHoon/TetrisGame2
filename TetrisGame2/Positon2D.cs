using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisGame2
{
	class Pos2D
	{
		public int x;
		public int y;

		public Pos2D()
		{
			x = 0;
			y = 0;
		}

		public Pos2D(int _x, int _y)
		{
			x = _x;
			y = _y;
		}
	}
}
