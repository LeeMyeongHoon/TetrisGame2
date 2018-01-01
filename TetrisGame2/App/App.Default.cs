using System.Drawing;
using System.Windows.Forms;

namespace TetrisGame2
{
	partial class App
	{
		// public
		public const int BLOCK_SIZE = 26;


		// private
		private int originX;
		private int originY;

		private bool hasStack;
		private bool hasShape;

		private Graphics graphics;


		// public
		public App()
		{
			InitializeComponent();

			originX = (ClientSize.Width / 2 - Stack.WIDTH / 2);
			originY = ClientSize.Height - 50;

			hasShape = false;
			hasStack = false;

			graphics = CreateGraphics();
		}

		~App()
		{
			graphics.Dispose();
		}


		// private


		// property
		public int OriginX { get => originX; }
		public int OriginY { get => originY; }
	}
}