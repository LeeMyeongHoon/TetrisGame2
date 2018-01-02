using System;
using System.Drawing;
using System.Diagnostics;

namespace TetrisGame2
{
	public class Shape
	{
		/*******************************************************************************************************************/
		//public static
		public enum Type { S, Z, J, L, T, O, l, COUNT };
		public enum DrawOption { Default, Expected, OnStack }

		public const int BLOCK_SIZE = 26;
		public const int MAX_BLOCK_DOWN_OFFSET = 1;
		public const int MAX_BLOCK_UP_OFFSET = 2;
		public const int BLOCK_COUNT = 4;
		public const int FORM_COUNT = 4;


		/*******************************************************************************************************************/
		// private static
		private static readonly int[] WEIGHT;
		private static readonly int[] WEIGHT_SUM;

		private static readonly Location2D[,,] BLOCK_OFFSET_DATA; // Type,Form,Block

		private static readonly Brush[] brushes;
		private static Random randomForForm;
		private static Random randomForType;

		static Shape()
		{
			#region InitRandomTypeData()
			randomForForm = new Random();
			randomForType = new Random();


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

			#region InitBlockOffsetData()
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

			#region InitBrushData()

			brushes = new Brush[(int)Type.COUNT];

			brushes[(int)Type.J] = Brushes.Blue;
			brushes[(int)Type.L] = Brushes.Orange;
			brushes[(int)Type.l] = Brushes.Pink;
			brushes[(int)Type.O] = Brushes.Yellow;
			brushes[(int)Type.S] = Brushes.Red;
			brushes[(int)Type.Z] = Brushes.GreenYellow;
			brushes[(int)Type.T] = Brushes.Violet;

			#endregion
		}

		private static int MakeRandomForm()
		{
			return randomForForm.Next(0, FORM_COUNT);
		}
		private static Type MakeRandomType()
		{
			var result = randomForType.Next(0, WEIGHT_SUM[(int)Type.COUNT - 1]);

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


		/*******************************************************************************************************************/
		// public 
		public int PosX { get => App.ToPointX(locX); }
		public int PosY { get => App.ToPointY(locY); }
		public int LocX { get => locX; set => locX = value; }
		public int LocY { get => locY; set => locY = value; }

		public int LowestBlockY
		{
			get
			{
				int lowestBlockY = 9999; // 괜히 큰값넣기
				for (int block = 0; block < Shape.BLOCK_COUNT; block++)
				{
					int blockY = GetBlockLocY(block);
					if (blockY < lowestBlockY)
					{
						lowestBlockY = blockY;
					}
				}
				return lowestBlockY;
			}
		}

		public int Form { get => form; set => form = value; }
		public int NextForm { get => nextForm; }
		public Type Type_ { get => type; set => type = value; }
		public Type NextType { get => nextType; }
		public Brush BrushOnType { get => brushes[(int)type]; }

		public Shape()
		{
			nextType = MakeRandomType();
			nextForm = MakeRandomForm();
			Reset();
		}

		public Shape(Type type, int form, int locX, int locY)
		{
			this.type = type;
			this.form = form;
			this.locX = locX;
			this.locY = locY;

			nextType = MakeRandomType();
			nextForm = MakeRandomForm();
		}
		public Shape(Shape other) : this(other.type, other.form, other.locX, other.locY) { }

		public void Reset()
		{
			locX = Stack.WIDTH / 2;
			locY = Stack.VALID_HEIGHT + Shape.MAX_BLOCK_DOWN_OFFSET;

			type = nextType;
			form = nextForm;

			nextType = MakeRandomType();
			nextForm = MakeRandomForm();
		}

		public void Transform()
		{
			++form;
			if (form == FORM_COUNT)
			{
				form = 0;
			}
		}

		public void Draw(DrawOption drawOption = DrawOption.Default)
		{
			for (int block = 0; block < BLOCK_COUNT; ++block)
			{
				if (drawOption == DrawOption.Default)
				{
					int x = GetBlockPosX(block);
					int y = GetBlockPosY(block) - Shape.BLOCK_SIZE;

					App.Graphics.FillRectangle(BrushOnType, x, y, BLOCK_SIZE, BLOCK_SIZE);
					App.Graphics.DrawRectangle(Pens.Gray, x, y, BLOCK_SIZE, BLOCK_SIZE);

				}
				else // if (drawOption == DrawOption.EXPECTED)
				{
					int x = GetBlockPosX(block);
					int y = GetBlockPosY(block) - Shape.BLOCK_SIZE;

					App.Graphics.DrawRectangle(Pens.Gray, x, y, BLOCK_SIZE, BLOCK_SIZE);
				}
			}
		}

		public void MoveDown() { --locY; }
		public void MoveRight(int offset = 1) { locX += offset; }
		public void MoveLeft(int offset = 1) { locX -= offset; }
		public void MoveSide(int offset) { locX += offset; }
		public void MoveSide(Side side) { locX += (int)side; }


		public int GetBlockLocX(int block) { return locX + BLOCK_OFFSET_DATA[(int)type, form, block].x; }
		public int GetBlockLocY(int block) { return locY + BLOCK_OFFSET_DATA[(int)type, form, block].y; }

		public int GetBlockPosX(int block) { return App.ToPointX(locX + BLOCK_OFFSET_DATA[(int)type, form, block].x); }
		public int GetBlockPosY(int block) { return App.ToPointY(locY + BLOCK_OFFSET_DATA[(int)type, form, block].y); }

		/*******************************************************************************************************************/
		// private
		private int locX;
		private int locY;
		private int form;
		private int nextForm;
		private Type type;
		private Type nextType;


	}
}
