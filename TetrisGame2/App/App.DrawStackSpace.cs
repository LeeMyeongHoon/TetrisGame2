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
		private void DrawStackSpace()
		{
			Rectangle rect = new Rectangle(MakePosX(0), MakePosY(0) - Stack.VALID_HEIGHT, Stack.WIDTH, Stack.VALID_HEIGHT);
			graphics.FillRectangle(Brushes.Black, rect);
		}
	}
}
