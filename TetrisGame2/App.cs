using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;

namespace TetrisGame2
{
	public partial class App
	{
		/*******************************************************************************************************************/
		// public static
		public const int ORIGIN_X = ((int)WindowSize.Width / 2 - Stack.WIDTH * Shape.BLOCK_SIZE / 2);
		public const int ORIGIN_Y = (int)WindowSize.Height - 50;

		public static readonly Brush BackGroundBrush = Brushes.Black;

		public static Graphics Graphics { get => graphics; }

		public static int ToPointX(int locX) { return ORIGIN_X + locX * Shape.BLOCK_SIZE; }
		public static int ToPointY(int locY) { return ORIGIN_Y - locY * Shape.BLOCK_SIZE; }
		public static Point ToPoint(int locX, int locY) { return new Point(ORIGIN_X - locX, ORIGIN_Y - locY); }

		private static Graphics graphics;


		/*******************************************************************************************************************/
		// private static
		private enum WindowSize { Width = 826, Height = 686 };
		private class TransformHandler
		{
			//public
			public TransformHandler(App app) { this.app = app; }

			public void Do()
			{
				if (app.shape.Type_ == Shape.Type.O)
				{
					return;
				}

				oldShape = new Shape(app.shape);
				app.shape.Transform();

				if (IsBottomOver())
				{
					app.shape = oldShape;   // move
					return;
				}

				bool canTransform = true;
				for (int block = 0; block < Shape.BLOCK_COUNT; block++)
				{
					int blockX = app.shape.GetBlockLocX(block);
					if (blockX < 0)
					{
						canTransform = false;
						obstacleX = blockX;
						moveDirection = Side.Right;
						break;
					}
					else if (blockX >= Stack.WIDTH)
					{
						canTransform = false;
						obstacleX = blockX;
						moveDirection = Side.Left;
						break;
					}
					else
					{
						int blockY = app.shape.GetBlockLocY(block);
						if (app.stack.GetData(blockX, blockY) != null)
						{
							canTransform = false;
							obstacleX = blockX;
							moveDirection = obstacleX < app.shape.LocX ? Side.Right : Side.Left;
							break;
						}
					}
				}

				if (!canTransform)
				{
					Side obstacleSide = (Side)(-(int)moveDirection);
					app.shape.MoveSide(moveDirection);
					while (IsShapeAttachedToObstacle(obstacleSide))
					{
						canTransform = true;
						for (int blockNum = 0; blockNum < Shape.BLOCK_COUNT; blockNum++)
						{
							int blockX = app.shape.GetBlockLocX(blockNum);
							int blockY = app.shape.GetBlockLocY(blockNum);
							if (blockX < 0 || blockX >= Stack.WIDTH || app.stack.GetData(blockX, blockY) != null)
							{
								canTransform = false;
								break;
							}
						}
						if (canTransform)
						{
							return;
						}
						else
						{
							app.shape.MoveSide(moveDirection);
						}
					}

					app.shape = oldShape; // move
				}

				else // if ( canTransform ) 
				{
					return;
				}
			}
			//private
			private bool IsBottomOver()
			{
				int oldShapeBottom = oldShape.LowestBlockY;
				for (int blockNum = 0; blockNum < Shape.BLOCK_COUNT; blockNum++)
				{
					int blockX = app.shape.GetBlockLocX(blockNum);
					int blockY = app.shape.GetBlockLocY(blockNum);
					if (blockY < 0 || (blockX >= 0 && blockX < Stack.WIDTH && app.stack.GetData(blockX, blockY) != null && blockY < oldShapeBottom))
					{
						return true;
					}
				}
				return false;
			}
			private bool IsShapeAttachedToObstacle(Side side)
			{
				for (int block = 0; block < Shape.BLOCK_COUNT; block++)
				{
					int blockX = app.shape.GetBlockLocX(block);
					if (blockX + (int)side == obstacleX)
					{
						return true;
					}
				}
				return false;
			}

