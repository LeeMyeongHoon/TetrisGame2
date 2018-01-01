using System;
using System.Drawing;
using System.Diagnostics;

namespace TetrisGame2
{
	public class Shape
	{
		//public static
		public enum Type
		{
			S, Z, J, L, T, O, l, COUNT
		};
		public enum DrawOption { DEFAULT, EXPECTED, ERASE }

		public const int BLOCK_SIZE = 26;
		public const int MAX_BLOCK_DOWN_OFFSET = 1 * BLOCK_SIZE;
		public const int MAX_BLOCK_UP_OFFSET = 2 * BLOCK_SIZE;
		public const int BLOCK_COUNT = 4;
		public const int FORM_COUNT = 4;

		// private static

		private static readonly int[] WEIGHT;
		private static readonly int[] WEIGHT_SUM;

		private static readonly Location2D[,,] BLOCK_OFFSET_DATA; // Type,Form,Block

		private static int MakeRandomForm()
		{
			Random random = new Random();
			return random.Next(0, FORM_COUNT);
		}
		private static Type MakeRandomType()
		{
			Random random = new Random();
			var result = random.Next(0, WEIGHT_SUM[(int)Type.COUNT - 1]);

			for (int type = 0; type < (int)Type.COUNT; ++type)
			{
				if (result < WEIGHT_SUM[type])
				{
					return (Type)type;
				}
			}

			Debug.Fail("MakeRandomType() Error");

			return Type.COUNT;
		}

