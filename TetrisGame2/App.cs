using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace TetrisGame2
{
	public partial class App
	{
		/*******************************************************************************************************************/
		// public static
		public const int ORIGIN_X = ((int)WindowSize.Width / 2 - Stack.WIDTH * Shape.BLOCK_SIZE / 2);
		public const int ORIGIN_Y = (int)WindowSize.Height - 50;

		public static readonly Brush BACKGROUND_BRUSH = Brushes.Black;

		/*******************************************************************************************************************/
		// private static
		private class TransformHandler
		{
			//public
			public TransformHandler(App app) { this.app = app; }

			public void Do()
			{
				if (app.curShape.Type_ == Shape.Type.O)
				{
					return;
				}

				oldShape = new Shape(app.curShape);
				app.curShape.Transform();

				if (IsBottomOver())
				{
					app.curShape = oldShape;   // move
					return;
				}

				bool isSideOver = false;
				for (int block = 0; block < Shape.BLOCK_COUNT; block++)
				{
					int blockX = app.curShape.GetBlockLocX(block);
					if (blockX < 0)
					{
						isSideOver = true;
						obstacleX = blockX;
						moveDirection = Side.Right;
						break;
					}
					else if (blockX >= Stack.WIDTH)
					{
						isSideOver = true;
						obstacleX = blockX;
						moveDirection = Side.Left;
						break;
					}
					else
					{
						int blockY = app.curShape.GetBlockLocY(block);
						if (app.stack.GetData(blockX, blockY) != null)
						{
							isSideOver = true;
							obstacleX = blockX;
							moveDirection = obstacleX < app.curShape.LocX ? Side.Right : Side.Left;
							break;
						}
					}
				}

				if (isSideOver)
				{

					Side obstacleSide = (Side)(-(int)moveDirection);
					app.curShape.MoveSide(moveDirection);
					while (IsShapeAttachedToObstacle(obstacleSide))
					{
						bool canTransform = true;
						for (int blockNum = 0; blockNum < Shape.BLOCK_COUNT; blockNum++)
						{
							int blockX = app.curShape.GetBlockLocX(blockNum);
							int blockY = app.curShape.GetBlockLocY(blockNum);
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
							app.curShape.MoveSide(moveDirection);
						}
					}

					app.curShape = oldShape; // move
				}

				else // if ( !isSideOver ) 
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
					int blockX = app.curShape.GetBlockLocX(blockNum);
					int blockY = app.curShape.GetBlockLocY(blockNum);
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
					int blockX = app.curShape.GetBlockLocX(block);
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

		private enum WindowSize : int { Width = 826, Height = 686 };

		private const int NEXT_SHAPE_X = Stack.WIDTH + 5;
		private const int NEXT_SHAPE_Y = Stack.VALID_HEIGHT - 5;

		private static readonly Brush[] BRUSHES_ON_SHAPE;
		private static readonly int BOUNDARY_POS_Y;
		static App()
		{
			#region InitBrushData()

			BRUSHES_ON_SHAPE = new Brush[(int)Shape.Type.COUNT];

			BRUSHES_ON_SHAPE[(int)Shape.Type.J] = Brushes.Red;
			BRUSHES_ON_SHAPE[(int)Shape.Type.L] = Brushes.Orange;
			BRUSHES_ON_SHAPE[(int)Shape.Type.l] = Brushes.Yellow;
			BRUSHES_ON_SHAPE[(int)Shape.Type.O] = Brushes.LightGreen;
			BRUSHES_ON_SHAPE[(int)Shape.Type.S] = Brushes.LightBlue;
			BRUSHES_ON_SHAPE[(int)Shape.Type.Z] = Brushes.LightPink;
			BRUSHES_ON_SHAPE[(int)Shape.Type.T] = Brushes.Blue;

			#endregion

			BOUNDARY_POS_Y = ToPointY(Stack.VALID_HEIGHT);
		}


		private static void Delay(int miliseconds)
		{
			// 인터넷에서 퍼온 함수....
			DateTime ThisMoment = DateTime.Now;
			DateTime AfterWards = ThisMoment.Add(new TimeSpan(0, 0, 0, 0, miliseconds));

			while (AfterWards >= ThisMoment)
			{
				System.Windows.Forms.Application.DoEvents();
				ThisMoment = DateTime.Now;
			}
		}

		private static Brush GetBrush(Shape shape)
		{
			return BRUSHES_ON_SHAPE[(int)shape.Type_];
		}

		public static int ToPointX(int locX) { return ORIGIN_X + locX * Shape.BLOCK_SIZE; }
		public static int ToPointY(int locY) { return ORIGIN_Y - locY * Shape.BLOCK_SIZE; }
		public static Point ToPoint(int locX, int locY) { return new Point(ToPointX(locX), ToPointY(locY)); }



		/*******************************************************************************************************************/
		// public
		public App()
		{
			InitializeComponent();
			shouldDrawObjects = false;
			this.FormClosed += AppExitHandler;
			this.MaximizeBox = false;
		}


		/*******************************************************************************************************************/
		// protected
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (shouldDrawObjects)
			{
				EraseNextShape(); // 검은색 공간 채우는 목적
				DrawNextShape();
				DrawStackBackground();

				DrawStack();
				DrawCurShape();
				DrawExpectedShape();
			}
		}


		/*******************************************************************************************************************/
		// private
		private bool isGameOver;
		private bool shouldDrawObjects;

		private Shape curShape;
		private Shape expectedShape;
		private Shape nextShape;
		private Stack stack;

		private Graphics graphics;

		private void GameStart()
		{
			Initialize();

			FinishGame();
		}

		private void Initialize()
		{
			#region HideBottons()

			BTN_START.Enabled = false;
			BTN_EXIT.Enabled = false;

			BTN_START.Hide();
			BTN_EXIT.Hide();

			this.Focus();

			#endregion

			#region InitData()

			curShape = new Shape();
			expectedShape = new Shape();
			nextShape = MakeNextShape();
			stack = new Stack();

			graphics = CreateGraphics();

			isGameOver = false;
			shouldDrawObjects = true;

			#endregion

			#region DrawObjects()

			EraseUpperSpace();

			DrawCurShape();
			DrawExpectedShape();
			DrawStackBackground();

			#endregion

			ReceiveGameKey();

			MoveDownShapeAutomacally();
		}

		private void ReceiveGameKey()
		{
			this.KeyPreview = true;
			this.KeyDown += GameKeyHandler;
		}


		
		private void MoveDownShapeAutomacally()
		{
			Timer shapeDownTimer = new Timer
			{
				Interval = 400
			};

			shapeDownTimer.Tick += DownKeyHandler;

			shapeDownTimer.Start();

			while (!isGameOver)
			{
				Application.DoEvents();
			}
			shapeDownTimer.Stop();

			shapeDownTimer.Tick -= DownKeyHandler;

			shapeDownTimer.Dispose();
		}

		private void EraseUpperSpace()
		{
			int posX = ToPointX(0);
			int posY = ToPointY(Stack.VALID_HEIGHT + 4);

			graphics.FillRectangle(Brushes.MidnightBlue, posX, posY, Stack.WIDTH * Shape.BLOCK_SIZE, Shape.BLOCK_SIZE * 4);
		}

		private void FinishGame()
		{
			KeyPreview = false;
			this.KeyDown -= GameKeyHandler;

			MessageBox.Show("Game Over", "Game Over");

			shouldDrawObjects = false;

			EraseAll();

			graphics.Dispose();

			#region ShowButtons()

			BTN_EXIT.Show();
			BTN_START.Show();
			BTN_START.Enabled = true;
			BTN_EXIT.Enabled = true;

			#endregion
		}

		private void SetExpectedShape()
		{
			expectedShape.LocX = curShape.LocX;
			expectedShape.LocY = curShape.LocY;
			expectedShape.Form = curShape.Form;
			expectedShape.Type_ = curShape.Type_;

			while (CanMoveDownShape(expectedShape))
			{
				expectedShape.MoveDown();
			}
		}

		private void PushCurShapeToStack()
		{
			stack.PushShape(curShape, GetBrush(curShape));

			//  shape의 블럭부분에서만 breakRow가 발생되므로
			int maxShapeBlockY = curShape.LocY + Shape.MAX_BLOCK_UP_OFFSET;
			int minShapeBlockY = curShape.LocY - Shape.MAX_BLOCK_DOWN_OFFSET;
			if (minShapeBlockY < 0)
			{
				minShapeBlockY = 0;
			}

			// FullRow가 존재하는지 확인
			int y;
			if ((y = stack.FindFullRow(minShapeBlockY, maxShapeBlockY)) != -1)
			{
				int oldHighestY = stack.HighestBlockY;
				stack.BreakRow(y);
				maxShapeBlockY--;

				int[] brokenRows = new int[Shape.BLOCK_COUNT];
				brokenRows[0] = y;

				int brokenRowCount = 1;

				// FullRow계속찾기
				while ((y = stack.FindFullRow(y, maxShapeBlockY)) != -1)
				{
					stack.BreakRow(y);
					brokenRows[brokenRowCount++] = y + brokenRowCount; // 알고리즘 stack.BreakRow(y); 코드로 인해 offset 다시 계산해야함.
					maxShapeBlockY--;
				}

				EraseStack(brokenRows[0], oldHighestY);
				DrawStack(brokenRows[0]);
			}


			if (stack.IsFull())
			{
				isGameOver = true;
				return;
			}

			curShape = nextShape;
			curShape.InitLocation();
			DrawCurShape();

			EraseNextShape();
			nextShape = MakeNextShape();
			DrawNextShape();

			SetExpectedShape();
			DrawExpectedShape();
		}

		private bool CanMoveSideShape(Side sideOffset)
		{
			for (int blockNum = 0; blockNum < Shape.BLOCK_COUNT; blockNum++)
			{
				int blockX = curShape.GetBlockLocX(blockNum);
				int blockY = curShape.GetBlockLocY(blockNum);

				int nextBlockX = blockX + (int)sideOffset;
				int nextBlockY = blockY;
				if (nextBlockX < 0 || nextBlockX >= Stack.WIDTH || stack.GetData(nextBlockX, nextBlockY) != null)
				{
					return false;
				}
			}
			return true;
		}

		private bool CanMoveDownCurShape()
		{
			return CanMoveDownShape(curShape);
		}

		private bool CanMoveDownShape(Shape shape)
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

		private Shape MakeNextShape()
		{
			return new Shape(Shape.MakeRandomType(), Shape.MakeRandomForm(), NEXT_SHAPE_X, NEXT_SHAPE_Y);
		}

		private void ProcGameUpKey()
		{
			EraseCurShape();
			EraseExpecatedShape();
			new TransformHandler(this).Do();
			DrawCurShape();
			DrawExpectedShape();
		}

		private void ProcGameSpaceKey()
		{
			this.KeyPreview = false;
			this.KeyDown -= GameKeyHandler;

			while (CanMoveDownCurShape())
			{
				EraseCurShape();
				curShape.MoveDown();
				DrawCurShape();
				Delay(5);
			}
			PushCurShapeToStack();

			this.KeyPreview = true;
			this.KeyDown += GameKeyHandler;
		}

		private void ProcGameDownKey()
		{
			if (CanMoveDownCurShape())
			{
				EraseCurShape();
				curShape.MoveDown();
				DrawCurShape();
			}
			else
			{
				PushCurShapeToStack();
			}
		}

		private void ProcGameSideKey(Side side)
		{
			if (CanMoveSideShape(side))
			{
				EraseExpecatedShape();
				EraseCurShape();
				curShape.MoveSide(side);
				DrawCurShape();
				DrawExpectedShape();
			}
		}

		private void DrawStack(int beginY = 0)
		{
			for (int x = 0; x < Stack.WIDTH; x++)
			{
				for (int y = beginY; y <= stack.HighestBlockY; y++)
				{
					if (stack.GetData(x, y) != null)
					{
						int posX = App.ToPointX(x);
						int posY = App.ToPointY(y) - Shape.BLOCK_SIZE;
						graphics.FillRectangle(stack.GetData(x, y), posX, posY, Shape.BLOCK_SIZE, Shape.BLOCK_SIZE);
						graphics.DrawRectangle(Pens.Gray, posX, posY, Shape.BLOCK_SIZE, Shape.BLOCK_SIZE);
					}
				}
			}
		}
        private void EraseStack(int beginY, int lastY)
        {
            for (int y = beginY; y <= lastY; y++)
            {
                int posX = App.ToPointX(0);
                int posY = App.ToPointY(y) - Shape.BLOCK_SIZE;
                graphics.FillRectangle(App.BACKGROUND_BRUSH, posX, posY, Stack.WIDTH * Shape.BLOCK_SIZE, Shape.BLOCK_SIZE);
                graphics.DrawRectangle(Pens.Black, posX, posY, Stack.WIDTH * Shape.BLOCK_SIZE, Shape.BLOCK_SIZE);
            }

        }
		private void DrawCurShape()
		{
			DrawShape(curShape);
		}
		private void EraseCurShape()
		{
			EraseShape(curShape);
		}

		private void DrawExpectedShape()
		{
			SetExpectedShape();

			int offset = 1;

			for (int block = 0; block < Shape.BLOCK_COUNT; ++block)
			{

				int x = expectedShape.GetBlockPosX(block) + offset;
				int y = expectedShape.GetBlockPosY(block) - Shape.BLOCK_SIZE + offset;

				int expectedBlockSize = Shape.BLOCK_SIZE - 2 * offset;

				graphics.DrawRectangle(Pens.White, x, y, expectedBlockSize, expectedBlockSize); // 다른 블럭이 그린 선의 영향이 않가게끔 조정
			}
		}
		private void EraseExpecatedShape()
		{
			int offset = 1;
			int expectedBlockSize = Shape.BLOCK_SIZE - 2 * offset;

			for (int block = 0; block < Shape.BLOCK_COUNT; ++block)
			{
				int x = expectedShape.GetBlockPosX(block) + offset;
				int y = expectedShape.GetBlockPosY(block) - Shape.BLOCK_SIZE + offset;

				if (y < BOUNDARY_POS_Y)
				{
					graphics.FillRectangle(Brushes.MidnightBlue, x, y, expectedBlockSize, expectedBlockSize);
					graphics.DrawRectangle(Pens.MidnightBlue, x, y, expectedBlockSize, expectedBlockSize);
				}
				else
				{
					graphics.FillRectangle(BACKGROUND_BRUSH, x, y, expectedBlockSize, expectedBlockSize);
					graphics.DrawRectangle(Pens.Black, x, y, expectedBlockSize, expectedBlockSize);
				}
			}
		}

		private void DrawNextShape()
		{
			DrawShape(nextShape);
		}
		private void EraseNextShape()
		{
			int posX = ToPointX(NEXT_SHAPE_X - 2);
			int posY = ToPointY(NEXT_SHAPE_Y + 3);

			graphics.FillRectangle(BACKGROUND_BRUSH, posX, posY, Shape.BLOCK_SIZE * 5 + 1, Shape.BLOCK_SIZE * 5);
		}

		private void DrawShape(Shape shape)
		{
			for (int block = 0; block < Shape.BLOCK_COUNT; ++block)
			{
				int x = shape.GetBlockPosX(block);
				int y = shape.GetBlockPosY(block) - Shape.BLOCK_SIZE;

				graphics.FillRectangle(GetBrush(shape), x, y, Shape.BLOCK_SIZE, Shape.BLOCK_SIZE);
				graphics.DrawRectangle(Pens.Gray, x, y, Shape.BLOCK_SIZE, Shape.BLOCK_SIZE);
			}
		}
		private void EraseShape(Shape shape)
		{
			for (int block = 0; block < Shape.BLOCK_COUNT; ++block)
			{
				int x = shape.GetBlockPosX(block);
				int y = shape.GetBlockPosY(block) - Shape.BLOCK_SIZE;

				if (y < BOUNDARY_POS_Y)
				{
					graphics.FillRectangle(Brushes.MidnightBlue, x, y, Shape.BLOCK_SIZE, Shape.BLOCK_SIZE);
					graphics.DrawRectangle(Pens.MidnightBlue, x, y, Shape.BLOCK_SIZE, Shape.BLOCK_SIZE);
				}
				else
				{
					graphics.FillRectangle(BACKGROUND_BRUSH, x, y, Shape.BLOCK_SIZE, Shape.BLOCK_SIZE);
					graphics.DrawRectangle(Pens.Black, x, y, Shape.BLOCK_SIZE, Shape.BLOCK_SIZE);
				}
			}
		}

		private void DrawStackBackground()
		{
			Graphics g = CreateGraphics();

			int posX = ToPointX(0) - 1;
			int posY = ToPointY(0) - Stack.VALID_HEIGHT * Shape.BLOCK_SIZE;
			int width = Stack.WIDTH * Shape.BLOCK_SIZE + 1;
			int height = Stack.VALID_HEIGHT * Shape.BLOCK_SIZE + 1;

			Rectangle rect = new Rectangle(posX, posY, width + 1, height);
			g.FillRectangle(BACKGROUND_BRUSH, rect);

			g.Dispose();
		}

		private void EraseAll()
		{
			graphics.FillRectangle(Brushes.MidnightBlue, 0, 0, (int)WindowSize.Width - 1, (int)WindowSize.Height - 1);
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

		private void GameKeyHandler(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Up:
					{
						ProcGameUpKey();
						break;
					}

				case Keys.Down:
					{
						ProcGameDownKey();
						break;
					}

				case Keys.Left:
					{
						ProcGameSideKey(Side.Left);
						break;
					}

				case Keys.Right:
					{
						ProcGameSideKey(Side.Right);
						break;
					}

				case Keys.Space:
					{
						e.Handled = true;
						ProcGameSpaceKey();
						break;
					}

				case Keys.P:
					{
						MessageBox.Show("PAUSE", "PAUSE");
						break;
					}

				default:
					break;
			}
		}

		private void DownKeyHandler(object sender, EventArgs e)
		{
			ProcGameDownKey();
		}

		private void AppExitHandler(object sender, FormClosedEventArgs e)
		{
			isGameOver = true;
			Application.Exit();
		}
	}
}