			private int obstacleX;
			private Side moveDirection;
			private Shape oldShape;
			private App app;
		}

		private const int NEXT_SHAPE_X = Stack.WIDTH + 5;
		private const int NEXT_SHAPE_Y = Stack.VALID_HEIGHT - 5;

		private static void DrawBackground()
		{
			int posX = ToPointX(0) - 1;
			int posY = ToPointY(0) - Stack.VALID_HEIGHT * Shape.BLOCK_SIZE;
			int width = Stack.WIDTH * Shape.BLOCK_SIZE + 1;
			int height = Stack.VALID_HEIGHT * Shape.BLOCK_SIZE + 1;

			Rectangle rect = new Rectangle(posX, posY, width, height);
			graphics.FillRectangle(BackGroundBrush, rect);
		}


		/*******************************************************************************************************************/
		// public
		public App()
		{
			InitializeComponent();

			isGameStart = false;
		}


		/*******************************************************************************************************************/
		// protected
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			graphics = e.Graphics;

			if (isGameStart)
			{
				DrawBackground();

				EraseNextShape();
				DrawNextShape();

				stack.Draw();

				shape.Draw();

				SetExpectedShape();
				expectedShape.Draw(Shape.DrawOption.Expected);
			}
		}


		/*******************************************************************************************************************/
		// private
		private bool isGameStart;
		private bool isFinished;

		private Shape shape;
		private Shape expectedShape;
		private Stack stack;
		private Mutex mutex;

		private void GameStart()
		{
			Initialize();


			// finish
			//BTN_EXIT.Show();
			//BTN_START.Show();
			//isGameStart = false;
		}

		private void Initialize()
		{
			BTN_EXIT.Hide();
			BTN_START.Hide();

			isFinished = false;

			shape = new Shape();
			expectedShape = new Shape();
			stack = new Stack();
			mutex = new Mutex();

			isGameStart = true;

			Invalidate();
			this.KeyDown += App_KeyDown;
		}

		private void EraseNextShape()
		{
			int posX = ToPointX(NEXT_SHAPE_X - 2);
			int posY = ToPointY(NEXT_SHAPE_Y + 3);

			graphics.FillRectangle(BackGroundBrush, posX, posY, Shape.BLOCK_SIZE * 5, Shape.BLOCK_SIZE * 5);
		}

		private void DrawNextShape()
		{
			new Shape(shape.NextType, shape.NextForm, NEXT_SHAPE_X, NEXT_SHAPE_Y).Draw();
		}

		private void SetExpectedShape()
		{
			expectedShape.LocX = shape.LocX;
			expectedShape.LocY = shape.LocY;
			expectedShape.Form = shape.Form;
			expectedShape.Type_ = shape.Type_;

			while (CanMoveDownShape(expectedShape))
			{
				expectedShape.MoveDown();
			}
		}

		bool CanMoveDownShape()
		{
			return CanMoveDownShape(shape);
		}
		bool CanMoveDownShape(Shape shape)
		{
			for (int block = 0; block < Shape.BLOCK_COUNT; block++)
			{
				int nextLocX = shape.GetBlockLocX(block);
				int nextLocY = shape.GetBlockLocY(block) - 1;
				if (nextLocY < 0 || stack.GetData(nextLocX, nextLocY) != null)
				{
					return false;
				}
			}
			return true;
		}

		private void BTN_START_Click(object sender, EventArgs e)
		{
			GameStart();
		}

		private void BTN_EXIT_Click(object sender, EventArgs e)
		{
			var result = MessageBox.Show("프로그램을 종료하시겠습니다?", BTN_EXIT.Text, MessageBoxButtons.OKCancel);

			if (result == DialogResult.OK)
			{
				Application.Exit();
			}
		}

		private void App_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Up:
					new TransformHandler(this).Do();
					break;
				case Keys.Down:
					break;
				case Keys.Left:
					break;
				case Keys.Right:
					break;
				case Keys.Space:
					break;
				case Keys.P:
					break;
				default:
					break;
			}
		}
	}
}