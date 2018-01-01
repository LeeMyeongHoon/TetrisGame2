using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace TetrisGame2
{
	public partial class App
	{
		public Point MakePoint(int x, int y)
		{
			return new Point(originX - x, originY - y);
		}
	}
}