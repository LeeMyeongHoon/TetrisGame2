using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;

namespace TetrisGame2
{
	public partial class App
	{
		// public static
		public const int ORIGIN_X = (WINDOW_WIDTH / 2 - Stack.WIDTH / 2);
		public const int ORIGIN_Y = WINDOW_HEIGHT - 50;

		public static Graphics graphics;

		public static int ToPointX(int locX) => ORIGIN_X + locX;
		public static int ToPointY(int locY) => ORIGIN_Y - locY;
		public static Point ToPoint(int locX, int locY) => new Point(ORIGIN_X - locX, ORIGIN_Y - locY);


		// private static
		private const int WINDOW_WIDTH = 586;
		private const int WINDOW_HEIGHT = 686;


		// public
		public App()
		{
			InitializeComponent();

			Debug.Assert(WINDOW_WIDTH == ClientSize.Width && WINDOW_HEIGHT == ClientSize.Height, "사이즈 동기화 에러");

			needDrawingObjects = false;
			graphics = CreateGraphics();

			shape = new Shape();
			stack = new Stack();
			mutex = new Mutex();
		}


		// protected
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if(needDrawingObjects)
			{
				DrawBackGround();
				shape.Draw();
			}
		}


		// private
		private bool needDrawingObjects;
		private bool isFinished;

		private Shape shape;
		private Stack stack;

		private Mutex mutex;

		private static void DrawBackGround()
		{
			Rectangle rect = new Rectangle(ToPointX(0), ToPointY(0) - Stack.VALID_HEIGHT, Stack.WIDTH, Stack.VALID_HEIGHT);
			graphics.FillRectangle(Brushes.Black, rect);
		}

		private void InitGameData()
		{
			needDrawingObjects = false;
			isFinished = false;

			shape.Reset();
		}

		private void BTN_START_Click(object sender, EventArgs e)
		{
			BTN_EXIT.Hide();
			BTN_START.Hide();

			InitGameData();
			DrawBackGround();

			shape.Draw();
		}

		private void BTN_EXIT_Click(object sender, EventArgs e)
		{
			var result = MessageBox.Show("프로그램을 종료하시겠습니다?", BTN_EXIT.Text, MessageBoxButtons.OKCancel);

			if (result == DialogResult.OK)
			{
				Application.Exit();
			}
		}
	}
}