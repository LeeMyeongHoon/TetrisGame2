using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace TetrisGame2
{
	partial class Shape
	{
		// public
		enum Type
		{
			S, Z, J, L, T, O, l, COUNT
		};

		public const int MAX_BLOCK_DOWN_OFFSET = 1;
		public const int MAX_BLOCK_UP_OFFSET = 2;
		public const int BLOCK_COUNT = 4;
		public const int FORM_COUNT = 4;

		// private
		Pos2D[,,] blockOffsetData;
		int posX;
		int posY;

		int form;
		int nextForm;

		Type type;
		Type nextType;

		int[] weight;
		int[] weightSum;


		// public
		public Shape(Graphics g)
		{
			InitRandomData();
			InitBlockOffsetData();
		}


		// private
		private void InitRandomData()
		{
			weight[(int)Type.J] = 2;
			weight[(int)Type.L] = 2;
			weight[(int)Type.l] = 2;
			weight[(int)Type.O] = 2;
			weight[(int)Type.S] = 4;
			weight[(int)Type.T] = 2;
			weight[(int)Type.Z] = 5;

			weightSum[0] = 2;
			for (int i = 1; i < (int)Type.COUNT; ++i)
			{
				weightSum[i] = weightSum[i - 1] + weight[i];
			}
		}

		private void InitBlockOffsetData()
		{
			blockOffsetData = new Pos2D[(int)Type.COUNT, FORM_COUNT, BLOCK_COUNT];

			int type;
			int form;

			type = (int)Type.J;

			//   [0]
			//	 [1]
			//[2][3]
			form = 0;
			blockOffsetData[type, form, 0] = new Pos2D(+1, +1);
			blockOffsetData[type, form, 1] = new Pos2D(+1, +0);
			blockOffsetData[type, form, 2] = new Pos2D(+0, -1);
			blockOffsetData[type, form, 3] = new Pos2D(+1, -1);

			//[0]
			//[1][2][3]
			form = 1;
			blockOffsetData[type, form, 0] = new Pos2D(-1, +0);
			blockOffsetData[type, form, 1] = new Pos2D(-1, -1);
			blockOffsetData[type, form, 2] = new Pos2D(+0, -1);
			blockOffsetData[type, form, 3] = new Pos2D(+1, -1);

			//[0][1]
			//[2]
			//[3]
			form = 2;
			blockOffsetData[type, form, 0] = new Pos2D(-1, +1);
			blockOffsetData[type, form, 1] = new Pos2D(+0, +1);
			blockOffsetData[type, form, 2] = new Pos2D(-1, +0);
			blockOffsetData[type, form, 3] = new Pos2D(-1, -1);

			//[0][1][2]
			//      [3]
			form = 3;
			blockOffsetData[type, form, 0] = new Pos2D(-1, +1);
			blockOffsetData[type, form, 1] = new Pos2D(+0, +1);
			blockOffsetData[type, form, 2] = new Pos2D(+1, +1);
			blockOffsetData[type, form, 3] = new Pos2D(+1, +0);



			type = (int)Type.L;

			//[0]
			//[1]
			//[2][3]
			form = 0;
			blockOffsetData[type, form, 0] = new Pos2D(-1, +1);
			blockOffsetData[type, form, 1] = new Pos2D(-1, +0);
			blockOffsetData[type, form, 2] = new Pos2D(-1, -1);
			blockOffsetData[type, form, 3] = new Pos2D(+0, -1);

			//[1][2][3]
			//[0]
			form = 1;
			blockOffsetData[type, form, 0] = new Pos2D(-1, +0);
			blockOffsetData[type, form, 1] = new Pos2D(-1, +1);
			blockOffsetData[type, form, 2] = new Pos2D(+0, +1);
			blockOffsetData[type, form, 3] = new Pos2D(+1, +1);

			//[0][1]
			//   [2]
			//	 [3]
			form = 2;
			blockOffsetData[type, form, 0] = new Pos2D(+0, +1);
			blockOffsetData[type, form, 1] = new Pos2D(+1, +1);
			blockOffsetData[type, form, 2] = new Pos2D(+1, +0);
			blockOffsetData[type, form, 3] = new Pos2D(+1, -1);

			//      [3]
			//[0][1][2]
			form = 3;
			blockOffsetData[type, form, 0] = new Pos2D(-1, -1);
			blockOffsetData[type, form, 1] = new Pos2D(+0, -1);
			blockOffsetData[type, form, 2] = new Pos2D(+1, -1);
			blockOffsetData[type, form, 3] = new Pos2D(+1, +0);


			type = (int)Type.Z;

			//[0][1]
			//   [2][3]
			// form 0, form 2
			blockOffsetData[type, 0, 0] = blockOffsetData[type, 2, 0] = new Pos2D(-1, +1);
			blockOffsetData[type, 0, 1] = blockOffsetData[type, 2, 1] = new Pos2D(+0, +1);
			blockOffsetData[type, 0, 2] = blockOffsetData[type, 2, 2] = new Pos2D(+0, +0);
			blockOffsetData[type, 0, 3] = blockOffsetData[type, 2, 3] = new Pos2D(+1, +0);

			//   [0]
			//[1][2]
			//[3]
			// form 1, form 3
			blockOffsetData[type, 1, 0] = blockOffsetData[type, 3, 0] = new Pos2D(+0, +1);
			blockOffsetData[type, 1, 1] = blockOffsetData[type, 3, 1] = new Pos2D(-1, +0);
			blockOffsetData[type, 1, 2] = blockOffsetData[type, 3, 2] = new Pos2D(+0, +0);
			blockOffsetData[type, 1, 3] = blockOffsetData[type, 3, 3] = new Pos2D(-1, -1);


			type = (int)Type.l;

			//[0]
			//[1]
			//[2]
			//[3]
			//form 0, form 2
			blockOffsetData[type, 0, 0] = blockOffsetData[type, 2, 0] = new Pos2D(+0, +2);
			blockOffsetData[type, 0, 1] = blockOffsetData[type, 2, 1] = new Pos2D(+0, +1);
			blockOffsetData[type, 0, 2] = blockOffsetData[type, 2, 2] = new Pos2D(+0, +0);
			blockOffsetData[type, 0, 3] = blockOffsetData[type, 2, 3] = new Pos2D(+0, -1);

			//[0][1][2][3]
			//form 1, form 3
			blockOffsetData[type, 1, 0] = blockOffsetData[type, 3, 0] = new Pos2D(-1, -1);
			blockOffsetData[type, 1, 1] = blockOffsetData[type, 3, 1] = new Pos2D(+0, -1);
			blockOffsetData[type, 1, 2] = blockOffsetData[type, 3, 2] = new Pos2D(+1, -1);
			blockOffsetData[type, 1, 3] = blockOffsetData[type, 3, 3] = new Pos2D(+2, -1);


			type = (int)Type.O;

			//[2][3]
			//[0][1]
			//fomr 0,1,2,3
			blockOffsetData[type, 0, 0] = blockOffsetData[type, 1, 0] = blockOffsetData[type, 2, 0] = blockOffsetData[type, 3, 0] = new Pos2D(-1, -1);
			blockOffsetData[type, 0, 1] = blockOffsetData[type, 1, 1] = blockOffsetData[type, 2, 1] = blockOffsetData[type, 3, 1] = new Pos2D(+0, -1);
			blockOffsetData[type, 0, 2] = blockOffsetData[type, 1, 2] = blockOffsetData[type, 2, 2] = blockOffsetData[type, 3, 2] = new Pos2D(-1, +0);
			blockOffsetData[type, 0, 3] = blockOffsetData[type, 1, 3] = blockOffsetData[type, 2, 3] = blockOffsetData[type, 3, 3] = new Pos2D(+0, +0);

			type = (int)Type.S;

			//   [0][1]
			//[2][3]
			// form 0, form 2
			blockOffsetData[type, 0, 0] = blockOffsetData[type, 2, 0] = new Pos2D(+0, +0);
			blockOffsetData[type, 0, 1] = blockOffsetData[type, 2, 1] = new Pos2D(+1, +0);
			blockOffsetData[type, 0, 2] = blockOffsetData[type, 2, 2] = new Pos2D(-1, -1);
			blockOffsetData[type, 0, 3] = blockOffsetData[type, 2, 3] = new Pos2D(+0, -1);

			//[0]
			//[1][2]
			//   [3]
			// form 1, form 3
			blockOffsetData[type, 1, 0] = blockOffsetData[type, 3, 0] = new Pos2D(-1, +1);
			blockOffsetData[type, 1, 1] = blockOffsetData[type, 3, 1] = new Pos2D(-1, +0);
			blockOffsetData[type, 1, 2] = blockOffsetData[type, 3, 2] = new Pos2D(+0, +0);
			blockOffsetData[type, 1, 3] = blockOffsetData[type, 3, 3] = new Pos2D(+0, -1);

			type = (int)Type.T;

			//   [0]
			//[1][2][3]
			form = 0;
			blockOffsetData[type, form, 0] = new Pos2D(+0, +0);
			blockOffsetData[type, form, 1] = new Pos2D(-1, -1);
			blockOffsetData[type, form, 2] = new Pos2D(+0, -1);
			blockOffsetData[type, form, 3] = new Pos2D(+1, -1);

			//[0]
			//[1][2]
			//[3]
			form = 1;
			blockOffsetData[type, form, 0] = new Pos2D(-1, +1);
			blockOffsetData[type, form, 1] = new Pos2D(-1, +0);
			blockOffsetData[type, form, 2] = new Pos2D(+0, +0);
			blockOffsetData[type, form, 3] = new Pos2D(-1, -1);

			//[1][2][3]
			//   [0]
			form = 2;
			blockOffsetData[type, form, 0] = new Pos2D(+0, +0);
			blockOffsetData[type, form, 1] = new Pos2D(-1, +1);
			blockOffsetData[type, form, 2] = new Pos2D(+0, +1);
			blockOffsetData[type, form, 3] = new Pos2D(+1, +1);

			//   [0]
			//[1][2]
			//   [3]
			form = 3;
			blockOffsetData[type, form, 0] = new Pos2D(+1, +1);
			blockOffsetData[type, form, 1] = new Pos2D(+0, +0);
			blockOffsetData[type, form, 2] = new Pos2D(+1, +0);
			blockOffsetData[type, form, 3] = new Pos2D(+1, -1);

		}

		private Type MakeRandomType()
		{
			Random random = new Random();
			var result = random.Next(0, weightSum[(int)Type.COUNT - 1]);

			for (int type = 0; type < (int)Type.COUNT; ++type)
			{
				if (result < weightSum[type])
				{
					return (Type)type;
				}
			}

			Debug.Fail("MakeRandomType() Error");

			return Type.COUNT;
		}

		private static int MakeRandomForm()
		{
			return new Random().Next(0, FORM_COUNT);
		}


	}
}
