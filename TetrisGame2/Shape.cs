﻿using System;
using System.Drawing;
using System.Diagnostics;

namespace TetrisGame2
{
	public class Shape
	{
		/*******************************************************************************************************************/
		//public static
		public enum Type { S, Z, J, L, T, O, l, COUNT };

		public const int BLOCK_SIZE = 26;
		public const int MAX_BLOCK_DOWN_OFFSET = 1;
		public const int MAX_BLOCK_UP_OFFSET = 2;
		public const int BLOCK_COUNT = 4;
		public const int FORM_COUNT = 4;


		/*******************************************************************************************************************/
		// private static
		private static readonly int[] WEIGHT_SUM;

		private static readonly Location2D[,,] BLOCK_OFFSET_DATA; // Type,Form,Block

		private static Random randomForForm;
		private static Random randomForType;

		static Shape()
		{
			#region InitRandomTypeData()
			randomForForm = new Random();
			randomForType = new Random();

			int[] WEIGHT = new int[(int)Type.COUNT];

			WEIGHT[(int)Type.S] = 2;
			WEIGHT[(int)Type.Z] = 2;
			WEIGHT[(int)Type.J] = 2;
			WEIGHT[(int)Type.L] = 2;
			WEIGHT[(int)Type.T] = 4;
			WEIGHT[(int)Type.O] = 2;
			WEIGHT[(int)Type.l] = 4;

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


		}

		public static int MakeRandomForm()
		{
			return randomForForm.Next(0, FORM_COUNT);
		}

		public static Type MakeRandomType()
		{
			var result = randomForType.Next(1, WEIGHT_SUM[(int)Type.COUNT - 1]);

			for (int type = 0; type < (int)Type.COUNT; ++type)
			{
				if (result <= WEIGHT_SUM[type])
				{
					return (Type)type;
				}
			}

			Debug.Fail("MakeRandomType() Error");

			return Type.COUNT;
		}


		/*******************************************************************************************************************/
		// public 
		public int PosX { get => App.ToPointX(LocX); }
		public int PosY { get => App.ToPointY(LocY); }
		public int LocX { get; set; }
		public int LocY { get; set; }

		public int Form { get; set; }
		public Type Type_ { get; set; }

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

		public Shape()
		{
			Reset();
		}

		public Shape(Type type, int form, int locX, int locY)
		{
			this.Type_ = type;
			this.Form = form;
			this.LocX = locX;
			this.LocY = locY;
		}

		public Shape(Shape other) : this(other.Type_, other.Form, other.LocX, other.LocY) { }

		public void Reset()
		{
			InitLocation();

			Type_ = MakeRandomType();
			Form = MakeRandomForm();
		}

		public void InitLocation()
		{
			LocX = Stack.WIDTH / 2;
			LocY = Stack.VALID_HEIGHT + Shape.MAX_BLOCK_DOWN_OFFSET;
		}

		public void Transform()
		{
			++Form;
			if (Form == FORM_COUNT)
			{
				Form = 0;
			}
		}

		public void MoveDown() { --LocY; }
		public void MoveRight(int offset = 1) { LocX += offset; }
		public void MoveLeft(int offset = 1) { LocX -= offset; }
		public void MoveSide(int offset) { LocX += offset; }
		public void MoveSide(Side side) { LocX += (int)side; }

		public int GetBlockLocX(int block) { return LocX + BLOCK_OFFSET_DATA[(int)Type_, Form, block].x; }
		public int GetBlockLocY(int block) { return LocY + BLOCK_OFFSET_DATA[(int)Type_, Form, block].y; }

		public int GetBlockPosX(int block) { return App.ToPointX(GetBlockLocX(block)); }
		public int GetBlockPosY(int block) { return App.ToPointY(GetBlockLocY(block)); }


		/*******************************************************************************************************************/
		// private
	}
}
