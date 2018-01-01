using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace TetrisGame2
{
	partial class Stack
	{
		public const int WIDTH = 10 * App.BLOCK_SIZE;
		public const int VALID_HEIGHT = 20 * App.BLOCK_SIZE;
		public const int OVER_HEIGHT = VALID_HEIGHT + (Shape.MAX_BLOCK_UP_OFFSET + 3) * App.BLOCK_SIZE;
	}
}