		static Shape()
		{
			#region InitRandomTypeData()

			WEIGHT = new int[(int)Type.COUNT];

			WEIGHT[(int)Type.S] = 2;
			WEIGHT[(int)Type.Z] = 2;
			WEIGHT[(int)Type.J] = 2;
			WEIGHT[(int)Type.L] = 2;
			WEIGHT[(int)Type.T] = 4;
			WEIGHT[(int)Type.O] = 2;
			WEIGHT[(int)Type.l] = 5;

			WEIGHT_SUM = new int[(int)Type.COUNT];

			WEIGHT_SUM[0] = 2;
			for (int i = 1; i < (int)Type.COUNT; ++i)
			{
				WEIGHT_SUM[i] = WEIGHT_SUM[i - 1] + WEIGHT[i];
			}

			#endregion();

			#region InitBlockOffsetData();
			BLOCK_OFFSET_DATA = new Location2D[(int)Type.COUNT, FORM_COUNT, BLOCK_COUNT];

			int type;
			int form;

			type = (int)Type.J;

			//   [0]
			//	 [1]
			//[2][3]
			form = 0;
			BLOCK_OFFSET_DATA[type, form, 0] = new Location2D(+1, +1);
			BLOCK_OFFSET_DATA[type, form, 1] = new Location2D(+1, +0);
			BLOCK_OFFSET_DATA[type, form, 2] = new Location2D(+0, -1);
			BLOCK_OFFSET_DATA[type, form, 3] = new Location2D(+1, -1);

			//[0]
			//[1][2][3]
			form = 1;
			BLOCK_OFFSET_DATA[type, form, 0] = new Location2D(-1, +0);
			BLOCK_OFFSET_DATA[type, form, 1] = new Location2D(-1, -1);
			BLOCK_OFFSET_DATA[type, form, 2] = new Location2D(+0, -1);
			BLOCK_OFFSET_DATA[type, form, 3] = new Location2D(+1, -1);

			//[0][1]
			//[2]
			//[3]
			form = 2;
			BLOCK_OFFSET_DATA[type, form, 0] = new Location2D(-1, +1);
			BLOCK_OFFSET_DATA[type, form, 1] = new Location2D(+0, +1);
			BLOCK_OFFSET_DATA[type, form, 2] = new Location2D(-1, +0);
			BLOCK_OFFSET_DATA[type, form, 3] = new Location2D(-1, -1);

			//[0][1][2]
			//      [3]
			form = 3;
			BLOCK_OFFSET_DATA[type, form, 0] = new Location2D(-1, +1);
			BLOCK_OFFSET_DATA[type, form, 1] = new Location2D(+0, +1);
			BLOCK_OFFSET_DATA[type, form, 2] = new Location2D(+1, +1);
			BLOCK_OFFSET_DATA[type, form, 3] = new Location2D(+1, +0);



			type = (int)Type.L;

			//[0]
			//[1]
			//[2][3]
			form = 0;
			BLOCK_OFFSET_DATA[type, form, 0] = new Location2D(-1, +1);
			BLOCK_OFFSET_DATA[type, form, 1] = new Location2D(-1, +0);
			BLOCK_OFFSET_DATA[type, form, 2] = new Location2D(-1, -1);
			BLOCK_OFFSET_DATA[type, form, 3] = new Location2D(+0, -1);

			//[1][2][3]
			//[0]
			form = 1;
			BLOCK_OFFSET_DATA[type, form, 0] = new Location2D(-1, +0);
			BLOCK_OFFSET_DATA[type, form, 1] = new Location2D(-1, +1);
			BLOCK_OFFSET_DATA[type, form, 2] = new Location2D(+0, +1);
			BLOCK_OFFSET_DATA[type, form, 3] = new Location2D(+1, +1);

			//[0][1]
			//   [2]
			//	 [3]
			form = 2;
			BLOCK_OFFSET_DATA[type, form, 0] = new Location2D(+0, +1);
			BLOCK_OFFSET_DATA[type, form, 1] = new Location2D(+1, +1);
			BLOCK_OFFSET_DATA[type, form, 2] = new Location2D(+1, +0);
			BLOCK_OFFSET_DATA[type, form, 3] = new Location2D(+1, -1);

			//      [3]
			//[0][1][2]
			form = 3;
			BLOCK_OFFSET_DATA[type, form, 0] = new Location2D(-1, -1);
			BLOCK_OFFSET_DATA[type, form, 1] = new Location2D(+0, -1);
			BLOCK_OFFSET_DATA[type, form, 2] = new Location2D(+1, -1);
			BLOCK_OFFSET_DATA[type, form, 3] = new Location2D(+1, +0);


			type = (int)Type.Z;

			//[0][1]
			//   [2][3]
			// form 0, form 2
			BLOCK_OFFSET_DATA[type, 0, 0] = BLOCK_OFFSET_DATA[type, 2, 0] = new Location2D(-1, +1);
			BLOCK_OFFSET_DATA[type, 0, 1] = BLOCK_OFFSET_DATA[type, 2, 1] = new Location2D(+0, +1);
			BLOCK_OFFSET_DATA[type, 0, 2] = BLOCK_OFFSET_DATA[type, 2, 2] = new Location2D(+0, +0);
			BLOCK_OFFSET_DATA[type, 0, 3] = BLOCK_OFFSET_DATA[type, 2, 3] = new Location2D(+1, +0);

			//   [0]
			//[1][2]
			//[3]
			// form 1, form 3
			BLOCK_OFFSET_DATA[type, 1, 0] = BLOCK_OFFSET_DATA[type, 3, 0] = new Location2D(+0, +1);
			BLOCK_OFFSET_DATA[type, 1, 1] = BLOCK_OFFSET_DATA[type, 3, 1] = new Location2D(-1, +0);
			BLOCK_OFFSET_DATA[type, 1, 2] = BLOCK_OFFSET_DATA[type, 3, 2] = new Location2D(+0, +0);
			BLOCK_OFFSET_DATA[type, 1, 3] = BLOCK_OFFSET_DATA[type, 3, 3] = new Location2D(-1, -1);


			type = (int)Type.l;

			//[0]
			//[1]
			//[2]
			//[3]
			//form 0, form 2
			BLOCK_OFFSET_DATA[type, 0, 0] = BLOCK_OFFSET_DATA[type, 2, 0] = new Location2D(+0, +2);
			BLOCK_OFFSET_DATA[type, 0, 1] = BLOCK_OFFSET_DATA[type, 2, 1] = new Location2D(+0, +1);
			BLOCK_OFFSET_DATA[type, 0, 2] = BLOCK_OFFSET_DATA[type, 2, 2] = new Location2D(+0, +0);
			BLOCK_OFFSET_DATA[type, 0, 3] = BLOCK_OFFSET_DATA[type, 2, 3] = new Location2D(+0, -1);

			//[0][1][2][3]
			//form 1, form 3
			BLOCK_OFFSET_DATA[type, 1, 0] = BLOCK_OFFSET_DATA[type, 3, 0] = new Location2D(-1, -1);
			BLOCK_OFFSET_DATA[type, 1, 1] = BLOCK_OFFSET_DATA[type, 3, 1] = new Location2D(+0, -1);
			BLOCK_OFFSET_DATA[type, 1, 2] = BLOCK_OFFSET_DATA[type, 3, 2] = new Location2D(+1, -1);
			BLOCK_OFFSET_DATA[type, 1, 3] = BLOCK_OFFSET_DATA[type, 3, 3] = new Location2D(+2, -1);


			type = (int)Type.O;

			//[2][3]
			//[0][1]
			//fomr 0,1,2,3
			BLOCK_OFFSET_DATA[type, 0, 0] = BLOCK_OFFSET_DATA[type, 1, 0] = BLOCK_OFFSET_DATA[type, 2, 0] = BLOCK_OFFSET_DATA[type, 3, 0] = new Location2D(-1, -1);
			BLOCK_OFFSET_DATA[type, 0, 1] = BLOCK_OFFSET_DATA[type, 1, 1] = BLOCK_OFFSET_DATA[type, 2, 1] = BLOCK_OFFSET_DATA[type, 3, 1] = new Location2D(+0, -1);
			BLOCK_OFFSET_DATA[type, 0, 2] = BLOCK_OFFSET_DATA[type, 1, 2] = BLOCK_OFFSET_DATA[type, 2, 2] = BLOCK_OFFSET_DATA[type, 3, 2] = new Location2D(-1, +0);
			BLOCK_OFFSET_DATA[type, 0, 3] = BLOCK_OFFSET_DATA[type, 1, 3] = BLOCK_OFFSET_DATA[type, 2, 3] = BLOCK_OFFSET_DATA[type, 3, 3] = new Location2D(+0, +0);

			type = (int)Type.S;

			//   [0][1]
			//[2][3]
			// form 0, form 2
			BLOCK_OFFSET_DATA[type, 0, 0] = BLOCK_OFFSET_DATA[type, 2, 0] = new Location2D(+0, +0);
			BLOCK_OFFSET_DATA[type, 0, 1] = BLOCK_OFFSET_DATA[type, 2, 1] = new Location2D(+1, +0);
			BLOCK_OFFSET_DATA[type, 0, 2] = BLOCK_OFFSET_DATA[type, 2, 2] = new Location2D(-1, -1);
			BLOCK_OFFSET_DATA[type, 0, 3] = BLOCK_OFFSET_DATA[type, 2, 3] = new Location2D(+0, -1);

			//[0]
			//[1][2]
			//   [3]
			// form 1, form 3
			BLOCK_OFFSET_DATA[type, 1, 0] = BLOCK_OFFSET_DATA[type, 3, 0] = new Location2D(-1, +1);
			BLOCK_OFFSET_DATA[type, 1, 1] = BLOCK_OFFSET_DATA[type, 3, 1] = new Location2D(-1, +0);
			BLOCK_OFFSET_DATA[type, 1, 2] = BLOCK_OFFSET_DATA[type, 3, 2] = new Location2D(+0, +0);
			BLOCK_OFFSET_DATA[type, 1, 3] = BLOCK_OFFSET_DATA[type, 3, 3] = new Location2D(+0, -1);

			type = (int)Type.T;

			//   [0]
			//[1][2][3]
			form = 0;
			BLOCK_OFFSET_DATA[type, form, 0] = new Location2D(+0, +0);
			BLOCK_OFFSET_DATA[type, form, 1] = new Location2D(-1, -1);
			BLOCK_OFFSET_DATA[type, form, 2] = new Location2D(+0, -1);
			BLOCK_OFFSET_DATA[type, form, 3] = new Location2D(+1, -1);

			//[0]
			//[1][2]
			//[3]
			form = 1;
			BLOCK_OFFSET_DATA[type, form, 0] = new Location2D(-1, +1);
			BLOCK_OFFSET_DATA[type, form, 1] = new Location2D(-1, +0);
			BLOCK_OFFSET_DATA[type, form, 2] = new Location2D(+0, +0);
			BLOCK_OFFSET_DATA[type, form, 3] = new Location2D(-1, -1);

			//[1][2][3]
			//   [0]
			form = 2;
			BLOCK_OFFSET_DATA[type, form, 0] = new Location2D(+0, +0);
			BLOCK_OFFSET_DATA[type, form, 1] = new Location2D(-1, +1);
			BLOCK_OFFSET_DATA[type, form, 2] = new Location2D(+0, +1);
			BLOCK_OFFSET_DATA[type, form, 3] = new Location2D(+1, +1);

			//   [0]
			//[1][2]
			//   [3]
			form = 3;
			BLOCK_OFFSET_DATA[type, form, 0] = new Location2D(+1, +1);
			BLOCK_OFFSET_DATA[type, form, 1] = new Location2D(+0, +0);
			BLOCK_OFFSET_DATA[type, form, 2] = new Location2D(+1, +0);
			BLOCK_OFFSET_DATA[type, form, 3] = new Location2D(+1, -1);
			#endregion
		}

		// public 
		public int PosX { get => App.ToPointX(locX); }
		public int PosY { get => App.ToPointY(locY); }
		public int LocX { get => locX; set => locX = value; }
		public int LocY { get => locY; set => locY = value; }

		public Shape()
		{
			nextType = MakeRandomType();
			nextForm = MakeRandomForm();
			Reset();
		}

		public Shape(Graphics g, Type _type, int _form, int _LocX, int _LocY)
			: this()
		{
			type = _type;
			form = _form;
			locX = _LocX;
			LocY = _LocY;

			nextType = MakeRandomType();
			nextForm = MakeRandomForm();
		}

		public void Reset()
		{
			locX = Stack.WIDTH / 2;
			LocY = Stack.VALID_HEIGHT + Shape.MAX_BLOCK_DOWN_OFFSET;

			type = nextType;
			form = nextForm;

			nextType = MakeRandomType();
			nextForm = MakeRandomForm();
		}

		public void Draw(DrawOption drawOption = DrawOption.DEFAULT)
		{
			for (int block = 0; block < BLOCK_COUNT; ++block)
			{
				Brush brush;
				Pen pens;

				if (drawOption == DrawOption.DEFAULT)
				{
					brush = BrushOnType;
					pens = Pens.Gray;
				}
				else if (drawOption == DrawOption.EXPECTED)
				{
					brush = Brushes.DarkGray;
					pens = Pens.Gray;
				}
				else //if (drawOption == DrawOption.ERASE)
				{
					brush = Brushes.Black;
					pens = Pens.Black;
				}

				int x = PosX + GetBlockOffsetX(block);
				int y = PosY + GetBlockOffsetY(block) - Shape.BLOCK_SIZE;

				App.graphics.FillRectangle(brush, x, y, BLOCK_SIZE, BLOCK_SIZE);
				App.graphics.DrawRectangle(Pens.Gray, x, y, BLOCK_SIZE, BLOCK_SIZE);
			}
		}


		// private
		private int locX;
		private int locY;
		private int form;
		private int nextForm;
		private Type type;
		private Type nextType;

		private Brush BrushOnType
		{
			get
			{
				switch (type)
				{
					case Type.J:
						return Brushes.Blue;

					case Type.L:
						return Brushes.Orange;

					case Type.l:
						return Brushes.Pink;

					case Type.O:
						return Brushes.Yellow;

					case Type.S:
						return Brushes.Red;

					case Type.Z:
						return Brushes.Green;

					case Type.T:
						return Brushes.Violet;

					default:
						Debug.Fail("색깔 선택 에러");
						return null;
				}
			}
		}

		private int GetBlockOffsetX(int block) => BLOCK_OFFSET_DATA[(int)type, form, block].x * BLOCK_SIZE;
		private int GetBlockOffsetY(int block) => -BLOCK_OFFSET_DATA[(int)type, form, block].y * BLOCK_SIZE;
	}
}
